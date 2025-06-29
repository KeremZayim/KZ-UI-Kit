using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustomUIKit.Controls // Bu namespace'i projenizin namespace'ine göre düzenleyin!
{
    // Progress Bar üzerinde gösterilecek metin türü
    public enum ProgressBarDisplayMode
    {
        None,         // Metin gösterme
        Percentage,   // Yüzde (%) gösterme
        ValueAndMax   // Mevcut değer / Maksimum değer (örn. 50/100) gösterme
    }

    [ToolboxItem(true)]
    [Description("Modern, sade görünümlü ve yumuşak animasyonlu bir ilerleme çubuğu.")]
    public class KZ_ProgressBar : UserControl
    {
        // --- Özel Alanlar ---
        private float _currentValue = 0; // Animasyon için mevcut çizim değeri
        private int _targetValue = 0;    // Value özelliği tarafından belirlenen hedef değer
        private int _minimum = 0;
        private int _maximum = 100;

        // Renk ayarları
        private Color _progressBarColor = Color.FromArgb(90, 80, 210); // MediumSlateBlue'ya yakın, daha derin bir mor-mavi
        private Color _barBackgroundColor = Color.FromArgb(235, 235, 235); // Açık, nötr gri arka plan
        private Color _textColor = Color.White; // Metin rengi
        private Color _borderColor = Color.FromArgb(200, 200, 200); // Dış kenarlık rengi

        private int _cornerRadius = 8; // Köşe yuvarlaklığı
        private int _borderThickness = 1; // Kenarlık kalınlığı

        private ProgressBarDisplayMode _displayMode = ProgressBarDisplayMode.Percentage; // Metin gösterim modu
        private Font _displayTextFont = new Font("Century Gothic", 9.75f, FontStyle.Bold); // Metin fontu boyutu artırıldı


        // Animasyon için dahili Timer
        private Timer _animationTimer;
        private float _animationSpeed = 0.1f; // Animasyon hızı (0.05-0.2 arası iyi sonuç verir)

        // --- Yapıcı (Constructor) ---
        public KZ_ProgressBar()
        {
            // Çizim performansını artır
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(250, 25); // Varsayılan boyut
            BackColor = Color.Transparent; // Kontrolün arka planı saydam olsun (içini biz çizeceğiz)

            // Animasyon Timer'ını başlat
            _animationTimer = new Timer();
            _animationTimer.Interval = 15; // Çok hızlı tick (daha yumuşak animasyon)
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        // --- Özellikler (Properties) ---

        [Category("KZ İlerleme")]
        [Description("İlerleme çubuğunun mevcut değerini ayarlar (Minimum ve Maksimum arasında olmalıdır). " +
                     "Değer değişimleri yumuşak bir animasyonla gerçekleşir.")]
        [DefaultValue(0)]
        public int Value
        {
            get => _targetValue;
            set
            {
                if (value < _minimum) value = _minimum;
                if (value > _maximum) value = _maximum;
                if (_targetValue != value)
                {
                    _targetValue = value;
                    _animationTimer.Start(); // Animasyonu başlat
                }
            }
        }

        [Category("KZ İlerleme")]
        [Description("İlerleme çubuğunun minimum değerini ayarlar.")]
        [DefaultValue(0)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (value < 0) value = 0;
                if (_minimum != value)
                {
                    _minimum = value;
                    if (_targetValue < _minimum) _targetValue = _minimum;
                    if (_currentValue < _minimum) _currentValue = _minimum;
                    Invalidate();
                }
            }
        }

        [Category("KZ İlerleme")]
        [Description("İlerleme çubuğunun maksimum değerini ayarlar.")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value < _minimum) value = _minimum;
                if (_maximum != value)
                {
                    _maximum = value;
                    if (_targetValue > _maximum) _targetValue = _maximum;
                    if (_currentValue > _maximum) _currentValue = _maximum;
                    Invalidate();
                }
            }
        }

        [Category("KZ Görünüm - Renkler")]
        [Description("İlerleme dolgusunun rengini ayarlar.")]
        public Color ProgressBarColor { get => _progressBarColor; set { _progressBarColor = value; Invalidate(); } }

        [Category("KZ Görünüm - Renkler")]
        [Description("Çubuğun arka planının (boş kısmının) rengini ayarlar.")]
        public Color BarBackgroundColor { get => _barBackgroundColor; set { _barBackgroundColor = value; Invalidate(); } }

        [Category("KZ Görünüm - Renkler")]
        [Description("İlerleme çubuğunun dış kenarlık rengini ayarlar.")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("KZ Görünüm")]
        [Description("İlerleme çubuğunun köşe yuvarlaklığı yarıçapını ayarlar.")]
        [DefaultValue(8)]
        public int CornerRadius { get => _cornerRadius; set { _cornerRadius = value; Invalidate(); } }

        [Category("KZ Görünüm")]
        [Description("İlerleme çubuğunun kenarlık kalınlığını ayarlar (0 kenarlık yok).")]
        [DefaultValue(1)]
        public int BorderThickness { get => _borderThickness; set { _borderThickness = Math.Max(0, value); Invalidate(); } }

        [Category("KZ Metin")]
        [Description("İlerleme çubuğu üzerinde gösterilecek metnin rengini ayarlar.")]
        public Color TextColor { get => _textColor; set { _textColor = value; Invalidate(); } }

        [Category("KZ Metin")]
        [Description("İlerleme çubuğu üzerinde gösterilecek metin türünü ayarlar.")]
        [DefaultValue(ProgressBarDisplayMode.Percentage)]
        public ProgressBarDisplayMode DisplayMode { get => _displayMode; set { _displayMode = value; Invalidate(); } }

        [Category("KZ Metin")]
        [Description("İlerleme çubuğu üzerinde gösterilecek metnin yazı tipini ayarlar.")]
        public Font DisplayTextFont { get => _displayTextFont; set { _displayTextFont?.Dispose(); _displayTextFont = value; Invalidate(); } }

        [Category("KZ Animasyon")]
        [Description("İlerleme animasyonunun hızını ayarlar. Daha yüksek değer daha hızlı animasyon demektir (0.01 - 0.5 arası önerilir).")]
        [DefaultValue(0.1f)]
        public float AnimationSpeed { get => _animationSpeed; set => _animationSpeed = Math.Max(0.01f, Math.Min(0.5f, value)); }


        // --- Animasyon Timer Tick Olayı ---
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float difference = _targetValue - _currentValue;

            if (Math.Abs(difference) < 0.1f) // Hedefe çok yaklaştıysa durdur
            {
                _currentValue = _targetValue;
                _animationTimer.Stop();
            }
            else
            {
                _currentValue += difference * _animationSpeed; // Yumuşak geçiş
            }
            Invalidate(); // Yeniden çizimi tetikle
        }


        // --- Çizim Metodu (OnPaint) ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Kontrolün tüm alanını kaplayan ana dikdörtgen
            Rectangle boundsRect = ClientRectangle;

            // Kenarlık varsa, iç alanı hesapla
            Rectangle innerRect = boundsRect;
            if (_borderThickness > 0)
            {
                using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                {
                    // Dış kenarlığı çiz
                    using (GraphicsPath outerPath = CreateRoundedRectanglePath(boundsRect, _cornerRadius))
                    {
                        g.DrawPath(borderPen, outerPath);
                    }
                }
                // İç alanı küçült
                innerRect = new Rectangle(
                    boundsRect.X + _borderThickness,
                    boundsRect.Y + _borderThickness,
                    boundsRect.Width - (_borderThickness * 2),
                    boundsRect.Height - (_borderThickness * 2)
                );
            }

            // Eğer iç alan geçerli değilse çizimi durdur
            if (innerRect.Width <= 0 || innerRect.Height <= 0) return;

            // 1. Arka Planı Çiz (Boş Kısım - Düz Renk)
            using (SolidBrush bgBrush = new SolidBrush(_barBackgroundColor))
            using (GraphicsPath bgPath = CreateRoundedRectanglePath(innerRect, _cornerRadius))
            {
                g.FillPath(bgBrush, bgPath);
            }

            // 2. İlerleme Dolgusunu Çiz (Düz Renk)
            if (_maximum > _minimum)
            {
                // Animasyonlu mevcut değere göre ilerlemeyi hesapla
                float percentage = (_currentValue - _minimum) / (_maximum - _minimum);
                int filledWidth = (int)(innerRect.Width * percentage);

                Rectangle filledRect = new Rectangle(innerRect.X, innerRect.Y, filledWidth, innerRect.Height);

                if (filledRect.Width > 0)
                {
                    using (SolidBrush pbBrush = new SolidBrush(_progressBarColor))
                    {
                        // Çubuk tam doluysa (veya neredeyse tam doluysa) sağ köşeleri de yuvarlak yap.
                        // filledRect.Width, innerRect.Width'e çok yakınsa True yerine False gönder (köşeleri yuvarlak yap)
                        bool drawRightCornersSquare = (filledRect.Width < innerRect.Width - (_cornerRadius / 2)); // Yarım radiusluk bir tolerans verdim

                        using (GraphicsPath filledPath = CreateRoundedRectanglePath(filledRect, _cornerRadius, drawRightCornersSquare))
                        {
                            g.FillPath(pbBrush, filledPath);
                        }
                    }
                }
            }

            // 3. Metni Çiz
            string displayText = "";
            if (_displayMode == ProgressBarDisplayMode.Percentage)
            {
                float percentageVal = (_maximum > _minimum) ? (_currentValue - _minimum) * 100f / (_maximum - _minimum) : 0f;
                displayText = $"{percentageVal:0}%";
            }
            else if (_displayMode == ProgressBarDisplayMode.ValueAndMax)
            {
                displayText = $"{_currentValue:0}/{_maximum}";
            }

            if (!string.IsNullOrEmpty(displayText) && _displayMode != ProgressBarDisplayMode.None)
            {
                // Metin için hafif bir gölge efekti (şeffaflık azaltıldı)
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0))) // Daha az opak siyah gölge
                {
                    g.DrawString(displayText, _displayTextFont, shadowBrush,
                                 new RectangleF(boundsRect.X + 1, boundsRect.Y + 1, boundsRect.Width, boundsRect.Height),
                                 new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }

                // Ana metni çiz
                using (SolidBrush textBrush = new SolidBrush(_textColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(displayText, _displayTextFont, textBrush, boundsRect, sf);
                }
            }
        }

        // --- Yardımcı Metotlar ---
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius, bool drawRightCornersAsSquare = false)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            Rectangle arcRect = new Rectangle(rect.X, rect.Y, d, d);

            // Üst sol köşe
            path.AddArc(arcRect, 180, 90);

            // Üst sağ köşe
            if (drawRightCornersAsSquare)
            {
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y + radius);
            }
            else
            {
                arcRect.X = rect.Right - d;
                path.AddArc(arcRect, 270, 90);
            }

            // Alt sağ köşe
            if (drawRightCornersAsSquare)
            {
                path.AddLine(rect.Right, rect.Bottom - radius, rect.Right, rect.Bottom);
            }
            else
            {
                arcRect.Y = rect.Bottom - d;
                arcRect.X = rect.Right - d;
                path.AddArc(arcRect, 0, 90);
            }

            // Alt sol köşe
            arcRect.X = rect.X;
            arcRect.Y = rect.Bottom - d;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        // --- Kaynakları Serbest Bırak (Dispose) ---
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _displayTextFont?.Dispose();
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}