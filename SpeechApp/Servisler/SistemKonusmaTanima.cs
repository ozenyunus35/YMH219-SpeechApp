using System.Speech.Recognition;

namespace SpeechApp.Servisler;

/// <summary>
/// Windows'un yerleşik konuşma tanıma motoru (SAPI) ile dikte.
/// Not: Windows'ta yüklü tanıma dilleri genelde İngilizce, Almanca, Fransızca vb.;
/// Türkçe masaüstü tanıyıcı Microsoft tarafından sunulmuyor.
/// </summary>
public sealed class SistemKonusmaTanima : IKonusmaTanima
{
    private SpeechRecognitionEngine? motor;

    public IReadOnlyList<string> Diller
    {
        get
        {
            try
            {
                return SpeechRecognitionEngine.InstalledRecognizers()
                    .Select(r => r.Culture.Name)
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex) when (ex is InvalidOperationException or PlatformNotSupportedException)
            {
                return Array.Empty<string>();
            }
        }
    }

    public string? SeciliDil { get; set; }

    public void MikrofondanDinle()
    {
        Durdur();
        var m = MotorOlustur(null);
        try
        {
            m.SetInputToDefaultAudioDevice();
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.Runtime.InteropServices.COMException)
        {
            m.Dispose();
            throw new KonusmaHatasi("Mikrofon bulunamadı veya kullanılamıyor. Mikrofonun bağlı olduğundan ve " +
                                    "Windows Ayarlar > Gizlilik > Mikrofon bölümünde masaüstü uygulamalarına izin verildiğinden emin olun.", ex);
        }

        Baslat(m);
    }

    public void DosyadanTani(string wavYolu)
    {
        Durdur();

        // Dosyayı kendimiz açıyoruz: SetInputToWaveFile hata verince dosyayı kilitli bırakıyordu.
        // Akış bu motora ait; sadece bu motor bitince kapanıyor (önceki motorun geç bitişi bunu kapatmasın).
        FileStream akis;
        try
        {
            akis = new FileStream(wavYolu, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or NotSupportedException or UnauthorizedAccessException)
        {
            throw new KonusmaHatasi("Ses dosyası açılamadı: " + ex.Message, ex);
        }

        SpeechRecognitionEngine m;
        try
        {
            m = MotorOlustur(akis);
        }
        catch
        {
            akis.Dispose();
            throw;
        }

        try
        {
            m.SetInputToWaveStream(akis);
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or FormatException or InvalidOperationException or UnauthorizedAccessException)
        {
            akis.Dispose();
            m.Dispose();
            throw new KonusmaHatasi("Ses dosyası okunamadı. Dosyanın PCM .wav formatında olduğundan emin olun.", ex);
        }

        Baslat(m, akis);
    }

    public void Durdur()
    {
        var m = motor;
        motor = null;
        if (m is null)
            return;
        try
        {
            m.RecognizeAsyncCancel();
        }
        catch (InvalidOperationException)
        {
            // zaten durmuş
        }
        // motorun kendisi RecognizeCompleted'da serbest bırakılıyor
    }

    private SpeechRecognitionEngine MotorOlustur(FileStream? akis)
    {
        RecognizerInfo? bilgi;
        try
        {
            var yuklu = SpeechRecognitionEngine.InstalledRecognizers();
            bilgi = yuklu.FirstOrDefault(r => r.Culture.Name == SeciliDil) ?? yuklu.FirstOrDefault();
        }
        catch (Exception ex) when (ex is InvalidOperationException or PlatformNotSupportedException)
        {
            throw new KonusmaHatasi("Konuşma tanıma servisine erişilemedi: " + ex.Message, ex);
        }

        if (bilgi is null)
            throw new KonusmaHatasi("Bu bilgisayarda konuşma tanıma motoru yüklü değil. " +
                                    "Windows Ayarlar > Saat ve dil > Konuşma bölümünden bir konuşma dili ekleyebilirsiniz.");

        var m = new SpeechRecognitionEngine(bilgi);
        m.LoadGrammar(new DictationGrammar());

        m.SpeechRecognized += (_, e) =>
            Tanindi?.Invoke(this, new TanimaSonucu(DateTime.Now, e.Result.Text, e.Result.Confidence));
        m.SpeechHypothesized += (_, e) => TahminDegisti?.Invoke(this, e.Result.Text);
        m.AudioLevelUpdated += (_, e) => SesSeviyesi?.Invoke(this, e.AudioLevel);
        m.RecognizeCompleted += (_, _) =>
        {
            // Sadece hâlâ aktif olan motor kendiliğinden bittiyse (ör. dosyanın sonu) "Bitti" bildir.
            // Durdur() ile iptal edilen eski motorun geç gelen bildirimi yeni dinlemeyi kapatmasın.
            bool aktifti = motor == m;
            if (aktifti)
                motor = null;

            // Girişi bırak ve .wav dosyasını kapat ki dosya kilitli kalmasın
            try { m.SetInputToNull(); } catch (InvalidOperationException) { }
            try { akis?.Dispose(); } catch (IOException) { }
            _ = Task.Run(m.Dispose);
            if (aktifti)
                Bitti?.Invoke(this, EventArgs.Empty);
        };
        return m;
    }

    private void Baslat(SpeechRecognitionEngine m, FileStream? akis = null)
    {
        motor = m;
        try
        {
            m.RecognizeAsync(RecognizeMode.Multiple);
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.Runtime.InteropServices.COMException)
        {
            motor = null;
            akis?.Dispose();
            m.Dispose();
            throw new KonusmaHatasi("Konuşma tanıma başlatılamadı: " + ex.Message, ex);
        }
    }

    public event EventHandler<TanimaSonucu>? Tanindi;
    public event EventHandler<string>? TahminDegisti;
    public event EventHandler<int>? SesSeviyesi;
    public event EventHandler? Bitti;

    // Çalışan motorun dosya akışını burada kapatmıyoruz: motor hâlâ okuyor olabilir.
    // Durdur() iptal ediyor, akışı RecognizeCompleted kapatıyor.
    public void Dispose() => Durdur();
}
