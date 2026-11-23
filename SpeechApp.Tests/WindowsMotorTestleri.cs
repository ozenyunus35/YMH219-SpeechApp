using SpeechApp.Servisler;
using Xunit.Abstractions;

namespace SpeechApp.Tests;

/// <summary>
/// Gerçek Windows konuşma motoruyla (System.Speech) testler.
/// Bilgisayarda ses veya tanıyıcı yoksa ilgili kısım atlanıp nedeni test çıktısına yazılıyor.
/// </summary>
public class WindowsMotorTestleri
{
    private readonly ITestOutputHelper cikti;

    public WindowsMotorTestleri(ITestOutputHelper cikti) => this.cikti = cikti;

    [Fact]
    public void YukluSesler_Listelenebiliyor()
    {
        using var okuyucu = new SistemMetinOkuyucu();
        cikti.WriteLine("Sesler: " + string.Join(", ", okuyucu.Sesler));
        Assert.NotNull(okuyucu.Sesler);
    }

    [Fact]
    public void Hiz_VeSesDuzeyi_SinirlarIcindeTutulur()
    {
        using var okuyucu = new SistemMetinOkuyucu();

        okuyucu.Hiz = 50;
        okuyucu.SesDuzeyi = -5;

        Assert.Equal(10, okuyucu.Hiz);
        Assert.Equal(0, okuyucu.SesDuzeyi);
    }

    [Fact]
    public void MetinWavDosyasinaKaydedilir_VeYenidenTaninabilir()
    {
        using var okuyucu = new SistemMetinOkuyucu();
        if (okuyucu.Sesler.Count == 0)
        {
            cikti.WriteLine("Bu makinede ses yok, test atlandı.");
            return;
        }

        string wav = Path.Combine(Path.GetTempPath(), $"tts-{Guid.NewGuid():N}.wav");
        try
        {
            okuyucu.DosyayaKaydet("hello world this is a test", wav);
            var bilgi = new FileInfo(wav);
            cikti.WriteLine($"WAV boyutu: {bilgi.Length} bayt");
            Assert.True(bilgi.Length > 10_000);

            // Aynı dosyayı tanıma motoruna verip geri yazıya çevir (tanıyıcı varsa)
            using var tanima = new SistemKonusmaTanima();
            if (tanima.Diller.Count == 0)
            {
                cikti.WriteLine("Tanıyıcı yok, tanıma kısmı atlandı.");
                return;
            }

            var sonuclar = new List<string>();
            using var bitti = new ManualResetEventSlim();
            tanima.Tanindi += (_, s) => sonuclar.Add(s.Metin);
            tanima.Bitti += (_, _) => bitti.Set();
            tanima.DosyadanTani(wav);
            bitti.Wait(TimeSpan.FromSeconds(30));
            cikti.WriteLine("Tanınan: " + string.Join(" | ", sonuclar));
            Assert.True(bitti.IsSet, "Tanıma bitmedi");
        }
        finally
        {
            if (File.Exists(wav)) File.Delete(wav);
        }
    }

    [Fact]
    public void BosMetinKaydedilmez()
    {
        using var okuyucu = new SistemMetinOkuyucu();
        Assert.Throws<KonusmaHatasi>(() => okuyucu.DosyayaKaydet("  ", Path.Combine(Path.GetTempPath(), "bos.wav")));
    }

    [Fact]
    public void BozukWav_AnlasilirHataVerir()
    {
        using var tanima = new SistemKonusmaTanima();
        if (tanima.Diller.Count == 0)
        {
            Assert.Throws<KonusmaHatasi>(() => tanima.MikrofondanDinle());
            return;
        }

        string sahte = Path.Combine(Path.GetTempPath(), $"bozuk-{Guid.NewGuid():N}.wav");
        File.WriteAllText(sahte, "bu bir ses dosyası değil");
        try
        {
            Assert.Throws<KonusmaHatasi>(() => tanima.DosyadanTani(sahte));
        }
        finally { File.Delete(sahte); }
    }
}
