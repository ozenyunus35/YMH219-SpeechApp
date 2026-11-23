using System.Speech.Synthesis;

namespace SpeechApp.Servisler;

/// <summary>
/// Windows'un yerleşik konuşma sentezleyicisi (SAPI) ile metin okuma.
/// </summary>
public sealed class SistemMetinOkuyucu : IMetinOkuyucu
{
    private readonly SpeechSynthesizer sentez;
    private Prompt? aktifOkuma;

    public SistemMetinOkuyucu()
    {
        try
        {
            sentez = new SpeechSynthesizer();
            sentez.SetOutputToDefaultAudioDevice();
        }
        catch (Exception ex) when (ex is InvalidOperationException or PlatformNotSupportedException or System.ComponentModel.Win32Exception)
        {
            throw new KonusmaHatasi("Windows konuşma motoru başlatılamadı: " + ex.Message, ex);
        }

        sentez.SpeakProgress += (_, e) => KelimeOkunuyor?.Invoke(this, new KelimeEventArgs(e.CharacterPosition, e.CharacterCount));
        sentez.SpeakCompleted += (_, e) =>
        {
            // Durdur'la iptal edilen eski okumanın "bitti" bilgisi yeni okumayı bitmiş göstermesin
            if (e.Prompt == aktifOkuma && !e.Cancelled)
                OkumaBitti?.Invoke(this, EventArgs.Empty);
        };
    }

    public IReadOnlyList<SesBilgisi> Sesler =>
        sentez.GetInstalledVoices()
              .Where(v => v.Enabled)
              .Select(v => new SesBilgisi(v.VoiceInfo.Name, v.VoiceInfo.Culture.Name, v.VoiceInfo.Gender.ToString()))
              .ToList();

    public string? SeciliSes
    {
        get => sentez.Voice?.Name;
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            try
            {
                sentez.SelectVoice(value);
            }
            catch (ArgumentException ex)
            {
                throw new KonusmaHatasi($"\"{value}\" sesi kullanılamıyor.", ex);
            }
        }
    }

    public int Hiz
    {
        get => sentez.Rate;
        set => sentez.Rate = Math.Clamp(value, -10, 10);
    }

    public int SesDuzeyi
    {
        get => sentez.Volume;
        set => sentez.Volume = Math.Clamp(value, 0, 100);
    }

    public void OkumayaBasla(string metin)
    {
        if (Sesler.Count == 0)
            throw new KonusmaHatasi("Bu bilgisayarda yüklü konuşma sesi bulunamadı. Windows Ayarlar > Saat ve dil > Konuşma bölümünden ses ekleyebilirsiniz.");

        Durdur();
        aktifOkuma = sentez.SpeakAsync(metin);
    }

    public void Duraklat()
    {
        if (sentez.State == SynthesizerState.Speaking)
            sentez.Pause();
    }

    public void DevamEt()
    {
        if (sentez.State == SynthesizerState.Paused)
            sentez.Resume();
    }

    public void Durdur()
    {
        // Duraklatılmış okumayı iptal etmeden önce devam ettirmek gerekiyor, yoksa iptal beklemede kalıyor
        if (sentez.State == SynthesizerState.Paused)
            sentez.Resume();
        aktifOkuma = null;
        sentez.SpeakAsyncCancelAll();
    }

    public void DosyayaKaydet(string metin, string wavYolu)
    {
        if (string.IsNullOrWhiteSpace(metin))
            throw new KonusmaHatasi("Kaydedilecek metin yok.");
        if (Sesler.Count == 0)
            throw new KonusmaHatasi("Bu bilgisayarda yüklü konuşma sesi bulunamadı.");

        // Ekrandaki okumayı bozmamak için ayrı bir sentezleyici
        using var ayri = new SpeechSynthesizer();
        try
        {
            if (SeciliSes != null)
                ayri.SelectVoice(SeciliSes);
            ayri.Rate = Hiz;
            ayri.Volume = SesDuzeyi;
            ayri.SetOutputToWaveFile(wavYolu);
            ayri.Speak(metin);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException)
        {
            throw new KonusmaHatasi("Ses dosyası yazılamadı: " + ex.Message, ex);
        }
        finally
        {
            ayri.SetOutputToNull();
        }
    }

    public event EventHandler<KelimeEventArgs>? KelimeOkunuyor;
    public event EventHandler? OkumaBitti;

    public void Dispose()
    {
        try { sentez.SpeakAsyncCancelAll(); } catch (Exception) { /* kapanırken önemli değil */ }
        sentez.Dispose();
    }
}
