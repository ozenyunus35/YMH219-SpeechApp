using System.Drawing.Drawing2D;
using SpeechApp.Kontroller;
using SpeechApp.Model;
using SpeechApp.Servisler;

namespace SpeechApp;

public partial class AnaForm : Form
{
    private const string OrnekMetin =
        "Merhaba! Bu uygulama Nesne Tabanlı Programlama dersi için hazırlandı. " +
        "Buraya yazdığınız metni seçtiğiniz sesle okuyabilir, okumayı duraklatıp devam ettirebilir " +
        "ve isterseniz sesi bir dosyaya kaydedebilirsiniz.";

    private const long EnBuyukMetinDosyasi = 1_000_000;

    /// <summary>
    /// .txt dosyasını okur. BOM'suz dosya geçerli UTF-8 değilse eski Not Defteri'nin
    /// Türkçe ANSI kodlaması (Windows-1254) olarak okunuyor, böylece ş/ğ/ı bozulmuyor.
    /// </summary>
    internal static string MetinDosyasiOku(string yol)
    {
        byte[] veri = File.ReadAllBytes(yol);
        if (veri.Length >= 2 && (veri[0] == 0xFF && veri[1] == 0xFE || veri[0] == 0xFE && veri[1] == 0xFF))
            return File.ReadAllText(yol);   // UTF-16 BOM
        int bas = veri.Length >= 3 && veri[0] == 0xEF && veri[1] == 0xBB && veri[2] == 0xBF ? 3 : 0;
        try
        {
            return new System.Text.UTF8Encoding(false, throwOnInvalidBytes: true).GetString(veri, bas, veri.Length - bas);
        }
        catch (System.Text.DecoderFallbackException)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            return System.Text.Encoding.GetEncoding(1254).GetString(veri);
        }
    }

    private static readonly Color VurguRengi = Color.FromArgb(255, 222, 236);

    private IMetinOkuyucu? okuyucu;
    private IKonusmaTanima? tanima;
    private OkumaYoneticisi? okuma;
    private DinlemeYoneticisi? dinleme;
    private int vurguKonum = -1, vurguUzunluk;

    public AnaForm()
    {
        InitializeComponent();
        sekmeSecici.Sekmeler = new[] { "Metni sesli oku", "Konuşmayı yazıya dök" };

        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
        catch (Exception) { /* varsayılan ikon */ }
    }

    /// <summary>Motorları dışarıdan vermek için (ör. test sırasında sahte mikrofon).</summary>
    public AnaForm(IMetinOkuyucu okuyucu, IKonusmaTanima tanima) : this()
    {
        this.okuyucu = okuyucu;
        this.tanima = tanima;
    }

    /// <summary>Konuşma motorunun olayları farklı bir thread'den gelebilir; ekranı UI thread'inde güncelle.</summary>
    private void UI(Action islem)
    {
        if (IsDisposed || Disposing || !IsHandleCreated) return;
        if (!InvokeRequired)
        {
            islem();
            return;
        }
        try
        {
            BeginInvoke(() => { if (!IsDisposed) islem(); });
        }
        catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException)
        {
            // Pencere kapanırken motorun geç gelen olayı: yok sayılıyor
        }
    }

    // ===================== Açılış =====================

    private void AnaForm_Load(object? sender, EventArgs e)
    {
        rtbMetin.Text = OrnekMetin;
        OkuyucuyuHazirla();
        TanimayiHazirla();
        MotorBilgisiniGoster();
        OkumaArayuzunuGuncelle();
        DinlemeArayuzunuGuncelle();

        ipucu.SetToolTip(btnWavKaydet, "Metni seçili sesle okuyup .wav dosyası olarak kaydeder");
        ipucu.SetToolTip(btnDosyadanTani, "Mikrofonunuz yoksa önceden kaydedilmiş bir konuşmayı yazıya dökebilirsiniz");
        ipucu.SetToolTip(btnOku, "Sesli oku (Ctrl+Enter veya F5)");
        ipucu.SetToolTip(btnDuraklat, "Duraklat / devam et (F6)");
        ipucu.SetToolTip(btnDurdur, "Okumayı durdur (Esc)");
        ipucu.SetToolTip(btnTemizle, "Metin kutusunu boşalt");
        ipucu.SetToolTip(btnMikrofon, "Dinlemeyi başlat / durdur (Ctrl+Enter veya F5)");
        ipucu.SetToolTip(btnMetinAc, "Metin dosyası aç (Ctrl+O)");
        ipucu.SetToolTip(btnMetinKaydet, "Metni .txt olarak kaydet (Ctrl+S)");
        ipucu.SetToolTip(btnYaziKaydet, "Tanıma sonuçlarını .txt olarak kaydet (Ctrl+S)");
        ipucu.SetToolTip(trkHiz, "Okuma hızı (okuma sırasında değiştirilemez)");
        ipucu.SetToolTip(trkSesDuzeyi, "Ses düzeyi (okuma sırasında değiştirilemez)");
        ipucu.SetToolTip(sekmeSecici, "Sekmeler: Ctrl+1 / Ctrl+2");

        // İki sekme paneli tek bir içerik panelinde: uyarı şeridi görünüp kaybolunca içerik her zaman
        // şeridin altına yerleşiyor (önce mikrofon sekmesinde şerit kartların üstünü örtüyordu)
        var icerik = new Panel { Dock = DockStyle.Fill, BackColor = pnlOku.BackColor };
        SuspendLayout();
        Controls.Remove(pnlOku);
        Controls.Remove(pnlDinle);
        icerik.Controls.Add(pnlDinle);
        icerik.Controls.Add(pnlOku);
        Controls.Add(icerik);
        icerik.BringToFront();
        ResumeLayout(true);

        // Pencere varsayılan boyutundan küçültülünce kartlardaki alt butonlar kesiliyordu
        MinimumSize = Size;
        rtbMetin.HideSelection = false;        // vurgulanan kelimeye kaydırma odak yokken de çalışsın
        rtbYazi.ReadOnly = true;               // her sonuçta yeniden yazılıyor; elle düzenleme kaybolmasın
        rtbYazi.BackColor = Color.White;
        rtbMetin.TextChanged += (_, _) => ButonlariGuncelle();
        lvSonuclar.Resize += (_, _) => SutunlariAyarla();
        SutunlariAyarla();
        bildirimZamanlayici.Tick += (_, _) => { bildirimZamanlayici.Stop(); UyariGizle(); };
        ButonlariGuncelle();

        if (okuyucu != null && okuyucu.Sesler.Count > 0 && !okuyucu.Sesler.Any(s => s.Dil.StartsWith("tr", StringComparison.OrdinalIgnoreCase)))
            Bildir($"Bu bilgisayarda Türkçe ses yüklü değil; metin {okuyucu.Sesler[0].Dil} sesiyle okunacak, Türkçe telaffuz bozuk olabilir.", hata: false);
    }

    private readonly System.Windows.Forms.Timer bildirimZamanlayici = new() { Interval = 6000 };

    private void SutunlariAyarla()
    {
        int kalan = lvSonuclar.ClientSize.Width - colSaat.Width - colGuven.Width - SystemInformation.VerticalScrollBarWidth - 4;
        if (kalan > 100)
            colMetin.Width = kalan;
    }

    /// <summary>Boş metin / sonuç yokken işe yaramayan butonlar pasif.</summary>
    private void ButonlariGuncelle()
    {
        bool metinVar = rtbMetin.Text.Trim().Length > 0;
        bool okumaHazir = (okuma?.Durum ?? OkumaDurumu.Hazir) == OkumaDurumu.Hazir;
        btnMetinKaydet.Enabled = metinVar;
        btnWavKaydet.Enabled = metinVar && okumaHazir && (okuyucu?.Sesler.Count ?? 0) > 0;
        btnTemizle.Enabled = metinVar;
        btnYaziKaydet.Enabled = lvSonuclar.Items.Count > 0;
        btnYaziTemizle.Enabled = lvSonuclar.Items.Count > 0 || rtbYazi.TextLength > 0;
    }

    // Klavye kısayolları
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool okuSekmesi = pnlOku.Visible;
        switch (keyData)
        {
            case Keys.Control | Keys.Enter:
            case Keys.F5:
                if (okuSekmesi) { if (btnOku.Enabled) OkumayaBasla(); }
                else MikrofonDugmesi();
                return true;
            case Keys.F6:
                if (okuSekmesi && btnDuraklat.Enabled) okuma?.DuraklatVeyaDevamEt();
                return true;
            case Keys.Escape:
                if (okuSekmesi && okuma is { Durum: not OkumaDurumu.Hazir }) okuma.Durdur();
                else if (!okuSekmesi && dinleme is { Durum: not DinlemeDurumu.Hazir }) dinleme.Durdur();
                else if (pnlUyari.Visible) UyariGizle();
                return true;
            case Keys.Control | Keys.D1:
                sekmeSecici.Secili = 0;
                return true;
            case Keys.Control | Keys.D2:
                sekmeSecici.Secili = 1;
                return true;
            case Keys.Control | Keys.O:
                if (okuSekmesi) btnMetinAc.PerformClick();
                return true;
            case Keys.Control | Keys.S:
                (okuSekmesi ? btnMetinKaydet : btnYaziKaydet).PerformClick();
                return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OkuyucuyuHazirla()
    {
        try
        {
            okuyucu ??= new SistemMetinOkuyucu();
        }
        catch (KonusmaHatasi ex)
        {
            Bildir(ex.Message, hata: true);
            btnOku.Enabled = btnWavKaydet.Enabled = false;
            return;
        }

        okuma = new OkumaYoneticisi(okuyucu);
        okuma.DurumDegisti += (_, _) => UI(OkumaArayuzunuGuncelle);
        okuma.KelimeVurgula += (_, k) => UI(() => KelimeyiVurgula(k.Konum, k.Uzunluk));
        okuma.HataOlustu += (_, mesaj) => UI(() => Bildir(mesaj, hata: true));

        cmbSes.Items.Clear();
        foreach (var ses in okuyucu.Sesler)
            cmbSes.Items.Add(ses);

        if (cmbSes.Items.Count > 0)
        {
            // Salt okunur olunca RichTextBox griye dönüyor; vurgu temizlenirken lekeler kalmasın diye sabit beyaz
            rtbMetin.BackColor = Color.White;

            // Türkçe ses varsa onu seç
            var turkce = okuyucu.Sesler.FirstOrDefault(s => s.Dil.StartsWith("tr", StringComparison.OrdinalIgnoreCase));
            cmbSes.SelectedItem = turkce ?? okuyucu.Sesler[0];
        }
        else
        {
            cmbSes.Items.Add("(yüklü ses bulunamadı)");
            cmbSes.SelectedIndex = 0;
            cmbSes.Enabled = false;
        }
    }

    private void TanimayiHazirla()
    {
        tanima ??= new SistemKonusmaTanima();
        dinleme = new DinlemeYoneticisi(tanima);
        dinleme.DurumDegisti += (_, _) => UI(DinlemeArayuzunuGuncelle);
        dinleme.SonucEklendi += (_, s) => UI(() => SonucuGoster(s));
        tanima.TahminDegisti += (_, t) => UI(() => lblTahmin.Text = "Duyulan: " + t + " ...");
        tanima.SesSeviyesi += (_, s) => UI(() => dalgaDinle.Seviye = s);

        cmbDil.Items.Clear();
        foreach (string dil in tanima.Diller)
            cmbDil.Items.Add(dil);

        if (cmbDil.Items.Count > 0)
        {
            cmbDil.SelectedIndex = 0;
            bool turkceVar = tanima.Diller.Any(d => d.StartsWith("tr", StringComparison.OrdinalIgnoreCase));
            lblDilIpucu.Text = turkceVar
                ? "Türkçe tanıyıcı yüklü, Türkçe konuşabilirsiniz."
                : $"Not: Bu bilgisayarda Türkçe tanıyıcı yok; {cmbDil.Items[0]} dilinde konuşun. (Windows Türkçe masaüstü tanıyıcı sunmuyor.)";
        }
        else
        {
            cmbDil.Items.Add("(tanıyıcı yok)");
            cmbDil.SelectedIndex = 0;
            cmbDil.Enabled = false;
            lblDilIpucu.Text = "Bu bilgisayarda konuşma tanıma motoru yüklü değil. Mikrofona basınca ne yapmanız gerektiği gösterilir.";
        }
    }

    private void MotorBilgisiniGoster()
    {
        int sesSayisi = okuyucu?.Sesler.Count ?? 0;
        var diller = tanima?.Diller ?? Array.Empty<string>();
        lblMotor.Text = $"Yüklü ses: {sesSayisi}   ·   Tanıma dilleri: {(diller.Count > 0 ? string.Join(", ", diller) : "yok")}   ·   Motor: Windows SAPI (System.Speech)";
    }

    // ===================== Metni oku (TTS) =====================

    private void btnOku_Click(object? sender, EventArgs e) => OkumayaBasla();

    internal bool OkumayaBasla()
    {
        if (okuma is null) return false;
        try
        {
            VurguyuTemizle();
            okuma.Basla(rtbMetin.Text);
            UyariGizle();
            return true;
        }
        catch (KonusmaHatasi ex)
        {
            Bildir(ex.Message, hata: true);
            return false;
        }
    }

    private void btnDuraklat_Click(object? sender, EventArgs e) => okuma?.DuraklatVeyaDevamEt();

    private void btnDurdur_Click(object? sender, EventArgs e) => okuma?.Durdur();

    private void btnTemizle_Click(object? sender, EventArgs e)
    {
        if (rtbMetin.TextLength > 0 &&
            MessageBox.Show(this, "Metin kutusu boşaltılsın mı?", "Temizle", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;
        okuma?.Durdur();
        rtbMetin.Clear();
        rtbMetin.Focus();
    }

    private void OkumaArayuzunuGuncelle()
    {
        var durum = okuma?.Durum ?? OkumaDurumu.Hazir;
        bool hazir = durum == OkumaDurumu.Hazir;

        btnOku.Enabled = okuma != null && hazir;
        btnDuraklat.Enabled = !hazir;
        btnDurdur.Enabled = !hazir;
        btnDuraklat.Text = durum == OkumaDurumu.Duraklatildi ? "▶  Devam et" : "❚❚  Duraklat";
        rtbMetin.ReadOnly = !hazir;
        cmbSes.Enabled = hazir && (okuyucu?.Sesler.Count ?? 0) > 0;
        // Hız ve ses düzeyi okuma başlarken motora veriliyor; okurken değiştirmek işe yaramadığı için kilitli
        trkHiz.Enabled = trkSesDuzeyi.Enabled = hazir;
        // Okurken mikrofon kendi sesimizi duymasın
        btnMikrofon.Enabled = hazir || (dinleme?.Durum ?? DinlemeDurumu.Hazir) != DinlemeDurumu.Hazir;
        ButonlariGuncelle();
        dalgaOku.Aktif = durum == OkumaDurumu.Okuyor;

        int ilerleme = okuma?.Ilerleme ?? 0;
        prgOku.Value = Math.Clamp(ilerleme, 0, 100);
        lblIlerleme.Text = durum switch
        {
            OkumaDurumu.Okuyor => $"Okunuyor: %{ilerleme}",
            OkumaDurumu.Duraklatildi => $"Duraklatıldı: %{ilerleme}",
            _ => ilerleme >= 100 ? "Okuma tamamlandı" : $"Okunan: %{ilerleme}"
        };

        if (hazir)
            VurguyuTemizle();

        DurumEtiketiniGuncelle();
    }

    private void KelimeyiVurgula(int konum, int uzunluk)
    {
        if (okuma is null || okuma.Durum == OkumaDurumu.Hazir)
            return;
        if (konum < 0 || konum + uzunluk > rtbMetin.TextLength)
            return;

        VurguyuTemizle();
        rtbMetin.Select(konum, uzunluk);
        rtbMetin.SelectionBackColor = VurguRengi;
        rtbMetin.SelectionColor = Renkler.GulKoyu;
        rtbMetin.Select(konum + uzunluk, 0);
        rtbMetin.ScrollToCaret();
        vurguKonum = konum;
        vurguUzunluk = uzunluk;

        int ilerleme = okuma?.Ilerleme ?? 0;
        prgOku.Value = Math.Clamp(ilerleme, 0, 100);
        lblIlerleme.Text = $"Okunuyor: %{ilerleme}";
    }

    private void VurguyuTemizle()
    {
        if (vurguKonum < 0 || vurguKonum + vurguUzunluk > rtbMetin.TextLength)
        {
            vurguKonum = -1;
            return;
        }
        rtbMetin.Select(vurguKonum, vurguUzunluk);
        rtbMetin.SelectionBackColor = rtbMetin.BackColor;
        rtbMetin.SelectionColor = rtbMetin.ForeColor;
        rtbMetin.Select(0, 0);
        vurguKonum = -1;
    }

    private void rtbMetin_TextChanged(object? sender, EventArgs e)
    {
        int kelime = rtbMetin.Text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        lblKarakter.Text = $"{rtbMetin.TextLength} karakter  ·  {kelime} kelime";
    }

    private void cmbSes_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (okuyucu is null || cmbSes.SelectedItem is not SesBilgisi ses) return;
        try
        {
            okuyucu.SeciliSes = ses.Ad;
        }
        catch (KonusmaHatasi ex)
        {
            Bildir(ex.Message, hata: true);
        }
    }

    private void trkHiz_ValueChanged(object? sender, EventArgs e)
    {
        if (okuyucu != null) okuyucu.Hiz = trkHiz.Value;
        lblHizDeger.Text = trkHiz.Value switch
        {
            0 => "normal",
            < 0 => $"yavaş ({trkHiz.Value})",
            _ => $"hızlı (+{trkHiz.Value})"
        };
    }

    private void trkSesDuzeyi_ValueChanged(object? sender, EventArgs e)
    {
        if (okuyucu != null) okuyucu.SesDuzeyi = trkSesDuzeyi.Value;
        lblSesDuzeyiDeger.Text = "%" + trkSesDuzeyi.Value;
    }

    private void btnWavKaydet_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog { Title = "Sesi kaydet", Filter = "WAV ses dosyası (*.wav)|*.wav", FileName = $"okuma-{DateTime.Now:yyyyMMdd-HHmm}.wav" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            WavKaydet(dialog.FileName);
    }

    internal bool WavKaydet(string yol)
    {
        if (okuyucu is null) return false;
        try
        {
            UseWaitCursor = true;
            btnWavKaydet.Enabled = false;
            Refresh();     // uzun metinde pencere donmuş gibi görünmesin, bekleme imleci hemen çizilsin
            okuyucu.DosyayaKaydet(rtbMetin.Text, yol);
            Bildir($"Ses kaydedildi: {Path.GetFileName(yol)}  ({new FileInfo(yol).Length / 1024} KB)", hata: false);
            return true;
        }
        catch (KonusmaHatasi ex)
        {
            Bildir(ex.Message, hata: true);
            return false;
        }
        finally
        {
            UseWaitCursor = false;
            ButonlariGuncelle();
        }
    }

    private void btnMetinAc_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Title = "Metin dosyası aç", Filter = "Metin dosyası (*.txt)|*.txt|Tüm dosyalar (*.*)|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            if (new FileInfo(dialog.FileName).Length > EnBuyukMetinDosyasi)
            {
                Bildir("Dosya çok büyük (en fazla 1 MB metin açılabilir).", hata: true);
                return;
            }
            okuma?.Durdur();
            rtbMetin.Text = MetinDosyasiOku(dialog.FileName);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
        {
            Bildir("Dosya açılamadı: " + ex.Message, hata: true);
        }
    }

    private void btnMetinKaydet_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog { Title = "Metni kaydet", Filter = "Metin dosyası (*.txt)|*.txt", FileName = "metin.txt" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            File.WriteAllText(dialog.FileName, rtbMetin.Text);
            Bildir("Metin kaydedildi: " + Path.GetFileName(dialog.FileName), hata: false);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Bildir("Dosya kaydedilemedi: " + ex.Message, hata: true);
        }
    }

    // ===================== Konuşmayı yazıya dök (STT) =====================

    private void btnMikrofon_Click(object? sender, EventArgs e) => MikrofonDugmesi();

    internal void MikrofonDugmesi()
    {
        if (dinleme is null) return;
        if (dinleme.Durum != DinlemeDurumu.Hazir)
        {
            dinleme.Durdur();
            return;
        }

        try
        {
            dinleme.MikrofonuBaslat();
            UyariGizle();
        }
        catch (KonusmaHatasi ex)
        {
            // Mikrofon veya tanıma motoru yoksa program kapanmıyor, ne yapılacağı anlatılıyor
            Bildir(ex.Message, hata: true);
        }
    }

    private void btnDosyadanTani_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Title = "Konuşma kaydı seç", Filter = "WAV ses dosyası (*.wav)|*.wav" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            DosyadanTani(dialog.FileName);
    }

    internal bool DosyadanTani(string yol)
    {
        if (dinleme is null) return false;
        try
        {
            dinleme.DosyadanBaslat(yol);
            UyariGizle();
            return true;
        }
        catch (KonusmaHatasi ex)
        {
            Bildir(ex.Message, hata: true);
            return false;
        }
    }

    private void cmbDil_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (tanima != null && cmbDil.SelectedItem is string dil && cmbDil.Enabled)
            tanima.SeciliDil = dil;
    }

    private void SonucuGoster(TanimaSonucu s)
    {
        rtbYazi.Text = dinleme?.Gecmis.TamMetin() ?? "";
        rtbYazi.SelectionStart = rtbYazi.TextLength;
        rtbYazi.ScrollToCaret();
        lblTahmin.Text = "";

        var item = new ListViewItem(s.Zaman.ToString("HH:mm:ss"));
        item.SubItems.Add(s.Metin);
        item.SubItems.Add(s.Guven.ToString("P0"));
        if (s.Guven < 0.5f)
            item.ForeColor = Renkler.Turuncu;      // düşük güvenli sonuçları farklı göster
        lvSonuclar.Items.Add(item);
        item.EnsureVisible();
        ButonlariGuncelle();
    }

    private void DinlemeArayuzunuGuncelle()
    {
        var durum = dinleme?.Durum ?? DinlemeDurumu.Hazir;
        btnMikrofon.Dinliyor = durum != DinlemeDurumu.Hazir;
        dalgaDinle.Aktif = durum != DinlemeDurumu.Hazir;
        btnDosyadanTani.Enabled = durum == DinlemeDurumu.Hazir;
        bool okumaHazir = (okuma?.Durum ?? OkumaDurumu.Hazir) == OkumaDurumu.Hazir;
        btnMikrofon.Enabled = okumaHazir || durum != DinlemeDurumu.Hazir;
        cmbDil.Enabled = durum == DinlemeDurumu.Hazir && (tanima?.Diller.Count ?? 0) > 0;

        lblMikDurum.Text = durum switch
        {
            DinlemeDurumu.MikrofonDinleniyor => "Dinleniyor, konuşabilirsiniz",
            DinlemeDurumu.DosyaIsleniyor => "Ses dosyası işleniyor... (durdurmak için tıklayın)",
            _ => "Dinlemek için mikrofona tıklayın"
        };
        lblMikDurum.ForeColor = durum == DinlemeDurumu.Hazir ? Renkler.Yazi : Renkler.GulKoyu;
        if (durum == DinlemeDurumu.Hazir)
            lblTahmin.Text = "";

        DurumEtiketiniGuncelle();
    }

    private void btnYaziTemizle_Click(object? sender, EventArgs e)
    {
        dinleme?.Gecmis.Temizle();
        rtbYazi.Clear();
        lvSonuclar.Items.Clear();
        ButonlariGuncelle();
    }

    private void btnYaziKaydet_Click(object? sender, EventArgs e)
    {
        if (dinleme is null || dinleme.Gecmis.Sonuclar.Count == 0)
        {
            Bildir("Kaydedilecek sonuç yok.", hata: true);
            return;
        }
        using var dialog = new SaveFileDialog { Title = "Sonuçları kaydet", Filter = "Metin dosyası (*.txt)|*.txt", FileName = $"tanima-{DateTime.Now:yyyyMMdd-HHmm}.txt" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            SonuclariKaydet(dialog.FileName);
    }

    internal bool SonuclariKaydet(string yol)
    {
        try
        {
            dinleme!.Gecmis.Kaydet(yol);
            Bildir($"{dinleme.Gecmis.Sonuclar.Count} sonuç kaydedildi: {Path.GetFileName(yol)}", hata: false);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            Bildir("Dosya kaydedilemedi: " + ex.Message, hata: true);
            return false;
        }
    }

    // ===================== Genel =====================

    private void sekmeSecici_SecimDegisti(object? sender, EventArgs e)
    {
        pnlOku.Visible = sekmeSecici.Secili == 0;
        pnlDinle.Visible = sekmeSecici.Secili == 1;
        PerformLayout();      // uyarı şeridi görünürken yerleşim yeniden hesaplansın (kartın üstü kapanmasın)
    }

    private void DurumEtiketiniGuncelle()
    {
        var o = okuma?.Durum ?? OkumaDurumu.Hazir;
        var d = dinleme?.Durum ?? DinlemeDurumu.Hazir;

        (string yazi, Color renk) = (o, d) switch
        {
            (OkumaDurumu.Okuyor, _) => ("Sesli okunuyor...", Renkler.Gul),
            (OkumaDurumu.Duraklatildi, _) => ("Okuma duraklatıldı", Renkler.Turuncu),
            (_, DinlemeDurumu.MikrofonDinleniyor) => ("Mikrofon dinleniyor...", Color.FromArgb(230, 60, 80)),
            (_, DinlemeDurumu.DosyaIsleniyor) => ("Ses dosyası işleniyor...", Renkler.Mor),
            _ => ("Hazır", Renkler.Yesil)
        };
        durumEtiketi.Text = yazi;
        durumEtiketi.Renk = renk;
        sekmeSecici.AktifNoktalar = new[] { o != OkumaDurumu.Hazir, d != DinlemeDurumu.Hazir };
        sekmeSecici.Invalidate();
    }

    private void Bildir(string mesaj, bool hata)
    {
        lblUyari.Text = (hata ? "⚠  " : "✓  ") + mesaj;
        pnlUyari.BackColor = hata ? Color.FromArgb(253, 232, 236) : Color.FromArgb(226, 246, 235);
        lblUyari.ForeColor = hata ? Color.FromArgb(150, 24, 60) : Color.FromArgb(20, 110, 60);
        // Uzun mesaj kesilmesin: şerit yüksekliği metne göre
        int genislik = Math.Max(200, lblUyari.Width);
        int yukseklik = TextRenderer.MeasureText(lblUyari.Text, lblUyari.Font, new Size(genislik, 0),
            TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height;
        pnlUyari.Height = Math.Max(40, yukseklik + 18);
        pnlUyari.Visible = true;
        PerformLayout();
        // Başarı mesajları birkaç saniye sonra kendiliğinden kapanıyor; hatalar kullanıcı kapatana kadar kalıyor
        bildirimZamanlayici.Stop();
        if (!hata)
            bildirimZamanlayici.Start();
    }

    private void UyariGizle()
    {
        pnlUyari.Visible = false;
        PerformLayout();
    }

    private void btnUyariKapat_Click(object? sender, EventArgs e) => UyariGizle();

    private void pnlUst_Paint(object? sender, PaintEventArgs e)
    {
        using var b = new LinearGradientBrush(pnlUst.ClientRectangle, Renkler.Gul, Renkler.Mor, 0f);
        e.Graphics.FillRectangle(b, pnlUst.ClientRectangle);

        // sağda dekoratif ses dalgası
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var kalem = new Pen(Color.FromArgb(50, 255, 255, 255), 3) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        for (int i = 0; i < 28; i++)
        {
            float x = pnlUst.Width - 560 + i * 10;
            float h = 8 + 22 * (float)Math.Abs(Math.Sin(i * 0.55));
            e.Graphics.DrawLine(kalem, x, 40 - h / 2, x, 40 + h / 2);
        }
    }

    private void AnaForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            bildirimZamanlayici.Dispose();
            okuma?.Durdur();
            dinleme?.Durdur();
            okuyucu?.Dispose();
            tanima?.Dispose();
        }
        catch (Exception)
        {
            // kapanırken oluşan hatalar önemli değil
        }
    }
}
