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
    private FileStream? dosyaAkisi;     // dosyadan tanırken açık tuttuğumuz .wav

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
        var m = MotorOlustur();
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
        var m = MotorOlustur();

        // Dosyayı kendimiz açıyoruz: SetInputToWaveFile hata verince dosyayı kilitli bırakıyordu.
        FileStream? akis = null;
        try
        {
            akis = new FileStream(wavYolu, FileMode.Open, FileAccess.Read, FileShare.Read);
            m.SetInputToWaveStream(akis);
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or FormatException or InvalidOperationException or UnauthorizedAccessException)
        {
            akis?.Dispose();
            m.Dispose();
            throw new KonusmaHatasi("Ses dosyası okunamadı. Dosyanın PCM .wav formatında olduğundan emin olun.", ex);
        }

        dosyaAkisi = akis;
        Baslat(m);
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

    private SpeechRecognitionEngine MotorOlustur()
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
            DosyayiKapat();
            _ = Task.Run(m.Dispose);
            if (aktifti)
                Bitti?.Invoke(this, EventArgs.Empty);
        };
        return m;
    }

    private void DosyayiKapat()
    {
        dosyaAkisi?.Dispose();
        dosyaAkisi = null;
    }

    private void Baslat(SpeechRecognitionEngine m)
    {
        motor = m;
        m.RecognizeAsync(RecognizeMode.Multiple);
    }

    public event EventHandler<TanimaSonucu>? Tanindi;
    public event EventHandler<string>? TahminDegisti;
    public event EventHandler<int>? SesSeviyesi;
    public event EventHandler? Bitti;

    public void Dispose()
    {
        Durdur();
        DosyayiKapat();
    }
}
