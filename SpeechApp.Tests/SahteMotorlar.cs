using SpeechApp.Servisler;

namespace SpeechApp.Tests;

/// <summary>Gerçek ses çıkarmayan, çağrıları kaydeden sahte okuyucu.</summary>
public sealed class SahteOkuyucu : IMetinOkuyucu
{
    public List<string> Cagrilar { get; } = new();
    public IReadOnlyList<SesBilgisi> Sesler { get; set; } = new[] { new SesBilgisi("Test Sesi", "tr-TR", "Female") };
    public string? SeciliSes { get; set; }
    public int Hiz { get; set; }
    public int SesDuzeyi { get; set; } = 100;

    public void OkumayaBasla(string metin) => Cagrilar.Add("basla:" + metin);
    public void Duraklat() => Cagrilar.Add("duraklat");
    public void DevamEt() => Cagrilar.Add("devam");
    public void Durdur() => Cagrilar.Add("durdur");
    public void DosyayaKaydet(string metin, string wavYolu) => File.WriteAllText(wavYolu, metin);

    public void KelimeGonder(int konum, int uzunluk) => KelimeOkunuyor?.Invoke(this, new KelimeEventArgs(konum, uzunluk));
    public void BitirGonder() => OkumaBitti?.Invoke(this, EventArgs.Empty);
    public void HataGonder(string m) => OkumaHatasi?.Invoke(this, m);

    public event EventHandler<KelimeEventArgs>? KelimeOkunuyor;
    public event EventHandler? OkumaBitti;
    public event EventHandler<string>? OkumaHatasi;
    public void Dispose() { }
}

/// <summary>Mikrofon yerine elle "konuşma" gönderilebilen sahte tanıyıcı.</summary>
public sealed class SahteTanima : IKonusmaTanima
{
    public bool MikrofonVar { get; set; } = true;
    public List<string> Cagrilar { get; } = new();
    public IReadOnlyList<string> Diller { get; set; } = new[] { "tr-TR" };
    public string? SeciliDil { get; set; }

    public void MikrofondanDinle()
    {
        if (!MikrofonVar)
            throw new KonusmaHatasi("Mikrofon bulunamadı.");
        Cagrilar.Add("mikrofon");
    }

    public void DosyadanTani(string wavYolu) => Cagrilar.Add("dosya:" + Path.GetFileName(wavYolu));
    public void Durdur() => Cagrilar.Add("durdur");

    public void Soyle(string metin, float guven = 0.9f) => Tanindi?.Invoke(this, new TanimaSonucu(DateTime.Now, metin, guven));
    public void BitirGonder() => Bitti?.Invoke(this, EventArgs.Empty);

    public event EventHandler<TanimaSonucu>? Tanindi;
    public event EventHandler<string>? TahminDegisti;
    public event EventHandler<int>? SesSeviyesi;
    public event EventHandler? Bitti;

    public void TahminGonder(string t) => TahminDegisti?.Invoke(this, t);
    public void SeviyeGonder(int s) => SesSeviyesi?.Invoke(this, s);
    public void Dispose() { }
}
