using System.Text;
using SpeechApp.Servisler;

namespace SpeechApp.Model;

public enum OkumaDurumu
{
    Hazir,
    Okuyor,
    Duraklatildi
}

public enum DinlemeDurumu
{
    Hazir,
    MikrofonDinleniyor,
    DosyaIsleniyor
}

/// <summary>
/// Metin okuma işinin durumunu yönetir (Hazır → Okuyor ⇄ Duraklatıldı → Hazır).
/// Form sadece butonları buna bağlıyor; asıl okuma IMetinOkuyucu'da.
/// </summary>
public sealed class OkumaYoneticisi
{
    private readonly IMetinOkuyucu okuyucu;
    private string metin = "";

    public OkumaYoneticisi(IMetinOkuyucu okuyucu)
    {
        this.okuyucu = okuyucu;
        okuyucu.KelimeOkunuyor += Okuyucu_KelimeOkunuyor;
        okuyucu.OkumaBitti += (_, _) =>
        {
            Ilerleme = Durum == OkumaDurumu.Okuyor ? 100 : Ilerleme;
            DurumuDegistir(OkumaDurumu.Hazir);
        };
        okuyucu.OkumaHatasi += (_, mesaj) =>
        {
            DurumuDegistir(OkumaDurumu.Hazir);
            HataOlustu?.Invoke(this, mesaj);
        };
    }

    public event EventHandler<string>? HataOlustu;

    public OkumaDurumu Durum { get; private set; } = OkumaDurumu.Hazir;

    /// <summary>Okunan kısmın yüzdesi (0-100).</summary>
    public int Ilerleme { get; private set; }

    public event EventHandler? DurumDegisti;
    public event EventHandler<KelimeEventArgs>? KelimeVurgula;

    public void Basla(string? yeniMetin)
    {
        if (string.IsNullOrWhiteSpace(yeniMetin))
            throw new KonusmaHatasi("Okunacak metin yok. Önce kutuya bir şeyler yazın.");

        if (Durum != OkumaDurumu.Hazir)
            okuyucu.Durdur();

        metin = yeniMetin;
        Ilerleme = 0;
        okuyucu.OkumayaBasla(metin);
        DurumuDegistir(OkumaDurumu.Okuyor);
    }

    public void DuraklatVeyaDevamEt()
    {
        switch (Durum)
        {
            case OkumaDurumu.Okuyor:
                okuyucu.Duraklat();
                DurumuDegistir(OkumaDurumu.Duraklatildi);
                break;
            case OkumaDurumu.Duraklatildi:
                okuyucu.DevamEt();
                DurumuDegistir(OkumaDurumu.Okuyor);
                break;
        }
    }

    public void Durdur()
    {
        if (Durum == OkumaDurumu.Hazir)
            return;
        okuyucu.Durdur();
        DurumuDegistir(OkumaDurumu.Hazir);
    }

    private void Okuyucu_KelimeOkunuyor(object? sender, KelimeEventArgs e)
    {
        if (metin.Length > 0)
            Ilerleme = Math.Clamp((int)((e.Konum + e.Uzunluk) * 100L / metin.Length), 0, 100);
        KelimeVurgula?.Invoke(this, e);
    }

    private void DurumuDegistir(OkumaDurumu yeni)
    {
        if (Durum == yeni)
            return;
        Durum = yeni;
        DurumDegisti?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>Tanınan cümlelerin zaman damgalı listesi.</summary>
public sealed class TanimaGecmisi
{
    private readonly List<TanimaSonucu> sonuclar = new();

    public IReadOnlyList<TanimaSonucu> Sonuclar => sonuclar;

    public void Ekle(TanimaSonucu sonuc)
    {
        if (!string.IsNullOrWhiteSpace(sonuc.Metin))
            sonuclar.Add(sonuc);
    }

    public void Temizle() => sonuclar.Clear();

    /// <summary>Tüm cümleler, cümle başı büyük harfle ve noktayla birleştirilmiş hali.</summary>
    public string TamMetin()
    {
        var sb = new StringBuilder();
        foreach (var s in sonuclar)
        {
            string c = s.Metin.Trim();
            if (c.Length == 0) continue;
            c = char.ToUpper(c[0]) + c[1..];
            if (!".!?".Contains(c[^1])) c += ".";
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(c);
        }
        return sb.ToString();
    }

    public void Kaydet(string yol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(yol);
        using var yazici = new StreamWriter(yol, false, new UTF8Encoding(true));
        yazici.WriteLine($"# Konuşma tanıma sonuçları - {DateTime.Now:dd.MM.yyyy HH:mm}");
        yazici.WriteLine();
        foreach (var s in sonuclar)
            yazici.WriteLine($"[{s.Zaman:HH:mm:ss}] ({s.Guven:P0}) {s.Metin}");
        yazici.WriteLine();
        yazici.WriteLine("# Tam metin:");
        yazici.WriteLine(TamMetin());
    }
}

/// <summary>Dinleme (konuşma tanıma) işinin durumunu ve sonuç listesini yönetir.</summary>
public sealed class DinlemeYoneticisi
{
    private readonly IKonusmaTanima tanima;

    public DinlemeYoneticisi(IKonusmaTanima tanima)
    {
        this.tanima = tanima;
        tanima.Tanindi += (_, s) =>
        {
            Gecmis.Ekle(s);
            SonucEklendi?.Invoke(this, s);
        };
        tanima.Bitti += (_, _) => DurumuDegistir(DinlemeDurumu.Hazir);
    }

    public DinlemeDurumu Durum { get; private set; } = DinlemeDurumu.Hazir;
    public TanimaGecmisi Gecmis { get; } = new();

    public event EventHandler? DurumDegisti;
    public event EventHandler<TanimaSonucu>? SonucEklendi;

    public void MikrofonuBaslat()
    {
        if (Durum != DinlemeDurumu.Hazir)
            Durdur();

        tanima.MikrofondanDinle();       // hata olursa KonusmaHatasi fırlatır, durum değişmez
        DurumuDegistir(DinlemeDurumu.MikrofonDinleniyor);
    }

    public void DosyadanBaslat(string? yol)
    {
        if (string.IsNullOrWhiteSpace(yol) || !File.Exists(yol))
            throw new KonusmaHatasi("Ses dosyası bulunamadı.");
        if (!yol.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            throw new KonusmaHatasi("Sadece .wav dosyaları destekleniyor.");

        if (Durum != DinlemeDurumu.Hazir)
            Durdur();

        tanima.DosyadanTani(yol);
        DurumuDegistir(DinlemeDurumu.DosyaIsleniyor);
    }

    public void Durdur()
    {
        if (Durum == DinlemeDurumu.Hazir)
            return;
        tanima.Durdur();
        DurumuDegistir(DinlemeDurumu.Hazir);
    }

    private void DurumuDegistir(DinlemeDurumu yeni)
    {
        if (Durum == yeni)
            return;
        Durum = yeni;
        DurumDegisti?.Invoke(this, EventArgs.Empty);
    }
}
