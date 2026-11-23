using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SpeechApp.Kontroller;

/// <summary>Uygulamanın renkleri ve ortak çizim yardımcıları.</summary>
public static class Renkler
{
    public static readonly Color Gul = Color.FromArgb(214, 51, 108);
    public static readonly Color GulKoyu = Color.FromArgb(170, 32, 84);
    public static readonly Color Mor = Color.FromArgb(124, 58, 237);
    public static readonly Color Zemin = Color.FromArgb(250, 245, 248);
    public static readonly Color Kart = Color.White;
    public static readonly Color Cizgi = Color.FromArgb(236, 222, 230);
    public static readonly Color Yazi = Color.FromArgb(42, 24, 40);
    public static readonly Color Soluk = Color.FromArgb(128, 108, 122);
    public static readonly Color Yesil = Color.FromArgb(22, 150, 90);
    public static readonly Color Turuncu = Color.FromArgb(214, 130, 20);

    public static GraphicsPath Yuvarlak(RectangleF r, float yaricap)
    {
        float d = Math.Max(1, Math.Min(yaricap * 2, Math.Min(r.Width, r.Height)));
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}

public enum StudyoButonTuru
{
    Ana,        // gül renkli dolgu
    Ikincil,    // beyaz, çerçeveli
    Sade        // sadece yazı
}

/// <summary>Hap şeklinde buton.</summary>
public class StudyoButonu : Button
{
    private StudyoButonTuru tur = StudyoButonTuru.Ikincil;
    private bool uzerinde, basili;

    public StudyoButonu()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = new Font("Segoe UI Semibold", 9.75F);
        Cursor = Cursors.Hand;
    }

    protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
    protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

    [Category("Görünüm")]
    [DefaultValue(StudyoButonTuru.Ikincil)]
    public StudyoButonTuru Tur { get => tur; set { tur = value; Invalidate(); } }

    protected override void OnMouseEnter(EventArgs e) { uzerinde = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { uzerinde = basili = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { basili = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { basili = false; Invalidate(); base.OnMouseUp(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Renkler.Zemin);

        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using var yol = Renkler.Yuvarlak(r, r.Height / 2);

        Color yazi;
        if (!Enabled)
        {
            using var b = new SolidBrush(Color.FromArgb(242, 236, 240));
            g.FillPath(b, yol);
            yazi = Color.FromArgb(190, 176, 186);
        }
        else if (tur == StudyoButonTuru.Ana)
        {
            float k = basili ? 0.85f : uzerinde ? 0.93f : 1f;
            using var b = new LinearGradientBrush(r, Carp(Renkler.Gul, k), Carp(Renkler.Mor, k), 0f);
            g.FillPath(b, yol);
            yazi = Color.White;
        }
        else if (tur == StudyoButonTuru.Ikincil)
        {
            using var b = new SolidBrush(basili ? Color.FromArgb(246, 234, 241) : uzerinde ? Color.FromArgb(252, 244, 248) : Color.White);
            using var p = new Pen(Renkler.Cizgi);
            g.FillPath(b, yol);
            g.DrawPath(p, yol);
            yazi = Renkler.Yazi;
        }
        else
        {
            if (uzerinde)
                using (var b = new SolidBrush(Color.FromArgb(20, Renkler.Gul))) g.FillPath(b, yol);
            yazi = Renkler.GulKoyu;
        }

        TextRenderer.DrawText(g, Text, Font, ClientRectangle, yazi,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

        // Klavyeyle (Tab) gelindiğinde odak çerçevesi
        if (Focused && ShowFocusCues)
        {
            using var odak = new Pen(Renkler.Mor, 2f) { DashStyle = DashStyle.Dot };
            using var ic = Renkler.Yuvarlak(RectangleF.Inflate(r, -3, -3), (r.Height - 6) / 2);
            g.DrawPath(odak, ic);
        }
    }

    private static Color Carp(Color c, float k) => Color.FromArgb(c.A, (int)(c.R * k), (int)(c.G * k), (int)(c.B * k));
}

/// <summary>Başlıklı beyaz kart.</summary>
public class StudyoKarti : Panel
{
    private string baslik = "";
    private readonly Font baslikFontu = new("Segoe UI Semibold", 10.5F);

    public StudyoKarti()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Renkler.Kart;
        Padding = new Padding(16, 44, 16, 14);
    }

    [Category("Görünüm")]
    [DefaultValue("")]
    public string Baslik { get => baslik; set { baslik = value; Invalidate(); } }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Renkler.Zemin);
        using (var yol = Renkler.Yuvarlak(new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f), 14))
        using (var b = new SolidBrush(BackColor))
        using (var p = new Pen(Renkler.Cizgi))
        {
            g.FillPath(b, yol);
            g.DrawPath(p, yol);
        }
        if (baslik.Length > 0)
            TextRenderer.DrawText(g, baslik, baslikFontu, new Point(16, 14), Renkler.Yazi);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) baslikFontu.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>
/// Ses dalgası göstergesi. Aktifken çubuklar hareket ediyor;
/// Seviye (0-100) verilirse çubukların boyu ona göre ayarlanıyor.
/// </summary>
public class SesDalgasi : Control
{
    private const int CubukSayisi = 32;
    private readonly float[] boylar = new float[CubukSayisi];
    private readonly System.Windows.Forms.Timer zamanlayici = new() { Interval = 50 };
    private readonly Random rastgele = new(7);
    private bool aktif;
    private int seviye = -1;
    private double faz;

    public SesDalgasi()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, false);
        zamanlayici.Tick += (_, _) => Ilerle();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Aktif
    {
        get => aktif;
        set
        {
            aktif = value;
            zamanlayici.Enabled = value;
            if (!value) { Array.Clear(boylar); seviye = -1; }
            Invalidate();
        }
    }

    /// <summary>Gerçek ses seviyesi (mikrofon). -1 ise yapay dalga çiziliyor.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Seviye { get => seviye; set => seviye = Math.Clamp(value, -1, 100); }

    private void Ilerle()
    {
        faz += 0.35;
        for (int i = 0; i < CubukSayisi; i++)
        {
            float hedef;
            if (seviye >= 0)
            {
                float merkezeYakinlik = 1f - Math.Abs(i - CubukSayisi / 2f) / (CubukSayisi / 2f);
                hedef = seviye / 100f * (0.35f + 0.65f * merkezeYakinlik) * (0.7f + 0.3f * (float)rastgele.NextDouble());
            }
            else
            {
                hedef = 0.25f + 0.55f * (float)Math.Abs(Math.Sin(faz + i * 0.45)) * (0.6f + 0.4f * (float)rastgele.NextDouble());
            }
            boylar[i] += (hedef - boylar[i]) * 0.45f;    // yumuşak geçiş
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Color.White);

        float aralik = Width / (float)CubukSayisi;
        float gen = Math.Max(2, aralik * 0.55f);
        float orta = Height / 2f;

        for (int i = 0; i < CubukSayisi; i++)
        {
            float h = Math.Max(3, boylar[i] * (Height - 4));
            var r = new RectangleF(i * aralik + (aralik - gen) / 2, orta - h / 2, gen, h);
            float t = i / (float)(CubukSayisi - 1);
            Color c = aktif
                ? Color.FromArgb(255, (int)(Renkler.Gul.R + (Renkler.Mor.R - Renkler.Gul.R) * t), (int)(Renkler.Gul.G + (Renkler.Mor.G - Renkler.Gul.G) * t), (int)(Renkler.Gul.B + (Renkler.Mor.B - Renkler.Gul.B) * t))
                : Color.FromArgb(226, 214, 222);
            using var yol = Renkler.Yuvarlak(r, gen / 2);
            using var b = new SolidBrush(c);
            g.FillPath(b, yol);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) zamanlayici.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>Büyük, yuvarlak mikrofon butonu. Dinlerken etrafında halkalar dalgalanıyor.</summary>
public class MikrofonButonu : Button
{
    private bool dinliyor;
    private bool uzerinde;
    private readonly System.Windows.Forms.Timer zamanlayici = new() { Interval = 40 };
    private float nabiz;

    public MikrofonButonu()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        zamanlayici.Tick += (_, _) => { nabiz = (nabiz + 0.03f) % 1f; Invalidate(); };
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Dinliyor
    {
        get => dinliyor;
        set { dinliyor = value; zamanlayici.Enabled = value; nabiz = 0; Invalidate(); }
    }

    protected override void OnMouseEnter(EventArgs e) { uzerinde = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { uzerinde = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
    protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Color.White);
        if (Focused && ShowFocusCues)
        {
            float rr = Math.Min(Width, Height) * 0.30f + 7;
            using var odak = new Pen(Renkler.Mor, 2f) { DashStyle = DashStyle.Dot };
            g.DrawEllipse(odak, Width / 2f - rr, Height / 2f - rr, rr * 2, rr * 2);
        }

        float cap = Math.Min(Width, Height);
        float cx = Width / 2f, cy = Height / 2f;
        float r = cap * 0.30f;

        if (dinliyor)
        {
            for (int i = 0; i < 2; i++)
            {
                float t = (nabiz + i * 0.5f) % 1f;
                float rr = r + t * (cap / 2f - r);
                using var p = new Pen(Color.FromArgb((int)(140 * (1 - t)), Renkler.Gul), 3);
                g.DrawEllipse(p, cx - rr, cy - rr, rr * 2, rr * 2);
            }
        }

        var daire = new RectangleF(cx - r, cy - r, r * 2, r * 2);
        if (Enabled)
        {
            using var b = new LinearGradientBrush(daire, dinliyor ? Color.FromArgb(230, 60, 80) : Renkler.Gul, dinliyor ? Color.FromArgb(200, 30, 60) : Renkler.Mor, 45f);
            g.FillEllipse(b, daire);
            if (uzerinde)
                using (var h = new Pen(Color.FromArgb(80, Renkler.Gul), 4)) g.DrawEllipse(h, RectangleF.Inflate(daire, 4, 4));
        }
        else
        {
            using var b = new SolidBrush(Color.FromArgb(226, 214, 222));
            g.FillEllipse(b, daire);
        }

        // Dinlerken kare (durdur) ikonu, değilken mikrofon ikonu
        using var beyaz = new SolidBrush(Color.White);
        using var kalem = new Pen(Color.White, r * 0.09f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        if (dinliyor)
        {
            float s = r * 0.55f;
            using var yol = Renkler.Yuvarlak(new RectangleF(cx - s / 2, cy - s / 2, s, s), s * 0.15f);
            g.FillPath(beyaz, yol);
        }
        else
        {
            float w = r * 0.42f, h = r * 0.75f;
            using (var govde = Renkler.Yuvarlak(new RectangleF(cx - w / 2, cy - h * 0.75f, w, h), w / 2))
                g.FillPath(beyaz, govde);
            g.DrawArc(kalem, cx - w * 0.95f, cy - h * 0.55f, w * 1.9f, h * 0.95f, 0, 180);
            g.DrawLine(kalem, cx, cy + h * 0.42f, cx, cy + h * 0.62f);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) zamanlayici.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>İki seçenekli sekme çubuğu (Oku / Dinle).</summary>
public class SekmeSecici : Control
{
    private string[] sekmeler = { "Sekme 1", "Sekme 2" };
    private int secili;
    private readonly Font font = new("Segoe UI Semibold", 10F);

    public event EventHandler? SecimDegisti;

    public SekmeSecici()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, false);
        Cursor = Cursors.Hand;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] Sekmeler { get => sekmeler; set { sekmeler = value; Invalidate(); } }

    /// <summary>Sekme başlığının yanında küçük nokta (o sekmede bir işlem sürüyorsa).</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool[] AktifNoktalar { get; set; } = new bool[2];

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Secili
    {
        get => secili;
        set
        {
            if (value == secili || value < 0 || value >= sekmeler.Length) return;
            secili = value;
            Invalidate();
            SecimDegisti?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Secili = Math.Clamp(e.X * sekmeler.Length / Math.Max(1, Width), 0, sekmeler.Length - 1);
        base.OnMouseDown(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Renkler.Zemin);

        var tum = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using (var yol = Renkler.Yuvarlak(tum, tum.Height / 2))
        using (var b = new SolidBrush(Color.FromArgb(240, 228, 236)))
            g.FillPath(b, yol);

        float w = (Width - 6) / (float)sekmeler.Length;
        for (int i = 0; i < sekmeler.Length; i++)
        {
            var r = new RectangleF(3 + i * w, 3, w, Height - 6);
            if (i == secili)
            {
                using var yol = Renkler.Yuvarlak(r, r.Height / 2);
                using var b = new SolidBrush(Color.White);
                using var golge = new SolidBrush(Color.FromArgb(25, 80, 20, 60));
                g.TranslateTransform(0, 1);
                g.FillPath(golge, yol);
                g.TranslateTransform(0, -1);
                g.FillPath(b, yol);
            }
            TextRenderer.DrawText(g, sekmeler[i], font, Rectangle.Round(r), i == secili ? Renkler.GulKoyu : Renkler.Soluk,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            if (i < AktifNoktalar.Length && AktifNoktalar[i])
            {
                Size s = TextRenderer.MeasureText(sekmeler[i], font);
                float nx = r.X + r.Width / 2 + s.Width / 2f + 6;
                using var nokta = new SolidBrush(Color.FromArgb(230, 60, 80));
                g.FillEllipse(nokta, nx, r.Y + r.Height / 2 - 4, 8, 8);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) font.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>Durum etiketi: renkli nokta + yazı (Hazır / Okuyor / Dinleniyor...).</summary>
public class DurumEtiketi : Control
{
    private Color renk = Renkler.Yesil;
    private readonly Font font = new("Segoe UI Semibold", 9F);

    public DurumEtiketi()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.Selectable, false);
        BackColor = Color.Transparent;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Renk { get => renk; set { renk = value; Invalidate(); } }

    protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using (var yol = Renkler.Yuvarlak(r, r.Height / 2))
        using (var b = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            g.FillPath(b, yol);
        using (var n = new SolidBrush(renk))
            g.FillEllipse(n, 12, Height / 2f - 4, 8, 8);
        TextRenderer.DrawText(g, Text, font, new Rectangle(26, 0, Width - 30, Height), Renkler.Yazi,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) font.Dispose();
        base.Dispose(disposing);
    }
}
