using System.Globalization;
using System.Text;
using SpeechApp.Model;
using SpeechApp.Servisler;

namespace SpeechApp.Tests;

/// <summary>Kod incelemesinde bulunan hataların geri gelmemesi için eklenen testler.</summary>
public class SaglamlikTestleri
{
    [Fact]
    public void TamMetin_TurkceWindowsta_IngilizceIHarfiBozulmaz()
    {
        var eski = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            var g = new TanimaGecmisi();
            g.Ekle(new TanimaSonucu(DateTime.Now, "ice cream is good", 0.9f));
            Assert.Equal("Ice cream is good.", g.TamMetin());
        }
        finally
        {
            CultureInfo.CurrentCulture = eski;
        }
    }

    [Fact]
    public void Gecmis_BaskaThreaddenEklenirken_OkumakHataVermez()
    {
        var g = new TanimaGecmisi();
        // Motor thread'i gibi arka planda sonuç ekleyen yazıcı; ekran thread'i aynı anda okuyor
        var yazan = Task.Run(() =>
        {
            for (int i = 0; i < 3000; i++)
                g.Ekle(new TanimaSonucu(DateTime.Now, "merhaba", 1f));
        });
        while (!yazan.IsCompleted)
        {
            _ = g.TamMetin();
            _ = g.Sonuclar.Count;
        }
        yazan.Wait();
        Assert.Equal(3000, g.Sonuclar.Count);
    }

    [Fact]
    public void Durdurduktan_SonraGelenGecKelimeOlayi_YoksayilirVeIlerlemeSifirlanir()
    {
        var o = new SahteOkuyucu();
        var y = new OkumaYoneticisi(o);
        int vurgu = 0;
        y.KelimeVurgula += (_, _) => vurgu++;

        y.Basla("bir iki üç dört");
        o.KelimeGonder(0, 3);
        y.Durdur();
        o.KelimeGonder(4, 3);

        Assert.Equal(1, vurgu);
        Assert.Equal(0, y.Ilerleme);
    }

    [Fact]
    public void SonKelimedeElleDurdurmak_TamamlandiGostermez()
    {
        var o = new SahteOkuyucu();
        var y = new OkumaYoneticisi(o);
        y.Basla("abcd");
        o.KelimeGonder(2, 2);
        Assert.Equal(100, y.Ilerleme);

        y.Durdur();
        Assert.True(y.Ilerleme < 100);
    }

    [Fact]
    public void DuraklatildigiAnda_BitenOkuma_YuzdeYuzOlur()
    {
        var o = new SahteOkuyucu();
        var y = new OkumaYoneticisi(o);
        y.Basla("abcdefghij");
        o.KelimeGonder(0, 2);
        y.DuraklatVeyaDevamEt();
        o.BitirGonder();

        Assert.Equal(OkumaDurumu.Hazir, y.Durum);
        Assert.Equal(100, y.Ilerleme);
    }

    [Fact]
    public void OkurkenYeniOkumaBaslayamazsa_OkuyorDaTakiliKalmaz()
    {
        var o = new SahteOkuyucu();
        var y = new OkumaYoneticisi(o);
        y.Basla("ilk metin");
        o.HataVer = true;

        Assert.Throws<KonusmaHatasi>(() => y.Basla("ikinci metin"));
        Assert.Equal(OkumaDurumu.Hazir, y.Durum);
    }
}
