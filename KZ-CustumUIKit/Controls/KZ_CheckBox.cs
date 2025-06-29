using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [Description("Yuvarlak köşeli animasyonlu onay kutusu (Kare versiyon)")]
    public class KZ_CheckBox : Control
    {
        // Özellikler
        private bool _checked = false;
        private Color _checkedColor = Color.MediumSlateBlue;
        private Color _uncheckedColor = Color.Gray;
        private Color _checkmarkColor = Color.WhiteSmoke;
        private int _borderRadius = 5;
        private float _animationProgress = 0f;
        private Timer _animationTimer;
        private int _fixedSize = 20; // Sabit kare boyut

        public KZ_CheckBox()
        {
            // Başlangıç ayarları
            this.Size = new Size(_fixedSize, _fixedSize);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.MinimumSize = new Size(16, 16);
            this.MaximumSize = new Size(100, 100);

            // Animasyon timer'ı ayarla
            _animationTimer = new Timer();
            _animationTimer.Interval = 15;
            _animationTimer.Tick += AnimationTimer_Tick;

            // Tıklama olayı
            this.Click += (s, e) => Checked = !Checked;
        }

        // Kontrol boyutunu kare olarak kilitle
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Genişlik ve yüksekliği eşitle
            int size = Math.Max(width, height);
            base.SetBoundsCore(x, y, size, size, specified);
        }

        [Category("KZ Appearance")]
        [Description("Kutunun işaretli olup olmadığını belirtir")]
        [DefaultValue(false)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    _animationTimer.Start();
                    OnCheckedChanged(EventArgs.Empty);
                }
            }
        }

        [Category("KZ Appearance")]
        [Description("İşaretli durumdaki arkaplan rengi")]
        public Color CheckedColor
        {
            get => _checkedColor;
            set { _checkedColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("İşaretsiz durumdaki arkaplan rengi")]
        public Color UncheckedColor
        {
            get => _uncheckedColor;
            set { _uncheckedColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Onay işareti rengi")]
        public Color CheckmarkColor
        {
            get => _checkmarkColor;
            set { _checkmarkColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Köşe yuvarlaklık yarıçapı")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, Math.Min(value, _fixedSize / 2)); Invalidate(); }
        }

        [Browsable(false)]
        public new Size Size
        {
            get => base.Size;
            set => base.Size = new Size(value.Width, value.Width); // Her zaman kare
        }

        [Description("Checked özelliği değiştiğinde tetiklenir")]
        public event EventHandler CheckedChanged;

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Animasyon ilerlemesini güncelle
            float target = _checked ? 1f : 0f;
            _animationProgress += (target - _animationProgress) / 5f;

            // Animasyon tamamlandı mı kontrol et
            if (Math.Abs(_animationProgress - target) < 0.01f)
            {
                _animationProgress = target;
                _animationTimer.Stop();
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Mevcut renkleri hesapla
            Color backColor = InterpolateColor(_uncheckedColor, _checkedColor, _animationProgress);
            Color borderColor = Color.FromArgb((int)(100 + 155 * _animationProgress), backColor);

            // Yuvarlak kare arkaplan çiz
            using (var path = CreateRoundedSquarePath(ClientRectangle, _borderRadius))
            using (var brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);

                // Kenarlık çiz
                using (var pen = new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Onay işareti çiz
            if (_animationProgress > 0)
            {
                float thickness = Width / 10f;
                using (var checkPen = new Pen(_checkmarkColor, thickness))
                {
                    checkPen.StartCap = LineCap.Round;
                    checkPen.EndCap = LineCap.Round;

                    // Onay işareti noktaları
                    PointF[] points = {
                        new PointF(Width * 0.25f, Height * 0.5f),
                        new PointF(Width * 0.4f, Height * 0.65f),
                        new PointF(Width * 0.75f, Height * 0.35f)
                    };

                    // Kademeli çizim
                    if (_animationProgress < 1)
                    {
                        for (int i = 0; i < points.Length - 1; i++)
                        {
                            float segmentProgress = Math.Min(1, _animationProgress * 3 - i);
                            if (segmentProgress > 0)
                            {
                                PointF endPoint = new PointF(
                                    points[i].X + (points[i + 1].X - points[i].X) * segmentProgress,
                                    points[i].Y + (points[i + 1].Y - points[i].Y) * segmentProgress);

                                g.DrawLine(checkPen, points[i], endPoint);
                            }
                        }
                    }
                    else
                    {
                        // Tamamlanmış onay işareti
                        g.DrawLines(checkPen, points);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundedSquarePath(Rectangle rect, int radius)
        {
            // Yuvarlak köşeli kare yolu oluştur
            GraphicsPath path = new GraphicsPath();
            radius = Math.Min(radius, rect.Width / 2);

            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        private Color InterpolateColor(Color start, Color end, float ratio)
        {
            // İki renk arasında geçiş yap
            return Color.FromArgb(
                (int)(start.R + (end.R - start.R) * ratio),
                (int)(start.G + (end.G - start.G) * ratio),
                (int)(start.B + (end.B - start.B) * ratio));
        }
    }
}