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
    [Designer(typeof(KZ_ListBoxDesigner))]
    [Description("Modern ListBox with header and customizable appearance")]
    public class KZ_ListBox : Control
    {
        #region Fields
        private ListBox _listBox = new ListBox();
        private int _borderRadius = 10;
        private Color _borderColor = Color.FromArgb(213, 218, 223);
        private Color _backColor = Color.White;
        private Color _headerBackColor = Color.FromArgb(123, 104, 238);
        private Color _headerTextColor = Color.White;
        private string _headerText = "Liste Başlığı";
        private Font _headerFont = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
        private int _headerHeight = 40;
        private int _borderSize = 1;
        private int _shadowDepth = 3;
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0);
        private Color _selectedItemBackColor = Color.FromArgb(230, 230, 250);
        private Color _selectedItemTextColor = Color.Black;
        private Color _itemSeparatorColor = Color.FromArgb(240, 240, 240);
        #endregion

        #region Constructor
        public KZ_ListBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Configure the base ListBox
            _listBox.BorderStyle = BorderStyle.None;
            _listBox.BackColor = _backColor; // Changed from Transparent to _backColor
            _listBox.ForeColor = Color.FromArgb(64, 64, 64);
            _listBox.Font = new Font("Segoe UI", 9.5f);
            _listBox.DrawMode = DrawMode.OwnerDrawFixed;
            _listBox.DrawItem += ListBox_DrawItem;
            Controls.Add(_listBox);

            Size = new Size(250, 300);
            UpdateListBoxPosition();
        }
        #endregion

        #region Properties
        [Category("KZ Görünüm")]
        [DefaultValue(10)]
        [Description("Kontrollerin köşe yuvarlaklık değeri")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [Description("Kenarlık rengi")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(1)]
        [Description("Kenarlık kalınlığı")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(3)]
        [Description("Gölge derinliği")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [Description("Gölge rengi")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("KZ Başlık")]
        [Description("Başlık arkaplan rengi")]
        public Color HeaderBackColor
        {
            get => _headerBackColor;
            set { _headerBackColor = value; Invalidate(); }
        }

        [Category("KZ Başlık")]
        [Description("Başlık yazı rengi")]
        public Color HeaderTextColor
        {
            get => _headerTextColor;
            set { _headerTextColor = value; Invalidate(); }
        }

        [Category("KZ Başlık")]
        [Description("Başlık metni")]
        public string HeaderText
        {
            get => _headerText;
            set { _headerText = value; Invalidate(); }
        }

        [Category("KZ Başlık")]
        [Description("Başlık yazı tipi")]
        public Font HeaderFont
        {
            get => _headerFont;
            set { _headerFont = value; Invalidate(); }
        }

        [Category("KZ Başlık")]
        [DefaultValue(40)]
        [Description("Başlık yüksekliği")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set { _headerHeight = value; UpdateListBoxPosition(); Invalidate(); }
        }

        [Category("KZ Öğeler")]
        [Description("Seçili öğe arkaplan rengi")]
        public Color SelectedItemBackColor
        {
            get => _selectedItemBackColor;
            set { _selectedItemBackColor = value; Invalidate(); }
        }

        [Category("KZ Öğeler")]
        [Description("Seçili öğe yazı rengi")]
        public Color SelectedItemTextColor
        {
            get => _selectedItemTextColor;
            set { _selectedItemTextColor = value; Invalidate(); }
        }

        [Category("KZ Öğeler")]
        [Description("Öğe ayırıcı çizgi rengi")]
        public Color ItemSeparatorColor
        {
            get => _itemSeparatorColor;
            set { _itemSeparatorColor = value; Invalidate(); }
        }

        [Category("KZ Veri")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        [Localizable(true)]
        [Description("Liste öğeleri")]
        public ListBox.ObjectCollection Items => _listBox.Items;

        // BackColor özelliğini tasarımcıda görünür hale getiriyoruz
        [Browsable(true)] // Burayı true yaptık
        [Category("KZ Görünüm")] // İlgili kategoriye taşıdık
        [Description("Ana kontrolün arka plan rengi.")]
        public override Color BackColor
        {
            get => _backColor;
            set { _backColor = value; _listBox.BackColor = value; Invalidate(); }
        }

        // Font özelliğini tasarımcıda görünür hale getiriyoruz
        [Browsable(true)] // Burayı true yaptık
        [Category("KZ Öğeler")] // İlgili kategoriye taşıdık (ListBox öğelerinin fontu olduğu için daha mantıklı)
        [Description("Liste öğelerinin yazı tipi.")]
        public override Font Font
        {
            get => _listBox.Font;
            set { _listBox.Font = value; Invalidate(); }
        }

        // ForeColor özelliğini tasarımcıda görünür hale getiriyoruz
        [Browsable(true)] // Burayı true yaptık
        [Category("KZ Öğeler")] // İlgili kategoriye taşıdık
        [Description("Liste öğelerinin yazı rengi.")]
        public override Color ForeColor
        {
            get => _listBox.ForeColor;
            set { _listBox.ForeColor = value; Invalidate(); }
        }
        #endregion

        #region Methods
        private void UpdateListBoxPosition()
        {
            // Adjust for border radius to prevent sharp corners peeking through
            int offset = _borderRadius / 2;
            _listBox.Location = new Point(_borderSize + offset, _headerHeight + _borderSize + offset);
            _listBox.Size = new Size(
                Width - (_borderSize * 2) - (offset * 2),
                Height - _headerHeight - (_borderSize * 2) - (offset * 2));
        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = isSelected ? _selectedItemBackColor : _backColor;
            Color textColor = isSelected ? _selectedItemTextColor : e.ForeColor;

            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (var brush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(_listBox.Items[e.Index].ToString(), e.Font, brush, e.Bounds, sf);
            }

            using (var pen = new Pen(_itemSeparatorColor))
            {
                e.Graphics.DrawLine(pen, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            e.DrawFocusRectangle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw shadow first
            DrawShadow(g);

            // Main background with full rounded corners
            Rectangle mainRect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = CreateRoundPath(mainRect, _borderRadius))
            using (var brush = new SolidBrush(_backColor))
            {
                g.FillPath(brush, path);
            }

            // Header with top rounded corners only
            Rectangle headerRect = new Rectangle(0, 0, Width, _headerHeight);
            using (var path = CreateTopRoundPath(headerRect, _borderRadius))
            using (var brush = new SolidBrush(_headerBackColor))
            {
                g.FillPath(brush, path);
            }

            // Header text
            using (var brush = new SolidBrush(_headerTextColor))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(_headerText, _headerFont, brush, headerRect, sf);
            }

            // Border
            using (var path = CreateRoundPath(mainRect, _borderRadius))
            using (var pen = new Pen(_borderColor, _borderSize))
            {
                g.DrawPath(pen, path);
            }
        }

        private void DrawShadow(Graphics g)
        {
            if (_shadowDepth <= 0) return;

            for (int i = 0; i < _shadowDepth; i++)
            {
                // Gölge opaklığını kademeli olarak azalt
                int alpha = _shadowColor.A - (i * (_shadowColor.A / (_shadowDepth + 1)));
                if (alpha < 0) alpha = 0;
                using (var pen = new Pen(Color.FromArgb(alpha, _shadowColor), 1))
                {
                    Rectangle rect = new Rectangle(
                        i + 1,
                        i + 1,
                        Width - (i + 1) * 2,
                        Height - (i + 1) * 2);

                    using (var path = CreateRoundPath(rect, _borderRadius))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            radius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        private GraphicsPath CreateTopRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            radius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();

            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateListBoxPosition();
            Invalidate();
        }
        #endregion

        #region Smart Tag
        public class KZ_ListBoxDesigner : ControlDesigner
        {
            private DesignerActionListCollection _actionLists;

            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (_actionLists == null)
                    {
                        _actionLists = new DesignerActionListCollection();
                        _actionLists.Add(new KZ_ListBoxActionList(Component));
                    }
                    return _actionLists;
                }
            }
        }

        public class KZ_ListBoxActionList : DesignerActionList
        {
            private KZ_ListBox _listBox;

            public KZ_ListBoxActionList(IComponent component) : base(component)
            {
                _listBox = component as KZ_ListBox;
            }

            public int BorderRadius
            {
                get => _listBox.BorderRadius;
                set => SetProperty("BorderRadius", value);
            }

            public Color BorderColor
            {
                get => _listBox.BorderColor;
                set => SetProperty("BorderColor", value);
            }

            public int BorderSize
            {
                get => _listBox.BorderSize;
                set => SetProperty("BorderSize", value);
            }

            public int ShadowDepth
            {
                get => _listBox.ShadowDepth;
                set => SetProperty("ShadowDepth", value);
            }

            public Color ShadowColor
            {
                get => _listBox.ShadowColor;
                set => SetProperty("ShadowColor", value);
            }

            public Color HeaderBackColor
            {
                get => _listBox.HeaderBackColor;
                set => SetProperty("HeaderBackColor", value);
            }

            public Color HeaderTextColor
            {
                get => _listBox.HeaderTextColor;
                set => SetProperty("HeaderTextColor", value);
            }

            public string HeaderText
            {
                get => _listBox.HeaderText;
                set => SetProperty("HeaderText", value);
            }

            public Font HeaderFont
            {
                get => _listBox.HeaderFont;
                set => SetProperty("HeaderFont", value);
            }

            public int HeaderHeight
            {
                get => _listBox.HeaderHeight;
                set => SetProperty("HeaderHeight", value);
            }

            public Color SelectedItemBackColor
            {
                get => _listBox.SelectedItemBackColor;
                set => SetProperty("SelectedItemBackColor", value);
            }

            public Color SelectedItemTextColor
            {
                get => _listBox.SelectedItemTextColor;
                set => SetProperty("SelectedItemTextColor", value);
            }

            public Color ItemSeparatorColor
            {
                get => _listBox.ItemSeparatorColor;
                set => SetProperty("ItemSeparatorColor", value);
            }

            public ListBox.ObjectCollection Items
            {
                get => _listBox.Items;
            }

            // BackColor'ı Smart Tag'e ekliyoruz
            public Color BackColor
            {
                get => _listBox.BackColor; // Artık _listBox.BackColor'ı kullanıyoruz
                set => SetProperty("BackColor", value);
            }

            // Font'u Smart Tag'e ekliyoruz
            public Font Font
            {
                get => _listBox.Font;
                set => SetProperty("Font", value);
            }

            // ForeColor'ı Smart Tag'e ekliyoruz
            public Color ForeColor
            {
                get => _listBox.ForeColor;
                set => SetProperty("ForeColor", value);
            }

            private void SetProperty(string propName, object value)
            {
                // PropertyDescriptor'ı _listBox yerine _listBox.Parent (_KZ_ListBox) üzerinden almalıyız
                // çünkü BackColor, Font, ForeColor gibi özellikler ana kontroldedir.
                PropertyDescriptor prop = TypeDescriptor.GetProperties(Component)[propName];
                prop?.SetValue(Component, value);
                _listBox.Invalidate();
                // Eğer değişiklik ana kontrolün de çizimini etkiliyorsa, ana kontrolü de invalidate etmeliyiz.
                // Örneğin, BackColor gibi, bu tüm kontrolün görünümünü etkiler.
                _listBox.Parent?.Invalidate();
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();

                items.Add(new DesignerActionHeaderItem("Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderRadius", "Köşe Yuvarlaklık", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderColor", "Kenarlık Rengi", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderSize", "Kenarlık Kalınlığı", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowDepth", "Gölge Derinliği", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowColor", "Gölge Rengi", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BackColor", "Ana Arkaplan Rengi", "Görünüm")); // Buraya ekledik

                items.Add(new DesignerActionHeaderItem("Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderText", "Başlık Metni", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderBackColor", "Başlık Arkaplan", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderTextColor", "Başlık Yazı Rengi", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderFont", "Başlık Yazı Tipi", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderHeight", "Başlık Yüksekliği", "Başlık"));

                items.Add(new DesignerActionHeaderItem("Öğeler"));
                items.Add(new DesignerActionPropertyItem("SelectedItemBackColor", "Seçili Öğe Arkaplan", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("SelectedItemTextColor", "Seçili Öğe Yazı Rengi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("ItemSeparatorColor", "Ayırıcı Çizgi Rengi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("Font", "Öğe Yazı Tipi", "Öğeler")); // Buraya ekledik
                items.Add(new DesignerActionPropertyItem("ForeColor", "Öğe Yazı Rengi", "Öğeler")); // Buraya ekledik
                items.Add(new DesignerActionPropertyItem("Items", "Liste Öğeleri", "Öğeler"));

                return items;
            }
        }
        #endregion
    }
}