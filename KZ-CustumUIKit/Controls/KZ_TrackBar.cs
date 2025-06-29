using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [Designer(typeof(KZ_TrackBarDesigner))]
    [Description("Modern görünümlü, kaydırma efekti olan özel trackbar bileşeni")]
    public class KZ_TrackBar : Control
    {
        // Varsayılan değerler
        private int _value = 50;
        private int _min = 0;
        private int _max = 100;
        private Color _trackColor = Color.FromArgb(230, 230, 250);
        private Color _progressColor = Color.FromArgb(123, 104, 238);
        private Color _thumbColor = Color.FromArgb(106, 90, 205);
        private int _trackHeight = 6;
        private int _thumbSize = 20;
        private bool _isDragging = false;
        private float _thumbPosition = 0.5f;

        [Category("KZ Appearance")]
        [DefaultValue(50)]
        [Description("Trackbar'ın mevcut değeri")]
        public int Value
        {
            get => _value;
            set
            {
                value = Math.Max(_min, Math.Min(_max, value));
                if (_value != value)
                {
                    _value = value;
                    UpdateThumbPosition();
                    OnValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Category("KZ Appearance")]
        [DefaultValue(0)]
        [Description("Trackbar'ın minimum değeri")]
        public int Minimum
        {
            get => _min;
            set
            {
                if (_min != value)
                {
                    _min = value;
                    if (_value < _min) _value = _min;
                    if (_max <= _min) _max = _min + 1;
                    UpdateThumbPosition();
                    Invalidate();
                }
            }
        }

        [Category("KZ Appearance")]
        [DefaultValue(100)]
        [Description("Trackbar'ın maksimum değeri")]
        public int Maximum
        {
            get => _max;
            set
            {
                if (_max != value)
                {
                    _max = value;
                    if (_value > _max) _value = _max;
                    if (_min >= _max) _min = _max - 1;
                    UpdateThumbPosition();
                    Invalidate();
                }
            }
        }

        [Category("KZ Appearance")]
        [Description("Arka plan rengi")]
        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("İlerleme rengi")]
        public Color ProgressColor
        {
            get => _progressColor;
            set { _progressColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Thumb rengi")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(6)]
        [Description("Track yüksekliği")]
        public int TrackHeight
        {
            get => _trackHeight;
            set { _trackHeight = Math.Max(2, value); Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(20)]
        [Description("Thumb boyutu")]
        public int ThumbSize
        {
            get => _thumbSize;
            set { _thumbSize = Math.Max(10, value); Invalidate(); }
        }

        [Browsable(false)]
        public float ValueNormalized => (_value - _min) / (float)(_max - _min);

        public event EventHandler ValueChanged;

        public KZ_TrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.UserPaint |
                   ControlStyles.ResizeRedraw |
                   ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(200, 30);
            Cursor = Cursors.Hand;
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void UpdateThumbPosition()
        {
            _thumbPosition = (_value - _min) / (float)(_max - _min);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track arkaplanı
            int trackTop = (Height - _trackHeight) / 2;
            using (var trackPath = CreateRoundedRect(0, trackTop, Width, _trackHeight, _trackHeight / 2))
            using (var trackBrush = new SolidBrush(_trackColor))
            {
                g.FillPath(trackBrush, trackPath);
            }

            // İlerleme çubuğu
            int progressWidth = (int)(Width * _thumbPosition);
            if (progressWidth > 0)
            {
                using (var progressPath = CreateRoundedRect(0, trackTop, progressWidth, _trackHeight, _trackHeight / 2))
                using (var progressBrush = new SolidBrush(_progressColor))
                {
                    g.FillPath(progressBrush, progressPath);
                }
            }

            // Thumb (kaydırıcı)
            int thumbX = (int)((Width - _thumbSize) * _thumbPosition);
            int thumbY = (Height - _thumbSize) / 2;

            using (var thumbPath = new GraphicsPath())
            {
                thumbPath.AddEllipse(thumbX, thumbY, _thumbSize, _thumbSize);

                // Thumb gölgesi
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillEllipse(shadowBrush, thumbX + 2, thumbY + 2, _thumbSize, _thumbSize);
                }

                // Thumb dolgusu
                using (var thumbBrush = new SolidBrush(_thumbColor))
                {
                    g.FillPath(thumbBrush, thumbPath);
                }

                // Thumb kenarlığı
                using (var thumbPen = new Pen(Color.FromArgb(150, Color.White), 1.5f))
                {
                    g.DrawPath(thumbPen, thumbPath);
                }
            }
        }

        private GraphicsPath CreateRoundedRect(int x, int y, int width, int height, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
            path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                UpdateValueFromMousePosition(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                UpdateValueFromMousePosition(e.X);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
        }

        private void UpdateValueFromMousePosition(int mouseX)
        {
            float position = (float)(mouseX - _thumbSize / 2) / (Width - _thumbSize);
            position = Math.Max(0, Math.Min(1, position));
            _thumbPosition = position;
            int newValue = _min + (int)((_max - _min) * position);

            if (newValue != _value)
            {
                _value = newValue;
                OnValueChanged(EventArgs.Empty);
            }

            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateThumbPosition();
            Invalidate();
        }
    }

    // Smart Tag Designer
    public class KZ_TrackBarDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new KZ_TrackBarActionList(Component));
                }
                return _actionLists;
            }
        }
    }

    // Smart Tag Action List
    public class KZ_TrackBarActionList : DesignerActionList
    {
        private KZ_TrackBar _trackBar;

        public KZ_TrackBarActionList(IComponent component) : base(component)
        {
            _trackBar = component as KZ_TrackBar;
        }

        public int Value
        {
            get => _trackBar.Value;
            set => SetProperty("Value", value);
        }

        public int Minimum
        {
            get => _trackBar.Minimum;
            set => SetProperty("Minimum", value);
        }

        public int Maximum
        {
            get => _trackBar.Maximum;
            set => SetProperty("Maximum", value);
        }

        public Color TrackColor
        {
            get => _trackBar.TrackColor;
            set => SetProperty("TrackColor", value);
        }

        public Color ProgressColor
        {
            get => _trackBar.ProgressColor;
            set => SetProperty("ProgressColor", value);
        }

        public Color ThumbColor
        {
            get => _trackBar.ThumbColor;
            set => SetProperty("ThumbColor", value);
        }

        private void SetProperty(string propName, object value)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(_trackBar)[propName];
            prop?.SetValue(_trackBar, value);
            _trackBar.Invalidate();
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Değer Ayarları"));
            items.Add(new DesignerActionPropertyItem("Value", "Mevcut Değer:", "Değer Ayarları"));
            items.Add(new DesignerActionPropertyItem("Minimum", "Minimum Değer:", "Değer Ayarları"));
            items.Add(new DesignerActionPropertyItem("Maximum", "Maksimum Değer:", "Değer Ayarları"));

            items.Add(new DesignerActionHeaderItem("Görünüm Ayarları"));
            items.Add(new DesignerActionPropertyItem("TrackColor", "Track Rengi:", "Görünüm Ayarları"));
            items.Add(new DesignerActionPropertyItem("ProgressColor", "İlerleme Rengi:", "Görünüm Ayarları"));
            items.Add(new DesignerActionPropertyItem("ThumbColor", "Thumb Rengi:", "Görünüm Ayarları"));

            return items;
        }
    }
}