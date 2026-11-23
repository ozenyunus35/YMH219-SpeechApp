namespace SpeechApp;

partial class AnaForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlUst = new Panel();
        durumEtiketi = new SpeechApp.Kontroller.DurumEtiketi();
        lblAltBaslik = new Label();
        lblBaslik = new Label();
        pnlSekme = new Panel();
        sekmeSecici = new SpeechApp.Kontroller.SekmeSecici();
        pnlUyari = new Panel();
        btnUyariKapat = new Button();
        lblUyari = new Label();
        pnlOku = new Panel();
        btnTemizle = new SpeechApp.Kontroller.StudyoButonu();
        btnDurdur = new SpeechApp.Kontroller.StudyoButonu();
        btnDuraklat = new SpeechApp.Kontroller.StudyoButonu();
        btnOku = new SpeechApp.Kontroller.StudyoButonu();
        dalgaOku = new SpeechApp.Kontroller.SesDalgasi();
        lblIlerleme = new Label();
        prgOku = new ProgressBar();
        kartAyar = new SpeechApp.Kontroller.StudyoKarti();
        btnMetinKaydet = new SpeechApp.Kontroller.StudyoButonu();
        btnMetinAc = new SpeechApp.Kontroller.StudyoButonu();
        btnWavKaydet = new SpeechApp.Kontroller.StudyoButonu();
        lblSesDuzeyiDeger = new Label();
        trkSesDuzeyi = new TrackBar();
        lblSesDuzeyi = new Label();
        lblHizDeger = new Label();
        trkHiz = new TrackBar();
        lblHiz = new Label();
        cmbSes = new ComboBox();
        lblSes = new Label();
        kartMetin = new SpeechApp.Kontroller.StudyoKarti();
        rtbMetin = new RichTextBox();
        lblKarakter = new Label();
        pnlDinle = new Panel();
        btnYaziKaydet = new SpeechApp.Kontroller.StudyoButonu();
        btnYaziTemizle = new SpeechApp.Kontroller.StudyoButonu();
        kartListe = new SpeechApp.Kontroller.StudyoKarti();
        lvSonuclar = new ListView();
        colSaat = new ColumnHeader();
        colMetin = new ColumnHeader();
        colGuven = new ColumnHeader();
        kartYazi = new SpeechApp.Kontroller.StudyoKarti();
        rtbYazi = new RichTextBox();
        lblTahmin = new Label();
        kartMikrofon = new SpeechApp.Kontroller.StudyoKarti();
        lblDilIpucu = new Label();
        btnDosyadanTani = new SpeechApp.Kontroller.StudyoButonu();
        cmbDil = new ComboBox();
        lblDil = new Label();
        dalgaDinle = new SpeechApp.Kontroller.SesDalgasi();
        lblMikDurum = new Label();
        btnMikrofon = new SpeechApp.Kontroller.MikrofonButonu();
        durumCubugu = new StatusStrip();
        lblMotor = new ToolStripStatusLabel();
        ipucu = new ToolTip(components);
        pnlUst.SuspendLayout();
        pnlSekme.SuspendLayout();
        pnlUyari.SuspendLayout();
        pnlOku.SuspendLayout();
        kartAyar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)trkSesDuzeyi).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trkHiz).BeginInit();
        kartMetin.SuspendLayout();
        pnlDinle.SuspendLayout();
        kartListe.SuspendLayout();
        kartYazi.SuspendLayout();
        kartMikrofon.SuspendLayout();
        durumCubugu.SuspendLayout();
        SuspendLayout();
        //
        // pnlUst
        //
        pnlUst.Controls.Add(durumEtiketi);
        pnlUst.Controls.Add(lblAltBaslik);
        pnlUst.Controls.Add(lblBaslik);
        pnlUst.Dock = DockStyle.Top;
        pnlUst.Location = new Point(0, 0);
        pnlUst.Name = "pnlUst";
        pnlUst.Size = new Size(1040, 80);
        pnlUst.TabIndex = 0;
        pnlUst.Paint += pnlUst_Paint;
        //
        // durumEtiketi
        //
        durumEtiketi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        durumEtiketi.Location = new Point(830, 24);
        durumEtiketi.Name = "durumEtiketi";
        durumEtiketi.Size = new Size(190, 32);
        durumEtiketi.TabIndex = 2;
        durumEtiketi.Text = "Hazır";
        //
        // lblAltBaslik
        //
        lblAltBaslik.AutoSize = true;
        lblAltBaslik.BackColor = Color.Transparent;
        lblAltBaslik.ForeColor = Color.FromArgb(255, 228, 240);
        lblAltBaslik.Location = new Point(26, 48);
        lblAltBaslik.Name = "lblAltBaslik";
        lblAltBaslik.Size = new Size(220, 15);
        lblAltBaslik.TabIndex = 1;
        lblAltBaslik.Text = "Metin ↔ Konuşma  ·  YMH219  ·  System.Speech";
        //
        // lblBaslik
        //
        lblBaslik.AutoSize = true;
        lblBaslik.BackColor = Color.Transparent;
        lblBaslik.Font = new Font("Segoe UI Semibold", 18F);
        lblBaslik.ForeColor = Color.White;
        lblBaslik.Location = new Point(22, 12);
        lblBaslik.Name = "lblBaslik";
        lblBaslik.Size = new Size(150, 32);
        lblBaslik.TabIndex = 0;
        lblBaslik.Text = "Ses Stüdyosu";
        //
        // pnlSekme
        //
        pnlSekme.BackColor = Color.FromArgb(250, 245, 248);
        pnlSekme.Controls.Add(sekmeSecici);
        pnlSekme.Dock = DockStyle.Top;
        pnlSekme.Location = new Point(0, 80);
        pnlSekme.Name = "pnlSekme";
        pnlSekme.Size = new Size(1040, 58);
        pnlSekme.TabIndex = 1;
        //
        // sekmeSecici
        //
        sekmeSecici.Location = new Point(24, 12);
        sekmeSecici.Name = "sekmeSecici";
        sekmeSecici.Size = new Size(440, 38);
        sekmeSecici.TabIndex = 0;
        sekmeSecici.SecimDegisti += sekmeSecici_SecimDegisti;
        //
        // pnlUyari
        //
        pnlUyari.BackColor = Color.FromArgb(253, 232, 236);
        pnlUyari.Controls.Add(btnUyariKapat);
        pnlUyari.Controls.Add(lblUyari);
        pnlUyari.Dock = DockStyle.Top;
        pnlUyari.Location = new Point(0, 138);
        pnlUyari.Name = "pnlUyari";
        pnlUyari.Size = new Size(1040, 40);
        pnlUyari.TabIndex = 2;
        pnlUyari.Visible = false;
        //
        // btnUyariKapat
        //
        btnUyariKapat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnUyariKapat.FlatAppearance.BorderSize = 0;
        btnUyariKapat.FlatStyle = FlatStyle.Flat;
        btnUyariKapat.ForeColor = Color.FromArgb(170, 32, 84);
        btnUyariKapat.Location = new Point(1000, 6);
        btnUyariKapat.Name = "btnUyariKapat";
        btnUyariKapat.Size = new Size(30, 28);
        btnUyariKapat.TabIndex = 1;
        btnUyariKapat.TabStop = false;
        btnUyariKapat.Text = "✕";
        btnUyariKapat.Click += btnUyariKapat_Click;
        //
        // lblUyari
        //
        lblUyari.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblUyari.Font = new Font("Segoe UI Semibold", 9F);
        lblUyari.ForeColor = Color.FromArgb(150, 24, 60);
        lblUyari.Location = new Point(24, 0);
        lblUyari.Name = "lblUyari";
        lblUyari.Size = new Size(960, 40);
        lblUyari.TabIndex = 0;
        lblUyari.TextAlign = ContentAlignment.MiddleLeft;
        //
        // pnlOku
        //
        pnlOku.BackColor = Color.FromArgb(250, 245, 248);
        pnlOku.Controls.Add(btnTemizle);
        pnlOku.Controls.Add(btnDurdur);
        pnlOku.Controls.Add(btnDuraklat);
        pnlOku.Controls.Add(btnOku);
        pnlOku.Controls.Add(dalgaOku);
        pnlOku.Controls.Add(lblIlerleme);
        pnlOku.Controls.Add(prgOku);
        pnlOku.Controls.Add(kartAyar);
        pnlOku.Controls.Add(kartMetin);
        pnlOku.Dock = DockStyle.Fill;
        pnlOku.Location = new Point(0, 178);
        pnlOku.Name = "pnlOku";
        pnlOku.Size = new Size(1040, 460);
        pnlOku.TabIndex = 3;
        //
        // btnTemizle
        //
        btnTemizle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnTemizle.Location = new Point(410, 406);
        btnTemizle.Name = "btnTemizle";
        btnTemizle.Size = new Size(100, 40);
        btnTemizle.TabIndex = 8;
        btnTemizle.Text = "Temizle";
        btnTemizle.Tur = SpeechApp.Kontroller.StudyoButonTuru.Sade;
        btnTemizle.Click += btnTemizle_Click;
        //
        // btnDurdur
        //
        btnDurdur.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDurdur.Enabled = false;
        btnDurdur.Location = new Point(294, 406);
        btnDurdur.Name = "btnDurdur";
        btnDurdur.Size = new Size(110, 40);
        btnDurdur.TabIndex = 7;
        btnDurdur.Text = "■  Durdur";
        btnDurdur.Click += btnDurdur_Click;
        //
        // btnDuraklat
        //
        btnDuraklat.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDuraklat.Enabled = false;
        btnDuraklat.Location = new Point(166, 406);
        btnDuraklat.Name = "btnDuraklat";
        btnDuraklat.Size = new Size(122, 40);
        btnDuraklat.TabIndex = 6;
        btnDuraklat.Text = "❚❚  Duraklat";
        btnDuraklat.Click += btnDuraklat_Click;
        //
        // btnOku
        //
        btnOku.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnOku.Location = new Point(24, 406);
        btnOku.Name = "btnOku";
        btnOku.Size = new Size(136, 40);
        btnOku.TabIndex = 5;
        btnOku.Text = "▶  Sesli oku";
        btnOku.Tur = SpeechApp.Kontroller.StudyoButonTuru.Ana;
        btnOku.Click += btnOku_Click;
        //
        // dalgaOku
        //
        dalgaOku.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dalgaOku.Location = new Point(530, 404);
        dalgaOku.Name = "dalgaOku";
        dalgaOku.Size = new Size(300, 44);
        dalgaOku.TabIndex = 9;
        //
        // lblIlerleme
        //
        lblIlerleme.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblIlerleme.Font = new Font("Segoe UI Semibold", 9F);
        lblIlerleme.ForeColor = Color.FromArgb(128, 108, 122);
        lblIlerleme.Location = new Point(846, 404);
        lblIlerleme.Name = "lblIlerleme";
        lblIlerleme.Size = new Size(170, 20);
        lblIlerleme.TabIndex = 10;
        lblIlerleme.Text = "Okunan: %0";
        lblIlerleme.TextAlign = ContentAlignment.MiddleRight;
        //
        // prgOku
        //
        prgOku.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        prgOku.Location = new Point(846, 428);
        prgOku.Name = "prgOku";
        prgOku.Size = new Size(170, 12);
        prgOku.TabIndex = 11;
        //
        // kartAyar
        //
        kartAyar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        kartAyar.Baslik = "Ses ayarları";
        kartAyar.Controls.Add(btnMetinKaydet);
        kartAyar.Controls.Add(btnMetinAc);
        kartAyar.Controls.Add(btnWavKaydet);
        kartAyar.Controls.Add(lblSesDuzeyiDeger);
        kartAyar.Controls.Add(trkSesDuzeyi);
        kartAyar.Controls.Add(lblSesDuzeyi);
        kartAyar.Controls.Add(lblHizDeger);
        kartAyar.Controls.Add(trkHiz);
        kartAyar.Controls.Add(lblHiz);
        kartAyar.Controls.Add(cmbSes);
        kartAyar.Controls.Add(lblSes);
        kartAyar.Location = new Point(694, 8);
        kartAyar.Name = "kartAyar";
        kartAyar.Size = new Size(322, 384);
        kartAyar.TabIndex = 1;
        //
        // btnMetinKaydet
        //
        btnMetinKaydet.Location = new Point(16, 334);
        btnMetinKaydet.Name = "btnMetinKaydet";
        btnMetinKaydet.Size = new Size(290, 34);
        btnMetinKaydet.TabIndex = 10;
        btnMetinKaydet.Text = "Metni .txt olarak kaydet";
        btnMetinKaydet.Click += btnMetinKaydet_Click;
        //
        // btnMetinAc
        //
        btnMetinAc.Location = new Point(16, 294);
        btnMetinAc.Name = "btnMetinAc";
        btnMetinAc.Size = new Size(290, 34);
        btnMetinAc.TabIndex = 9;
        btnMetinAc.Text = "Metin dosyası aç (.txt)";
        btnMetinAc.Click += btnMetinAc_Click;
        //
        // btnWavKaydet
        //
        btnWavKaydet.Location = new Point(16, 254);
        btnWavKaydet.Name = "btnWavKaydet";
        btnWavKaydet.Size = new Size(290, 34);
        btnWavKaydet.TabIndex = 8;
        btnWavKaydet.Text = "Sesi .wav dosyasına kaydet";
        btnWavKaydet.Click += btnWavKaydet_Click;
        //
        // lblSesDuzeyiDeger
        //
        lblSesDuzeyiDeger.Font = new Font("Segoe UI Semibold", 9F);
        lblSesDuzeyiDeger.ForeColor = Color.FromArgb(170, 32, 84);
        lblSesDuzeyiDeger.Location = new Point(226, 170);
        lblSesDuzeyiDeger.Name = "lblSesDuzeyiDeger";
        lblSesDuzeyiDeger.Size = new Size(80, 18);
        lblSesDuzeyiDeger.TabIndex = 7;
        lblSesDuzeyiDeger.Text = "%100";
        lblSesDuzeyiDeger.TextAlign = ContentAlignment.MiddleRight;
        //
        // trkSesDuzeyi
        //
        trkSesDuzeyi.AutoSize = false;
        trkSesDuzeyi.BackColor = Color.White;
        trkSesDuzeyi.LargeChange = 10;
        trkSesDuzeyi.Location = new Point(12, 192);
        trkSesDuzeyi.Maximum = 100;
        trkSesDuzeyi.Name = "trkSesDuzeyi";
        trkSesDuzeyi.Size = new Size(298, 32);
        trkSesDuzeyi.SmallChange = 5;
        trkSesDuzeyi.TabIndex = 6;
        trkSesDuzeyi.TabStop = false;
        trkSesDuzeyi.TickFrequency = 10;
        trkSesDuzeyi.Value = 100;
        trkSesDuzeyi.ValueChanged += trkSesDuzeyi_ValueChanged;
        //
        // lblSesDuzeyi
        //
        lblSesDuzeyi.AutoSize = true;
        lblSesDuzeyi.ForeColor = Color.FromArgb(128, 108, 122);
        lblSesDuzeyi.Location = new Point(16, 172);
        lblSesDuzeyi.Name = "lblSesDuzeyi";
        lblSesDuzeyi.Size = new Size(64, 15);
        lblSesDuzeyi.TabIndex = 5;
        lblSesDuzeyi.Text = "Ses düzeyi";
        //
        // lblHizDeger
        //
        lblHizDeger.Font = new Font("Segoe UI Semibold", 9F);
        lblHizDeger.ForeColor = Color.FromArgb(170, 32, 84);
        lblHizDeger.Location = new Point(226, 104);
        lblHizDeger.Name = "lblHizDeger";
        lblHizDeger.Size = new Size(80, 18);
        lblHizDeger.TabIndex = 4;
        lblHizDeger.Text = "normal";
        lblHizDeger.TextAlign = ContentAlignment.MiddleRight;
        //
        // trkHiz
        //
        trkHiz.AutoSize = false;
        trkHiz.BackColor = Color.White;
        trkHiz.Location = new Point(12, 126);
        trkHiz.Maximum = 10;
        trkHiz.Minimum = -10;
        trkHiz.Name = "trkHiz";
        trkHiz.Size = new Size(298, 32);
        trkHiz.TabIndex = 3;
        trkHiz.TabStop = false;
        trkHiz.TickFrequency = 2;
        trkHiz.ValueChanged += trkHiz_ValueChanged;
        //
        // lblHiz
        //
        lblHiz.AutoSize = true;
        lblHiz.ForeColor = Color.FromArgb(128, 108, 122);
        lblHiz.Location = new Point(16, 106);
        lblHiz.Name = "lblHiz";
        lblHiz.Size = new Size(64, 15);
        lblHiz.TabIndex = 2;
        lblHiz.Text = "Okuma hızı";
        //
        // cmbSes
        //
        cmbSes.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSes.FlatStyle = FlatStyle.Flat;
        cmbSes.Font = new Font("Segoe UI", 9.75F);
        cmbSes.Location = new Point(16, 66);
        cmbSes.Name = "cmbSes";
        cmbSes.Size = new Size(290, 25);
        cmbSes.TabIndex = 1;
        cmbSes.SelectedIndexChanged += cmbSes_SelectedIndexChanged;
        //
        // lblSes
        //
        lblSes.AutoSize = true;
        lblSes.ForeColor = Color.FromArgb(128, 108, 122);
        lblSes.Location = new Point(16, 46);
        lblSes.Name = "lblSes";
        lblSes.Size = new Size(24, 15);
        lblSes.TabIndex = 0;
        lblSes.Text = "Ses";
        //
        // kartMetin
        //
        kartMetin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        kartMetin.Baslik = "Okunacak metin";
        kartMetin.Controls.Add(rtbMetin);
        kartMetin.Controls.Add(lblKarakter);
        kartMetin.Location = new Point(24, 8);
        kartMetin.Name = "kartMetin";
        kartMetin.Size = new Size(654, 384);
        kartMetin.TabIndex = 0;
        //
        // rtbMetin
        //
        rtbMetin.BorderStyle = BorderStyle.None;
        rtbMetin.DetectUrls = false;
        rtbMetin.Dock = DockStyle.Fill;
        rtbMetin.Font = new Font("Segoe UI", 12F);
        rtbMetin.ForeColor = Color.FromArgb(42, 24, 40);
        rtbMetin.Name = "rtbMetin";
        rtbMetin.TabIndex = 0;
        rtbMetin.Text = "";
        rtbMetin.TextChanged += rtbMetin_TextChanged;
        //
        // lblKarakter
        //
        lblKarakter.Dock = DockStyle.Bottom;
        lblKarakter.ForeColor = Color.FromArgb(128, 108, 122);
        lblKarakter.Name = "lblKarakter";
        lblKarakter.Size = new Size(622, 22);
        lblKarakter.TabIndex = 1;
        lblKarakter.Text = "0 karakter";
        lblKarakter.TextAlign = ContentAlignment.BottomRight;
        //
        // pnlDinle
        //
        pnlDinle.BackColor = Color.FromArgb(250, 245, 248);
        pnlDinle.Controls.Add(btnYaziKaydet);
        pnlDinle.Controls.Add(btnYaziTemizle);
        pnlDinle.Controls.Add(kartListe);
        pnlDinle.Controls.Add(kartYazi);
        pnlDinle.Controls.Add(kartMikrofon);
        pnlDinle.Dock = DockStyle.Fill;
        pnlDinle.Location = new Point(0, 178);
        pnlDinle.Name = "pnlDinle";
        pnlDinle.Size = new Size(1040, 460);
        pnlDinle.TabIndex = 4;
        pnlDinle.Visible = false;
        //
        // btnYaziKaydet
        //
        btnYaziKaydet.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnYaziKaydet.Location = new Point(830, 408);
        btnYaziKaydet.Name = "btnYaziKaydet";
        btnYaziKaydet.Size = new Size(186, 38);
        btnYaziKaydet.TabIndex = 4;
        btnYaziKaydet.Text = "Sonuçları .txt kaydet";
        btnYaziKaydet.Click += btnYaziKaydet_Click;
        //
        // btnYaziTemizle
        //
        btnYaziTemizle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnYaziTemizle.Location = new Point(720, 408);
        btnYaziTemizle.Name = "btnYaziTemizle";
        btnYaziTemizle.Size = new Size(100, 38);
        btnYaziTemizle.TabIndex = 3;
        btnYaziTemizle.Text = "Temizle";
        btnYaziTemizle.Tur = SpeechApp.Kontroller.StudyoButonTuru.Sade;
        btnYaziTemizle.Click += btnYaziTemizle_Click;
        //
        // kartListe
        //
        kartListe.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        kartListe.Baslik = "Zaman damgalı sonuçlar";
        kartListe.Controls.Add(lvSonuclar);
        kartListe.Location = new Point(344, 236);
        kartListe.Name = "kartListe";
        kartListe.Size = new Size(672, 162);
        kartListe.TabIndex = 2;
        //
        // lvSonuclar
        //
        lvSonuclar.BorderStyle = BorderStyle.None;
        lvSonuclar.Columns.AddRange(new ColumnHeader[] { colSaat, colMetin, colGuven });
        lvSonuclar.Dock = DockStyle.Fill;
        lvSonuclar.FullRowSelect = true;
        lvSonuclar.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvSonuclar.Name = "lvSonuclar";
        lvSonuclar.TabIndex = 0;
        lvSonuclar.UseCompatibleStateImageBehavior = false;
        lvSonuclar.View = View.Details;
        //
        // colSaat
        //
        colSaat.Text = "Saat";
        colSaat.Width = 80;
        //
        // colMetin
        //
        colMetin.Text = "Tanınan metin";
        colMetin.Width = 440;
        //
        // colGuven
        //
        colGuven.Text = "Güven";
        colGuven.TextAlign = HorizontalAlignment.Right;
        colGuven.Width = 70;
        //
        // kartYazi
        //
        kartYazi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        kartYazi.Baslik = "Yazıya dökülen metin";
        kartYazi.Controls.Add(rtbYazi);
        kartYazi.Controls.Add(lblTahmin);
        kartYazi.Location = new Point(344, 8);
        kartYazi.Name = "kartYazi";
        kartYazi.Size = new Size(672, 218);
        kartYazi.TabIndex = 1;
        //
        // rtbYazi
        //
        rtbYazi.BorderStyle = BorderStyle.None;
        rtbYazi.Dock = DockStyle.Fill;
        rtbYazi.Font = new Font("Segoe UI", 12F);
        rtbYazi.ForeColor = Color.FromArgb(42, 24, 40);
        rtbYazi.Name = "rtbYazi";
        rtbYazi.TabIndex = 0;
        rtbYazi.Text = "";
        //
        // lblTahmin
        //
        lblTahmin.Dock = DockStyle.Bottom;
        lblTahmin.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblTahmin.ForeColor = Color.FromArgb(128, 108, 122);
        lblTahmin.Name = "lblTahmin";
        lblTahmin.Size = new Size(640, 22);
        lblTahmin.TabIndex = 1;
        lblTahmin.TextAlign = ContentAlignment.MiddleLeft;
        //
        // kartMikrofon
        //
        kartMikrofon.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        kartMikrofon.Baslik = "Mikrofon";
        kartMikrofon.Controls.Add(lblDilIpucu);
        kartMikrofon.Controls.Add(btnDosyadanTani);
        kartMikrofon.Controls.Add(cmbDil);
        kartMikrofon.Controls.Add(lblDil);
        kartMikrofon.Controls.Add(dalgaDinle);
        kartMikrofon.Controls.Add(lblMikDurum);
        kartMikrofon.Controls.Add(btnMikrofon);
        kartMikrofon.Location = new Point(24, 8);
        kartMikrofon.Name = "kartMikrofon";
        kartMikrofon.Size = new Size(304, 438);
        kartMikrofon.TabIndex = 0;
        //
        // lblDilIpucu
        //
        lblDilIpucu.Font = new Font("Segoe UI", 8.25F);
        lblDilIpucu.ForeColor = Color.FromArgb(128, 108, 122);
        lblDilIpucu.Location = new Point(16, 330);
        lblDilIpucu.Name = "lblDilIpucu";
        lblDilIpucu.Size = new Size(272, 50);
        lblDilIpucu.TabIndex = 5;
        lblDilIpucu.Text = "Not: Windows'ta Türkçe konuşma tanıyıcı bulunmuyor; yüklü dilde (genelde İngilizce) konuşun.";
        //
        // btnDosyadanTani
        //
        btnDosyadanTani.Location = new Point(16, 386);
        btnDosyadanTani.Name = "btnDosyadanTani";
        btnDosyadanTani.Size = new Size(272, 36);
        btnDosyadanTani.TabIndex = 6;
        btnDosyadanTani.Text = "Mikrofon yerine .wav dosyasından tanı";
        btnDosyadanTani.Click += btnDosyadanTani_Click;
        //
        // cmbDil
        //
        cmbDil.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDil.FlatStyle = FlatStyle.Flat;
        cmbDil.Font = new Font("Segoe UI", 9.75F);
        cmbDil.Location = new Point(16, 298);
        cmbDil.Name = "cmbDil";
        cmbDil.Size = new Size(272, 25);
        cmbDil.TabIndex = 4;
        cmbDil.SelectedIndexChanged += cmbDil_SelectedIndexChanged;
        //
        // lblDil
        //
        lblDil.AutoSize = true;
        lblDil.ForeColor = Color.FromArgb(128, 108, 122);
        lblDil.Location = new Point(16, 278);
        lblDil.Name = "lblDil";
        lblDil.Size = new Size(76, 15);
        lblDil.TabIndex = 3;
        lblDil.Text = "Tanıma dili";
        //
        // dalgaDinle
        //
        dalgaDinle.Location = new Point(28, 230);
        dalgaDinle.Name = "dalgaDinle";
        dalgaDinle.Size = new Size(248, 36);
        dalgaDinle.TabIndex = 2;
        //
        // lblMikDurum
        //
        lblMikDurum.Font = new Font("Segoe UI Semibold", 10F);
        lblMikDurum.ForeColor = Color.FromArgb(42, 24, 40);
        lblMikDurum.Location = new Point(16, 200);
        lblMikDurum.Name = "lblMikDurum";
        lblMikDurum.Size = new Size(272, 24);
        lblMikDurum.TabIndex = 1;
        lblMikDurum.Text = "Dinlemek için mikrofona tıklayın";
        lblMikDurum.TextAlign = ContentAlignment.MiddleCenter;
        //
        // btnMikrofon
        //
        btnMikrofon.Location = new Point(62, 40);
        btnMikrofon.Name = "btnMikrofon";
        btnMikrofon.Size = new Size(180, 160);
        btnMikrofon.TabIndex = 0;
        btnMikrofon.Text = "Dinle";
        btnMikrofon.Click += btnMikrofon_Click;
        //
        // durumCubugu
        //
        durumCubugu.BackColor = Color.White;
        durumCubugu.Items.AddRange(new ToolStripItem[] { lblMotor });
        durumCubugu.Name = "durumCubugu";
        durumCubugu.SizingGrip = false;
        durumCubugu.TabIndex = 5;
        //
        // lblMotor
        //
        lblMotor.ForeColor = Color.FromArgb(128, 108, 122);
        lblMotor.Name = "lblMotor";
        lblMotor.Size = new Size(200, 17);
        lblMotor.Text = "Konuşma motoru yükleniyor...";
        //
        // AnaForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(250, 245, 248);
        ClientSize = new Size(1040, 660);
        Controls.Add(pnlOku);
        Controls.Add(pnlDinle);
        Controls.Add(pnlUyari);
        Controls.Add(pnlSekme);
        Controls.Add(pnlUst);
        Controls.Add(durumCubugu);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(960, 620);
        Name = "AnaForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Ses Stüdyosu - YMH219";
        FormClosing += AnaForm_FormClosing;
        Load += AnaForm_Load;
        pnlUst.ResumeLayout(false);
        pnlUst.PerformLayout();
        pnlSekme.ResumeLayout(false);
        pnlUyari.ResumeLayout(false);
        pnlOku.ResumeLayout(false);
        kartAyar.ResumeLayout(false);
        kartAyar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)trkSesDuzeyi).EndInit();
        ((System.ComponentModel.ISupportInitialize)trkHiz).EndInit();
        kartMetin.ResumeLayout(false);
        pnlDinle.ResumeLayout(false);
        kartListe.ResumeLayout(false);
        kartYazi.ResumeLayout(false);
        kartMikrofon.ResumeLayout(false);
        kartMikrofon.PerformLayout();
        durumCubugu.ResumeLayout(false);
        durumCubugu.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel pnlUst;
    private Label lblBaslik;
    private Label lblAltBaslik;
    private SpeechApp.Kontroller.DurumEtiketi durumEtiketi;
    private Panel pnlSekme;
    private SpeechApp.Kontroller.SekmeSecici sekmeSecici;
    private Panel pnlUyari;
    private Label lblUyari;
    private Button btnUyariKapat;
    private Panel pnlOku;
    private SpeechApp.Kontroller.StudyoKarti kartMetin;
    private RichTextBox rtbMetin;
    private Label lblKarakter;
    private SpeechApp.Kontroller.StudyoKarti kartAyar;
    private Label lblSes;
    private ComboBox cmbSes;
    private Label lblHiz;
    private TrackBar trkHiz;
    private Label lblHizDeger;
    private Label lblSesDuzeyi;
    private TrackBar trkSesDuzeyi;
    private Label lblSesDuzeyiDeger;
    private SpeechApp.Kontroller.StudyoButonu btnWavKaydet;
    private SpeechApp.Kontroller.StudyoButonu btnMetinAc;
    private SpeechApp.Kontroller.StudyoButonu btnMetinKaydet;
    private SpeechApp.Kontroller.StudyoButonu btnOku;
    private SpeechApp.Kontroller.StudyoButonu btnDuraklat;
    private SpeechApp.Kontroller.StudyoButonu btnDurdur;
    private SpeechApp.Kontroller.StudyoButonu btnTemizle;
    private SpeechApp.Kontroller.SesDalgasi dalgaOku;
    private Label lblIlerleme;
    private ProgressBar prgOku;
    private Panel pnlDinle;
    private SpeechApp.Kontroller.StudyoKarti kartMikrofon;
    private SpeechApp.Kontroller.MikrofonButonu btnMikrofon;
    private Label lblMikDurum;
    private SpeechApp.Kontroller.SesDalgasi dalgaDinle;
    private Label lblDil;
    private ComboBox cmbDil;
    private Label lblDilIpucu;
    private SpeechApp.Kontroller.StudyoButonu btnDosyadanTani;
    private SpeechApp.Kontroller.StudyoKarti kartYazi;
    private RichTextBox rtbYazi;
    private Label lblTahmin;
    private SpeechApp.Kontroller.StudyoKarti kartListe;
    private ListView lvSonuclar;
    private ColumnHeader colSaat;
    private ColumnHeader colMetin;
    private ColumnHeader colGuven;
    private SpeechApp.Kontroller.StudyoButonu btnYaziTemizle;
    private SpeechApp.Kontroller.StudyoButonu btnYaziKaydet;
    private StatusStrip durumCubugu;
    private ToolStripStatusLabel lblMotor;
    private ToolTip ipucu;
}
