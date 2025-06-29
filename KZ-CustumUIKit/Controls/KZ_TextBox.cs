using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [Designer(typeof(KZ_TextBox.KZ_TextBoxDesigner))]
    [Description("Modern textbox with perfect shadows, typography and icon support")]
    public class KZ_TextBox : Control
    {
        #region Fields
        private TextBox _textBox = new TextBox();

        private const int TopPaddingForPlaceholder = 8;

        private int _borderRadius = 10;
        private Color _borderColor = Color.FromArgb(213, 218, 223);
        private Color _borderFocusColor = Color.FromArgb(123, 104, 238);
        private Color _backColor = Color.White;
        private int _borderSize = 2;
        private bool _underlinedStyle = false;

        private Image _leftIcon;
        private Image _rightIcon;
        // Icon paths are now internal and not exposed directly for Smart Tag selection
        private string _leftIconPath = null;
        private string _rightIconPath = null;

        private int _iconSize = 22;
        private int _iconPadding = 12;
        private bool _showLeftIcon = false;
        private bool _showRightIcon = false;

        private int _textPadding = 14;
        private Font _font = new Font("Segoe UI Semilight", 10.5f, FontStyle.Regular, GraphicsUnit.Point);

        private string _placeholderText = "";
        private Color _placeholderColor = Color.FromArgb(164, 165, 169);
        private Color _placeholderActiveColor = Color.FromArgb(123, 104, 238);
        private float _placeholderScale = 1.0f;
        private PointF _placeholderLocation;
        private Timer _animationTimer;
        private bool _isAnimating = false;
        private bool _isPlaceholderUp = false;
        private bool _hasText = false;
        private float _placeholderYPosition = 0;
        private const float PLACEHOLDER_SCALE_DOWN = 0.85f;
        private const int ANIMATION_DURATION = 200;
        private DateTime _animationStartTime;

        private bool _isPassword = false;
        private char _passwordChar = '•';

        private int _shadowDepth = 3;
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0);

        private Rectangle _leftIconRect;
        private Rectangle _rightIconRect;

        private bool _isMouseCurrentlyOverLeftIcon = false;
        private bool _isMouseCurrentlyOverRightIcon = false;
        #endregion

        #region Events
        [Category("KZ Icon Events")]
        [Description("Occurs when the left icon is clicked.")]
        public event EventHandler LeftIconClick;

        [Category("KZ Icon Events")]
        [Description("Occurs when the left icon is double-clicked.")]
        public event EventHandler LeftIconDoubleClick;

        [Category("KZ Icon Events")]
        [Description("Occurs when the mouse pointer enters the left icon area.")]
        public event EventHandler LeftIconMouseEnter;

        [Category("KZ Icon Events")]
        [Description("Occurs when the mouse pointer leaves the left icon area.")]
        public event EventHandler LeftIconMouseLeave;

        [Category("KZ Icon Events")]
        [Description("Occurs when the right icon is clicked.")]
        public event EventHandler RightIconClick;

        [Category("KZ Icon Events")]
        [Description("Occurs when the right icon is double-clicked.")]
        public event EventHandler RightIconDoubleClick;

        [Category("KZ Icon Events")]
        [Description("Occurs when the mouse pointer enters the right icon area.")]
        public event EventHandler RightIconMouseEnter;

        [Category("KZ Icon Events")]
        [Description("Occurs when the mouse pointer leaves the right icon area.")]
        public event EventHandler RightIconMouseLeave;
        #endregion

        #region Constructor
        public KZ_TextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            _textBox.Visible = false;

            _textBox.BorderStyle = BorderStyle.None;
            _textBox.BackColor = _backColor;
            _textBox.ForeColor = Color.FromArgb(64, 64, 64);
            _textBox.Font = _font;
            _textBox.Location = new Point(0, 0);
            _textBox.Size = new Size(100, 20);
            _textBox.TextChanged += (s, e) =>
            {
                _hasText = !string.IsNullOrEmpty(_textBox.Text);
                if (_hasText && !_isPlaceholderUp)
                {
                    _textBox.Visible = true;
                    StartPlaceholderAnimation(true);
                }
                OnTextChanged(e);
                Invalidate();
            };
            _textBox.GotFocus += (s, e) =>
            {
                OnGotFocus(e);
                if (!_isPlaceholderUp) StartPlaceholderAnimation(true);
                Invalidate();
            };
            _textBox.LostFocus += (s, e) =>
            {
                OnLostFocus(e);
                if (!_hasText)
                {
                    StartPlaceholderAnimation(false);
                }
                Invalidate();
            };

            Controls.Add(_textBox);

            _animationTimer = new Timer();
            _animationTimer.Interval = 15;
            _animationTimer.Tick += AnimationTimer_Tick;

            Size = new Size(250, 58);
            UpdateTextBoxPosition();
            UpdatePlaceholderPosition();
            UpdateIconRectangles();
        }
        #endregion

        #region Properties
        [Category("KZ Appearance")]
        public override Color BackColor
        {
            get => _backColor;
            set { _backColor = value; _textBox.BackColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(10)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color BorderFocusColor
        {
            get => _borderFocusColor;
            set { _borderFocusColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(2)]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(false)]
        public bool UnderlinedStyle
        {
            get => _underlinedStyle;
            set { _underlinedStyle = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(3)]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("KZ Text")]
        [DefaultValue(14)]
        public int TextPadding
        {
            get => _textPadding;
            set { _textPadding = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); Invalidate(); }
        }

        [Category("KZ Text")]
        public override Font Font
        {
            get => _font;
            set
            {
                _font = value;
                _textBox.Font = value;
                UpdateTextBoxPosition();
                UpdatePlaceholderPosition();
                Invalidate();
            }
        }

        // --- İKON YÖNETİMİ BAŞLANGICI ---
        [Category("KZ Icons")]
        [Description("Sets the left icon image.")]
        [Browsable(true)] // Smart Tag'de görünür olması için true yaptık
        [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))] // ImageEditor kullanıyoruz
        public Image LeftIcon
        {
            get => _leftIcon;
            set
            {
                if (_leftIcon != value)
                {
                    _leftIcon?.Dispose(); // Eski ikonu dispose et

                    if (value != null)
                    {
                        // Image.FromFile'ın aksine, ImageEditor ile gelen Image
                        // genellikle zaten bellekte ve dosyayı kilitlemez.
                        // Ancak yine de olası kilitlenme sorunlarına karşı bir kopyasını almak daha güvenli.
                        try
                        {
                            _leftIcon = new Bitmap(value);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Hata: Sol ikon atanırken hata: {ex.Message}");
                            _leftIcon = null;
                        }
                    }
                    else
                    {
                        _leftIcon = null;
                    }

                    _leftIconPath = null; // Direct image assignment clears path
                    UpdateTextBoxPosition();
                    UpdatePlaceholderPosition();
                    UpdateIconRectangles();
                    Invalidate();
                }
            }
        }

        [Category("KZ Icons")]
        [Description("Sets the right icon image.")]
        [Browsable(true)] // Smart Tag'de görünür olması için true yaptık
        [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))] // ImageEditor kullanıyoruz
        public Image RightIcon
        {
            get => _rightIcon;
            set
            {
                if (_rightIcon != value)
                {
                    _rightIcon?.Dispose(); // Eski ikonu dispose et

                    if (value != null)
                    {
                        try
                        {
                            _rightIcon = new Bitmap(value);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Hata: Sağ ikon atanırken hata: {ex.Message}");
                            _rightIcon = null;
                        }
                    }
                    else
                    {
                        _rightIcon = null;
                    }

                    _rightIconPath = null; // Direct image assignment clears path
                    UpdateTextBoxPosition();
                    UpdatePlaceholderPosition();
                    UpdateIconRectangles();
                    Invalidate();
                }
            }
        }

        // LeftIconPath ve RightIconPath özellikleri artık Smart Tag'de görünmeyecek
        // Sadece ImageEditor üzerinden doğrudan Image seçimi yapılacak.
        [Browsable(false)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string LeftIconPath
        {
            get => _leftIconPath;
            set
            {
                if (_leftIconPath != value)
                {
                    _leftIconPath = value;
                    if (!string.IsNullOrWhiteSpace(_leftIconPath) && File.Exists(_leftIconPath))
                    {
                        try
                        {
                            _leftIcon?.Dispose();
                            using (Image tempImage = Image.FromFile(_leftIconPath))
                            {
                                _leftIcon = new Bitmap(tempImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Hata: Sol ikon '{_leftIconPath}' yolundan yüklenemedi: {ex.Message}");
                            _leftIcon?.Dispose();
                            _leftIcon = null;
                        }
                    }
                    else
                    {
                        _leftIcon?.Dispose();
                        _leftIcon = null;
                    }
                    UpdateTextBoxPosition();
                    UpdatePlaceholderPosition();
                    UpdateIconRectangles();
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string RightIconPath
        {
            get => _rightIconPath;
            set
            {
                if (_rightIconPath != value)
                {
                    _rightIconPath = value;
                    if (!string.IsNullOrWhiteSpace(_rightIconPath) && File.Exists(_rightIconPath))
                    {
                        try
                        {
                            _rightIcon?.Dispose();
                            using (Image tempImage = Image.FromFile(_rightIconPath))
                            {
                                _rightIcon = new Bitmap(tempImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Hata: Sağ ikon '{_rightIconPath}' yolundan yüklenemedi: {ex.Message}");
                            _rightIcon?.Dispose();
                            _rightIcon = null;
                        }
                    }
                    else
                    {
                        _rightIcon?.Dispose();
                        _rightIcon = null;
                    }
                    UpdateTextBoxPosition();
                    UpdatePlaceholderPosition();
                    UpdateIconRectangles();
                    Invalidate();
                }
            }
        }
        // --- İKON YÖNETİMİ SONU ---

        [Category("KZ Icons")]
        [DefaultValue(22)]
        public int IconSize
        {
            get => _iconSize;
            set { _iconSize = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); UpdateIconRectangles(); Invalidate(); }
        }

        [Category("KZ Icons")]
        [DefaultValue(12)]
        public int IconPadding
        {
            get => _iconPadding;
            set { _iconPadding = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); UpdateIconRectangles(); Invalidate(); }
        }

        [Category("KZ Icons")]
        [DefaultValue(false)]
        public bool ShowLeftIcon
        {
            get => _showLeftIcon;
            set { _showLeftIcon = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); UpdateIconRectangles(); Invalidate(); }
        }

        [Category("KZ Icons")]
        [DefaultValue(false)]
        public bool ShowRightIcon
        {
            get => _showRightIcon;
            set { _showRightIcon = value; UpdateTextBoxPosition(); UpdatePlaceholderPosition(); UpdateIconRectangles(); Invalidate(); }
        }

        [Category("KZ Placeholder")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set { _placeholderText = value; UpdatePlaceholderPosition(); Invalidate(); }
        }

        [Category("KZ Placeholder")]
        public Color PlaceholderColor
        {
            get => _placeholderColor;
            set { _placeholderColor = value; Invalidate(); }
        }

        [Category("KZ Placeholder")]
        public Color PlaceholderActiveColor
        {
            get => _placeholderActiveColor;
            set { _placeholderActiveColor = value; Invalidate(); }
        }

        [Category("KZ Placeholder")]
        [DefaultValue(0.85f)]
        public float PlaceholderScale { get; set; } = 0.85f;

        [Category("KZ Behavior")]
        [DefaultValue(false)]
        public bool Password
        {
            get => _isPassword;
            set
            {
                _isPassword = value;
                _textBox.UseSystemPasswordChar = value;
                if (_isPassword)
                    _textBox.PasswordChar = _passwordChar;
                if (DesignMode)
                {
                    _textBox.Text = value ? "" : Text;
                }
                Invalidate();
            }
        }

        [Category("KZ Behavior")]
        [DefaultValue('•')]
        public char PasswordChar
        {
            get => _passwordChar;
            set
            {
                _passwordChar = value;
                if (_isPassword)
                    _textBox.PasswordChar = value;
            }
        }

        [Browsable(true)]
        public override string Text
        {
            get => _textBox.Text;
            set
            {
                if (DesignMode)
                {
                    _textBox.Text = _isPassword ? "" : value;
                }
                else
                {
                    _textBox.Text = value;
                }
                _hasText = !string.IsNullOrEmpty(_textBox.Text);
                Invalidate();
            }
        }

        [Browsable(false)]
        public override Color ForeColor
        {
            get => _textBox.ForeColor;
            set => _textBox.ForeColor = value;
        }
        #endregion

        #region Methods
        private void UpdateTextBoxPosition()
        {
            int left = _textPadding + (_showLeftIcon && _leftIcon != null ? _iconSize + _iconPadding : 0);
            int right = _textPadding + (_showRightIcon && _rightIcon != null ? _iconSize + _iconPadding : 0);

            int textBoxHeight = _font.Height;
            _textBox.Location = new Point(left, TopPaddingForPlaceholder + (this.Height - TopPaddingForPlaceholder - textBoxHeight) / 2);
            _textBox.Width = this.Width - left - right;
            _textBox.Height = textBoxHeight;
        }

        private void UpdatePlaceholderPosition()
        {
            int left = _textPadding + (_showLeftIcon && _leftIcon != null ? _iconSize + _iconPadding : 0);

            float topY = 0;
            float baseY = _textBox.Top;

            float yPos = _isAnimating ? _placeholderYPosition : (_isPlaceholderUp ? topY : baseY);
            _placeholderLocation = new PointF(left, yPos);
        }

        private void UpdateIconRectangles()
        {
            _leftIconRect = Rectangle.Empty;
            _rightIconRect = Rectangle.Empty;

            if (_showLeftIcon && _leftIcon != null)
            {
                int left = _iconPadding;
                int top = TopPaddingForPlaceholder + (Height - TopPaddingForPlaceholder - _iconSize) / 2;
                _leftIconRect = new Rectangle(left, top, _iconSize, _iconSize);
            }

            if (_showRightIcon && _rightIcon != null)
            {
                int left = Width - _iconPadding - _iconSize;
                int top = TopPaddingForPlaceholder + (Height - TopPaddingForPlaceholder - _iconSize) / 2;
                _rightIconRect = new Rectangle(left, top, _iconSize, _iconSize);
            }
        }

        private void StartPlaceholderAnimation(bool moveUp)
        {
            if (_isAnimating || _isPlaceholderUp == moveUp)
                return;

            _isPlaceholderUp = moveUp;
            _isAnimating = true;
            _animationStartTime = DateTime.Now;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float elapsed = (float)(DateTime.Now - _animationStartTime).TotalMilliseconds;
            float progress = Math.Min(elapsed / ANIMATION_DURATION, 1.0f);
            // EaseInOutCubic geçiş fonksiyonu
            float easedProgress = EaseInOutCubic(progress);

            float topY = 0;
            float baseY = _textBox.Top;

            _placeholderYPosition = _isPlaceholderUp
                ? baseY + (topY - baseY) * easedProgress
                : topY + (baseY - topY) * easedProgress;

            _placeholderScale = _isPlaceholderUp
                ? 1.0f - (1.0f - PLACEHOLDER_SCALE_DOWN) * easedProgress
                : PLACEHOLDER_SCALE_DOWN + (1.0f - PLACEHOLDER_SCALE_DOWN) * easedProgress;

            if (progress >= 1.0f)
            {
                _animationTimer.Stop();
                _isAnimating = false;
                _placeholderScale = _isPlaceholderUp ? PLACEHOLDER_SCALE_DOWN : 1.0f;

                if (!_isPlaceholderUp && !_hasText)
                {
                    _textBox.Visible = false;
                }
                UpdatePlaceholderPosition();
            }
            Invalidate();
        }
        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            g.Clear(Parent.BackColor);

            RectangleF rectF = new RectangleF(
                _borderSize / 2f,
                TopPaddingForPlaceholder + _borderSize / 2f,
                this.Width - _borderSize,
                this.Height - TopPaddingForPlaceholder - _borderSize);

            DrawShadow(g);

            using (var path = CreateRoundPath(Rectangle.Round(rectF), _borderRadius))
            using (var brush = new SolidBrush(_backColor))
            {
                g.FillPath(brush, path);
            }

            Color borderColor = _textBox.Focused ? _borderFocusColor : _borderColor;
            DrawBorder(g, borderColor, Rectangle.Round(rectF));

            if ((_isPlaceholderUp || _isAnimating) && !string.IsNullOrEmpty(_placeholderText))
            {
                using (var placeholderFont = new Font(_font.FontFamily, _font.Size * _placeholderScale, _font.Style))
                {
                    SizeF placeholderSize = g.MeasureString(_placeholderText, placeholderFont);
                    float gapWidth = placeholderSize.Width + 8;
                    RectangleF gapRect = new RectangleF(_placeholderLocation.X - 4, TopPaddingForPlaceholder, gapWidth, _borderSize);
                    g.FillRectangle(new SolidBrush(_backColor), gapRect);
                }
            }

            using (var brush = new SolidBrush(_textBox.Focused ? _placeholderActiveColor : _placeholderColor))
            using (var placeholderFont = new Font(_font.FontFamily, _font.Size * _placeholderScale, _font.Style))
            {
                if (!_isPlaceholderUp && !_hasText)
                {
                    g.DrawString(_placeholderText, _font, new SolidBrush(_placeholderColor), _placeholderLocation);
                }
                else
                {
                    g.DrawString(_placeholderText, placeholderFont, brush, _placeholderLocation);
                }
            }

            DrawIcons(g);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!(_showLeftIcon && _leftIcon != null && _leftIconRect.Contains(e.Location)) &&
                !(_showRightIcon && _rightIcon != null && _rightIconRect.Contains(e.Location)))
            {
                _textBox.Visible = true;
                _textBox.Focus();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_showLeftIcon && _leftIcon != null && _leftIconRect.Contains(e.Location))
            {
                LeftIconClick?.Invoke(this, EventArgs.Empty);
            }
            else if (_showRightIcon && _rightIcon != null && _rightIconRect.Contains(e.Location))
            {
                RightIconClick?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (_showLeftIcon && _leftIcon != null && _leftIconRect.Contains(e.Location))
            {
                LeftIconDoubleClick?.Invoke(this, EventArgs.Empty);
            }
            else if (_showRightIcon && _rightIcon != null && _rightIconRect.Contains(e.Location))
            {
                RightIconDoubleClick?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool overLeft = _showLeftIcon && _leftIcon != null && _leftIconRect.Contains(e.Location);
            bool overRight = _showRightIcon && _rightIcon != null && _rightIconRect.Contains(e.Location);

            if (overLeft && !_isMouseCurrentlyOverLeftIcon)
            {
                _isMouseCurrentlyOverLeftIcon = true;
                LeftIconMouseEnter?.Invoke(this, EventArgs.Empty);
            }
            else if (!overLeft && _isMouseCurrentlyOverLeftIcon)
            {
                _isMouseCurrentlyOverLeftIcon = false;
                LeftIconMouseLeave?.Invoke(this, EventArgs.Empty);
            }

            if (overRight && !_isMouseCurrentlyOverRightIcon)
            {
                _isMouseCurrentlyOverRightIcon = true;
                RightIconMouseEnter?.Invoke(this, EventArgs.Empty);
            }
            else if (!overRight && _isMouseCurrentlyOverRightIcon)
            {
                _isMouseCurrentlyOverRightIcon = false;
                RightIconMouseLeave?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_isMouseCurrentlyOverLeftIcon)
            {
                _isMouseCurrentlyOverLeftIcon = false;
                LeftIconMouseLeave?.Invoke(this, EventArgs.Empty);
            }
            if (_isMouseCurrentlyOverRightIcon)
            {
                _isMouseCurrentlyOverRightIcon = false;
                RightIconMouseLeave?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DrawShadow(Graphics g)
        {
            if (_shadowDepth <= 0 || DesignMode) return;

            Rectangle rect = new Rectangle(0, TopPaddingForPlaceholder, Width - 1, Height - TopPaddingForPlaceholder - 1);
            for (int i = 0; i < _shadowDepth; i++)
            {
                using (var path = CreateRoundPath(rect, _borderRadius))
                using (var pen = new Pen(Color.FromArgb(12 * (ShadowDepth - i), _shadowColor), 1))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
                rect.Inflate(-1, -1);
            }
        }

        private void DrawBorder(Graphics g, Color borderColor, Rectangle rect)
        {
            if (_borderSize <= 0) return;

            if (_underlinedStyle)
            {
                using (var pen = new Pen(borderColor, _borderSize))
                {
                    g.DrawLine(pen, 0, Height - _borderSize, Width, Height - _borderSize);
                }
            }
            else
            {
                using (var path = CreateRoundPath(rect, _borderRadius))
                using (var pen = new Pen(borderColor, _borderSize))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DrawIcons(Graphics g)
        {
            if (_showLeftIcon && _leftIcon != null)
            {
                g.DrawImage(_leftIcon, _leftIconRect);
            }

            if (_showRightIcon && _rightIcon != null)
            {
                g.DrawImage(_rightIcon, _rightIconRect);
            }
        }

        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTextBoxPosition();
            UpdatePlaceholderPosition();
            UpdateIconRectangles();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftIcon?.Dispose();
                _rightIcon?.Dispose();
                _animationTimer?.Dispose();
                _textBox?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Smart Tag
        public class KZ_TextBoxDesigner : ControlDesigner
        {
            private DesignerActionListCollection _actionLists;

            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (_actionLists == null)
                    {
                        _actionLists = new DesignerActionListCollection();
                        _actionLists.Add(new KZ_TextBoxActionList(Component));
                    }
                    return _actionLists;
                }
            }
        }

        public class KZ_TextBoxActionList : DesignerActionList
        {
            private KZ_TextBox _textBox;

            public KZ_TextBoxActionList(IComponent component) : base(component)
            {
                _textBox = component as KZ_TextBox;
            }

            private void SetProperty(string propName, object value)
            {
                PropertyDescriptor prop = TypeDescriptor.GetProperties(_textBox)[propName];
                prop?.SetValue(_textBox, value);
                _textBox.Invalidate();
            }

            public int BorderRadius { get => _textBox.BorderRadius; set => SetProperty("BorderRadius", value); }
            public Color BorderColor { get => _textBox.BorderColor; set => SetProperty("BorderColor", value); }
            public Color BorderFocusColor { get => _textBox.BorderFocusColor; set => SetProperty("BorderFocusColor", value); }
            public bool UnderlinedStyle { get => _textBox.UnderlinedStyle; set => SetProperty("UnderlinedStyle", value); }
            public int ShadowDepth { get => _textBox.ShadowDepth; set => SetProperty("ShadowDepth", value); }
            public Color ShadowColor { get => _textBox.ShadowColor; set => SetProperty("ShadowColor", value); }
            public int TextPadding { get => _textBox.TextPadding; set => SetProperty("TextPadding", value); }
            public Font Font { get => _textBox.Font; set => SetProperty("Font", value); }

            // LeftIcon ve RightIcon'ı doğrudan Image tipi olarak Smart Tag'e ekledik
            [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
            public Image LeftIcon { get => _textBox.LeftIcon; set => SetProperty("LeftIcon", value); }

            [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
            public Image RightIcon { get => _textBox.RightIcon; set => SetProperty("RightIcon", value); }

            public int IconSize { get => _textBox.IconSize; set => SetProperty("IconSize", value); }
            public int IconPadding { get => _textBox.IconPadding; set => SetProperty("IconPadding", value); }
            public bool ShowLeftIcon { get => _textBox.ShowLeftIcon; set => SetProperty("ShowLeftIcon", value); }
            public bool ShowRightIcon { get => _textBox.ShowRightIcon; set => SetProperty("ShowRightIcon", value); }
            public string PlaceholderText { get => _textBox.PlaceholderText; set => SetProperty("PlaceholderText", value); }
            public Color PlaceholderColor { get => _textBox.PlaceholderColor; set => SetProperty("PlaceholderColor", value); }
            public Color PlaceholderActiveColor { get => _textBox.PlaceholderActiveColor; set => SetProperty("PlaceholderActiveColor", value); }
            public float PlaceholderScale { get => _textBox.PlaceholderScale; set => SetProperty("PlaceholderScale", value); }
            public bool Password { get => _textBox.Password; set => SetProperty("Password", value); }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                var items = new DesignerActionItemCollection();

                items.Add(new DesignerActionHeaderItem("Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderRadius", "Kenarlık Yuvarlaklığı:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderColor", "Kenarlık Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderFocusColor", "Odak Kenarlık Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("UnderlinedStyle", "Altı Çizgili Stil:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowDepth", "Gölge Derinliği:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowColor", "Gölge Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("TextPadding", "Metin Boşluğu:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("Font", "Yazı Tipi:", "Görünüm"));

                items.Add(new DesignerActionHeaderItem("İkonlar"));

                items.Add(new DesignerActionPropertyItem("LeftIcon", "Sol İkon:", "İkonlar"));
                items.Add(new DesignerActionPropertyItem("RightIcon", "Sağ İkon:", "İkonlar"));
                items.Add(new DesignerActionPropertyItem("IconSize", "İkon Boyutu:", "İkonlar"));
                items.Add(new DesignerActionPropertyItem("IconPadding", "İkon Boşluğu:", "İkonlar"));
                items.Add(new DesignerActionPropertyItem("ShowLeftIcon", "Sol İkonu Göster:", "İkonlar"));
                items.Add(new DesignerActionPropertyItem("ShowRightIcon", "Sağ İkonu Göster:", "İkonlar"));

                items.Add(new DesignerActionHeaderItem("Yer Tutucu"));
                items.Add(new DesignerActionPropertyItem("PlaceholderText", "Yer Tutucu Metni:", "Yer Tutucu"));
                items.Add(new DesignerActionPropertyItem("PlaceholderColor", "Yer Tutucu Rengi:", "Yer Tutucu"));
                items.Add(new DesignerActionPropertyItem("PlaceholderActiveColor", "Aktif Yer Tutucu Rengi:", "Yer Tutucu"));
                items.Add(new DesignerActionPropertyItem("PlaceholderScale", "Yer Tutucu Boyutu:", "Yer Tutucu"));

                items.Add(new DesignerActionHeaderItem("Davranış"));
                items.Add(new DesignerActionPropertyItem("Password", "Şifre Modu:","Davranış"));

                return items;
            }
        }
        #endregion
    }
}