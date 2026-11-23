using SpeechApp.Model;
using SpeechApp.Servisler;

namespace SpeechApp.Tests;

public class OkumaYoneticisiTestleri
{
    private readonly SahteOkuyucu okuyucu = new();
    private readonly OkumaYoneticisi yonetici;

    public OkumaYoneticisiTestleri() => yonetici = new OkumaYoneticisi(okuyucu);

    [Fact]
    public void Basla_MetniOkuyucuyaVerir_VeDurumOkuyorOlur()
    {
        yonetici.Basla("Merhaba dünya");

        Assert.Equal(OkumaDurumu.Okuyor, yonetici.Durum);
        Assert.Equal("basla:Merhaba dünya", okuyucu.Cagrilar.Last());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   \n ")]
    public void BosMetin_HataVerir_VeDurumDegismez(string? metin)
    {
        var ex = Assert.Throws<KonusmaHatasi>(() => yonetici.Basla(metin));

        Assert.Contains("metin yok", ex.Message);
        Assert.Equal(OkumaDurumu.Hazir, yonetici.Durum);
        Assert.Empty(okuyucu.Cagrilar);
    }

    [Fact]
    public void Duraklat_SonraDevam()
    {
        yonetici.Basla("bir iki üç");

        yonetici.DuraklatVeyaDevamEt();
        Assert.Equal(OkumaDurumu.Duraklatildi, yonetici.Durum);
        Assert.Equal("duraklat", okuyucu.Cagrilar.Last());

        yonetici.DuraklatVeyaDevamEt();
        Assert.Equal(OkumaDurumu.Okuyor, yonetici.Durum);
        Assert.Equal("devam", okuyucu.Cagrilar.Last());
    }

    [Fact]
    public void HazirdayDuraklat_BirSeyYapmaz()
    {
        yonetici.DuraklatVeyaDevamEt();
        yonetici.Durdur();

        Assert.Equal(OkumaDurumu.Hazir, yonetici.Durum);
        Assert.Empty(okuyucu.Cagrilar);
    }

    [Fact]
    public void Durdur_DuraklatilmisOkumayiDaBitirir()
    {
        yonetici.Basla("metin");
        yonetici.DuraklatVeyaDevamEt();

        yonetici.Durdur();

        Assert.Equal(OkumaDurumu.Hazir, yonetici.Durum);
        Assert.Equal("durdur", okuyucu.Cagrilar.Last());
    }

    [Fact]
    public void OkurkenYeniMetin_OnceEskisiniDurdurur()
    {
        yonetici.Basla("birinci");
        yonetici.Basla("ikinci");

        Assert.Equal(new[] { "basla:birinci", "durdur", "basla:ikinci" }, okuyucu.Cagrilar);
        Assert.Equal(OkumaDurumu.Okuyor, yonetici.Durum);
    }

    [Fact]
    public void KelimeOlaylari_IlerlemeyiHesaplar_VeVurguOlayiGelir()
    {
        string metin = "Bir iki üç dört";   // 15 karakter
        int vurgu = 0;
        yonetici.KelimeVurgula += (_, _) => vurgu++;
        yonetici.Basla(metin);

        okuyucu.KelimeGonder(0, 3);     // "Bir"
        Assert.Equal(20, yonetici.Ilerleme);

        okuyucu.KelimeGonder(11, 4);    // "dört"
        Assert.Equal(100, yonetici.Ilerleme);
        Assert.Equal(2, vurgu);
    }

    [Fact]
    public void OkumaBitince_HazirDurumunaDoner_VeOlayGelir()
    {
        int degisim = 0;
        yonetici.DurumDegisti += (_, _) => degisim++;
        yonetici.Basla("kısa");

        okuyucu.BitirGonder();

        Assert.Equal(OkumaDurumu.Hazir, yonetici.Durum);
        Assert.Equal(100, yonetici.Ilerleme);
        Assert.Equal(2, degisim);      // Hazır->Okuyor, Okuyor->Hazır
    }
}

public class OkumaHatasiTestleri
{
    [Fact]
    public void MotorHatasi_HazirDurumunaDoner_VeHataOlayiGelir()
    {
        var okuyucu = new SahteOkuyucu();
        var yonetici = new OkumaYoneticisi(okuyucu);
        string? hata = null;
        yonetici.HataOlustu += (_, m) => hata = m;
        yonetici.Basla("metin");

        okuyucu.HataGonder("ses aygıtı yok");

        Assert.Equal(OkumaDurumu.Hazir, yonetici.Durum);
        Assert.Equal("ses aygıtı yok", hata);
    }
}

public class DinlemeYoneticisiTestleri
{
    private readonly SahteTanima tanima = new();
    private readonly DinlemeYoneticisi yonetici;

    public DinlemeYoneticisiTestleri() => yonetici = new DinlemeYoneticisi(tanima);

    [Fact]
    public void MikrofonuBaslat_DurumDinliyorOlur_SonuclarGecmiseEklenir()
    {
        var gelen = new List<string>();
        yonetici.SonucEklendi += (_, s) => gelen.Add(s.Metin);

        yonetici.MikrofonuBaslat();
        tanima.Soyle("merhaba");
        tanima.Soyle("nasılsın");

        Assert.Equal(DinlemeDurumu.MikrofonDinleniyor, yonetici.Durum);
        Assert.Equal(new[] { "merhaba", "nasılsın" }, gelen);
        Assert.Equal(2, yonetici.Gecmis.Sonuclar.Count);
    }

    [Fact]
    public void MikrofonYoksa_HataFirlar_DurumHazirKalir()
    {
        tanima.MikrofonVar = false;

        Assert.Throws<KonusmaHatasi>(() => yonetici.MikrofonuBaslat());
        Assert.Equal(DinlemeDurumu.Hazir, yonetici.Durum);
    }

    [Fact]
    public void Durdur_HazirDurumunaDoner()
    {
        yonetici.MikrofonuBaslat();

        yonetici.Durdur();

        Assert.Equal(DinlemeDurumu.Hazir, yonetici.Durum);
        Assert.Equal("durdur", tanima.Cagrilar.Last());
    }

    [Fact]
    public void OlmayanDosya_VeyaWavOlmayan_Reddedilir()
    {
        Assert.Throws<KonusmaHatasi>(() => yonetici.DosyadanBaslat(@"C:\yok\yok.wav"));
        Assert.Throws<KonusmaHatasi>(() => yonetici.DosyadanBaslat(null));

        string txt = Path.Combine(Path.GetTempPath(), $"deneme-{Guid.NewGuid():N}.txt");
        File.WriteAllText(txt, "x");
        try
        {
            var ex = Assert.Throws<KonusmaHatasi>(() => yonetici.DosyadanBaslat(txt));
            Assert.Contains(".wav", ex.Message);
        }
        finally { File.Delete(txt); }
    }

    [Fact]
    public void DosyadanTanima_DosyaBitinceHazirOlur()
    {
        string wav = Path.Combine(Path.GetTempPath(), $"deneme-{Guid.NewGuid():N}.wav");
        File.WriteAllText(wav, "x");
        try
        {
            yonetici.DosyadanBaslat(wav);
            Assert.Equal(DinlemeDurumu.DosyaIsleniyor, yonetici.Durum);

            tanima.Soyle("dosyadaki cümle");
            tanima.BitirGonder();

            Assert.Equal(DinlemeDurumu.Hazir, yonetici.Durum);
            Assert.Single(yonetici.Gecmis.Sonuclar);
        }
        finally { File.Delete(wav); }
    }

    [Fact]
    public void BosSonuclar_GecmiseEklenmez()
    {
        yonetici.MikrofonuBaslat();
        tanima.Soyle("   ");

        Assert.Empty(yonetici.Gecmis.Sonuclar);
    }
}

public class TanimaGecmisiTestleri
{
    [Fact]
    public void TamMetin_CumleleriBuyukHarfVeNoktaylaBirlestirir()
    {
        var g = new TanimaGecmisi();
        g.Ekle(new TanimaSonucu(DateTime.Now, "merhaba arkadaşlar", 0.9f));
        g.Ekle(new TanimaSonucu(DateTime.Now, "bugün hava güzel mi?", 0.8f));
        g.Ekle(new TanimaSonucu(DateTime.Now, "evet", 0.7f));

        Assert.Equal("Merhaba arkadaşlar. Bugün hava güzel mi? Evet.", g.TamMetin());
    }

    [Fact]
    public void Kaydet_ZamanDamgasiVeGuvenYazar()
    {
        var g = new TanimaGecmisi();
        g.Ekle(new TanimaSonucu(new DateTime(2026, 11, 25, 14, 3, 9), "hello world", 0.87f));
        string yol = Path.Combine(Path.GetTempPath(), $"tanima-{Guid.NewGuid():N}.txt");
        try
        {
            g.Kaydet(yol);
            string icerik = File.ReadAllText(yol);

            Assert.Contains("[14:03:09]", icerik);
            Assert.Contains("hello world", icerik);
            Assert.Contains("Hello world.", icerik);
        }
        finally { File.Delete(yol); }
    }

    [Fact]
    public void Temizle_HepsiniSiler()
    {
        var g = new TanimaGecmisi();
        g.Ekle(new TanimaSonucu(DateTime.Now, "bir", 1));
        g.Temizle();

        Assert.Empty(g.Sonuclar);
        Assert.Equal("", g.TamMetin());
    }
}
