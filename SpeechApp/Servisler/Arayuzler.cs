namespace SpeechApp.Servisler;

/// <summary>Kullanıcıya gösterilebilecek, anlaşılır mesajlı konuşma hatası.</summary>
public sealed class KonusmaHatasi : Exception
{
    public KonusmaHatasi(string mesaj, Exception? ic = null) : base(mesaj, ic) { }
}

public sealed record SesBilgisi(string Ad, string Dil, string Cinsiyet)
{
    public override string ToString() => $"{Ad}  ({Dil})";
}

public sealed class KelimeEventArgs : EventArgs
{
    public KelimeEventArgs(int konum, int uzunluk)
    {
        Konum = konum;
        Uzunluk = uzunluk;
    }

    public int Konum { get; }
    public int Uzunluk { get; }
}

/// <summary>
/// Metni sesli okuyan motor. Uygulama System.Speech kullanan gerçeğini,
/// testler ise sahte (fake) bir uygulamasını kullanıyor.
/// </summary>
public interface IMetinOkuyucu : IDisposable
{
    IReadOnlyList<SesBilgisi> Sesler { get; }
    string? SeciliSes { get; set; }

    /// <summary>-10 (yavaş) ile 10 (hızlı) arası.</summary>
    int Hiz { get; set; }

    /// <summary>0 ile 100 arası.</summary>
    int SesDuzeyi { get; set; }

    void OkumayaBasla(string metin);
    void Duraklat();
    void DevamEt();
    void Durdur();
    void DosyayaKaydet(string metin, string wavYolu);

    event EventHandler<KelimeEventArgs>? KelimeOkunuyor;
    event EventHandler? OkumaBitti;

    /// <summary>Okuma sırasında hata olursa (ör. ses çıkış aygıtı yok).</summary>
    event EventHandler<string>? OkumaHatasi;
}

public sealed record TanimaSonucu(DateTime Zaman, string Metin, float Guven);

/// <summary>Mikrofondan veya .wav dosyasından konuşmayı yazıya çeviren motor.</summary>
public interface IKonusmaTanima : IDisposable
{
    /// <summary>Bilgisayarda yüklü tanıma dilleri (ör. "en-US").</summary>
    IReadOnlyList<string> Diller { get; }
    string? SeciliDil { get; set; }

    void MikrofondanDinle();
    void DosyadanTani(string wavYolu);
    void Durdur();

    event EventHandler<TanimaSonucu>? Tanindi;
    event EventHandler<string>? TahminDegisti;
    event EventHandler<int>? SesSeviyesi;
    event EventHandler? Bitti;
}
