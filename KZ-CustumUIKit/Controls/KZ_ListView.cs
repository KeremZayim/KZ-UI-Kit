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
    [Designer(typeof(KZ_ListViewDesigner))]
    [Description("Modern ListView with header, customizable appearance, and columns.")]
    public class KZ_ListView : Control
    {
        #region Fields
        private ListView _listView = new ListView();
        private int _borderRadius = 10;
        private Color _borderColor = Color.FromArgb(213, 218, 223);
        private Color _backColor = Color.White; // Ana kontrolün arka plan rengi
        private Color _headerBackColor = Color.FromArgb(123, 104, 238);
        private Color _headerTextColor = Color.White;
        private string _headerText = "Liste Başlığı";
        private Font _headerFont = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
        private int _headerHeight = 40;
        private int _borderSize = 1;
        private int _shadowDepth = 3;
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0); // Yarı şeffaf siyah
        private Color _selectedItemBackColor = Color.FromArgb(230, 230, 250); // Seçili öğe arka planı
        private Color _selectedItemTextColor = Color.Black; // Seçili öğe metin rengi
        private Color _itemBackColor = Color.White; // Liste öğelerinin varsayılan arka plan rengi
        private Color _columnHeaderBackColor = Color.WhiteSmoke; // Sütun başlıkları arka planı
        private Color _columnHeaderTextColor = Color.Black; // Sütun başlıkları metin rengi
        private Color _gridLinesColor = Color.FromArgb(220, 220, 220); // Izgara çizgisi rengi
        private bool _drawEmptyCells = false; // Yeni özellik: Boş hücreleri çiz

        // Sütun başlık yüksekliğini tutmak için alan
        private int _columnHeaderActualHeight = 0;
        #endregion

        #region Constructor
        public KZ_ListView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Temel ListView'ı yapılandır
            _listView.BorderStyle = BorderStyle.None;
            _listView.BackColor = _itemBackColor; // ListView'ın arka planını _itemBackColor ile ayarla
            _listView.ForeColor = Color.FromArgb(64, 64, 64);
            _listView.Font = new Font("Segoe UI", 9.5f);
            _listView.View = View.Details; // Detay görünümü varsayılan olsun
            _listView.FullRowSelect = true; // Tüm satırı seçili hale getir
            _listView.HeaderStyle = ColumnHeaderStyle.Nonclickable; // Başlık tıklanabilir olmasın (isteğe bağlı)
            _listView.GridLines = false; // GridLines'ı kapatıyoruz, çünkü kendimiz çizeceğiz

            // ListView'ın özel çizimini elle kontrol etme (OwnerDraw için gerekli)
            _listView.OwnerDraw = true;
            _listView.DrawColumnHeader += ListView_DrawColumnHeader;
            _listView.DrawItem += ListView_DrawItem;
            _listView.DrawSubItem += ListView_DrawSubItem;

            Controls.Add(_listView);

            Size = new Size(250, 300);
            UpdateListViewPosition(); // Konumlandırmayı gölge ve kenarlık için güncelle
        }
        #endregion

        #region Properties
        [Category("KZ Appearance")]
        [DefaultValue(10)]
        [Description("Radius of the control corners.")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; UpdateListViewPosition(); Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Color of the border.")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(1)]
        [Description("Thickness of the border.")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; UpdateListViewPosition(); Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(3)]
        [Description("Depth of the shadow.")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = value; UpdateListViewPosition(); Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Color of the shadow.")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("KZ Header")]
        [Description("Background color of the header.")]
        public Color HeaderBackColor
        {
            get => _headerBackColor;
            set { _headerBackColor = value; Invalidate(); }
        }

        [Category("KZ Header")]
        [Description("Text color of the header.")]
        public Color HeaderTextColor
        {
            get => _headerTextColor;
            set { _headerTextColor = value; Invalidate(); }
        }

        [Category("KZ Header")]
        [Description("Text displayed in the header.")]
        public string HeaderText
        {
            get => _headerText;
            set { _headerText = value; Invalidate(); }
        }

        [Category("KZ Header")]
        [Description("Font of the header text.")]
        public Font HeaderFont
        {
            get => _headerFont;
            set { _headerFont = value; Invalidate(); }
        }

        [Category("KZ Header")]
        [DefaultValue(40)]
        [Description("Height of the header.")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set { _headerHeight = value; UpdateListViewPosition(); Invalidate(); }
        }

        [Category("KZ Items")]
        [Description("Background color of selected items.")]
        public Color SelectedItemBackColor
        {
            get => _selectedItemBackColor;
            set { _selectedItemBackColor = value; Invalidate(); }
        }

        [Category("KZ Items")]
        [Description("Text color of selected items.")]
        public Color SelectedItemTextColor
        {
            get => _selectedItemTextColor;
            set { _selectedItemTextColor = value; Invalidate(); }
        }

        [Category("KZ Items")]
        [Description("Default background color of list items.")]
        public Color ItemBackColor
        {
            get => _itemBackColor;
            set { _itemBackColor = value; _listView.BackColor = value; Invalidate(); }
        }

        [Category("KZ Columns")]
        [Description("Background color of column headers.")]
        public Color ColumnHeaderBackColor
        {
            get => _columnHeaderBackColor;
            set { _columnHeaderBackColor = value; Invalidate(); }
        }

        [Category("KZ Columns")]
        [Description("Text color of column headers.")]
        public Color ColumnHeaderTextColor
        {
            get => _columnHeaderTextColor;
            set { _columnHeaderTextColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [Description("Color of the grid lines between items/subitems.")]
        public Color GridLinesColor
        {
            get => _gridLinesColor;
            set { _gridLinesColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        [DefaultValue(false)]
        [Description("Determines whether empty cells are drawn with grid lines even if there are no items.")]
        public bool DrawEmptyCells
        {
            get => _drawEmptyCells;
            set { _drawEmptyCells = value; Invalidate(); }
        }

        [Category("KZ Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        [Localizable(true)]
        [Description("ListView items.")]
        public ListView.ListViewItemCollection Items => _listView.Items;

        [Category("KZ Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        [Localizable(true)]
        [Description("ListView column headers.")]
        public ListView.ColumnHeaderCollection Columns => _listView.Columns;

        // KZ_ListView'ın genel arka plan rengi
        [Browsable(true)]
        [Category("KZ Appearance")]
        [Description("Main background color of the control.")]
        public override Color BackColor
        {
            get => _backColor;
            set { _backColor = value; Invalidate(); }
        }

        // ListView'ın Font ve ForeColor'ını kendi properties'imizden yönetelim.
        // Bu property'ler, alt ListView'a aktarılacak şekilde ayarlandı.
        [Browsable(true)]
        [Category("KZ Items")]
        [Description("Font for ListView items.")]
        public override Font Font
        {
            get => _listView.Font;
            set { _listView.Font = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("KZ Items")]
        [Description("Foreground color for ListView items.")]
        public override Color ForeColor
        {
            get => _listView.ForeColor;
            set { _listView.ForeColor = value; Invalidate(); }
        }

        // ListView'ın görünüm modunu dışarıya açalım
        [Category("KZ Appearance")]
        [DefaultValue(View.Details)]
        [Description("The view mode of the ListView.")]
        public View View
        {
            get => _listView.View;
            set => _listView.View = value;
        }

        // ListView'ın tam satır seçimini dışarıya açalım
        [Category("KZ Items")]
        [DefaultValue(true)]
        [Description("Determines whether selecting a row selects the entire row.")]
        public bool FullRowSelect
        {
            get => _listView.FullRowSelect;
            set => _listView.FullRowSelect = value;
        }

        // ListView'ın başlık stilini dışarıya açalım
        [Category("KZ Columns")]
        [DefaultValue(ColumnHeaderStyle.Nonclickable)]
        [Description("The style of the column headers.")]
        public ColumnHeaderStyle HeaderStyle
        {
            get => _listView.HeaderStyle;
            set => _listView.HeaderStyle = value;
        }

        #endregion

        #region Methods
        private void UpdateListViewPosition()
        {
            // ListView'ın konumunu ve boyutunu, hem borderSize hem de shadowDepth'i dikkate alarak ayarla.
            // Yuvarlak köşeler için ekstra bir iç boşluk bırakmalıyız ki ListView'ın dikdörtgen kenarları görünmesin.
            int innerPadding = _borderSize + _shadowDepth;
            int cornerOffset = _borderRadius > 0 ? _borderRadius / 2 : 0; // Köşeler için ek ofset

            _listView.Location = new Point(
                innerPadding + cornerOffset,
                _headerHeight + innerPadding + cornerOffset
            );

            _listView.Size = new Size(
                Width - (innerPadding * 2) - (cornerOffset * 2),
                Height - _headerHeight - (innerPadding * 2) - (cornerOffset * 2)
            );

            // Negatif boyutları engelle
            if (_listView.Width < 0) _listView.Width = 0;
            if (_listView.Height < 0) _listView.Height = 0;
        }

        // Sütun başlıklarını çiz
        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Başlık yüksekliğini sakla
            _columnHeaderActualHeight = e.Bounds.Height;

            // Arka planı çiz
            using (var brush = new SolidBrush(_columnHeaderBackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Metni çiz
            using (var brush = new SolidBrush(_columnHeaderTextColor))
            {
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    // Metin için biraz padding ekleyebiliriz
                    Rectangle textBounds = new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height);
                    e.Graphics.DrawString(e.Header.Text, e.Font, brush, textBounds, sf);
                }
            }

            // Sütun başlıkları arasına dikey çizgi çek
            if (e.ColumnIndex < _listView.Columns.Count - 1)
            {
                using (var pen = new Pen(_gridLinesColor))
                {
                    e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Y, e.Bounds.Right - 1, e.Bounds.Bottom);
                }
            }
            // Başlık altındaki yatay çizgiyi de burada çizebiliriz.
            using (var pen = new Pen(_gridLinesColor))
            {
                e.Graphics.DrawLine(pen, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        // ListView öğelerinin (satırların) arka planını çiz
        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Varsayılan arka planı çizdirmiyoruz, kendimiz dolduruyoruz.
            Color itemBackColor = e.Item.Selected ? _selectedItemBackColor : _itemBackColor;
            using (var brush = new SolidBrush(itemBackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Eğer ListView'ın görünüm modu "Details" değilse, metni burada çizmeliyiz.
            // "Details" modunda metin DrawSubItem içinde çizilir.
            if (_listView.View != View.Details)
            {
                Color itemTextColor = e.Item.Selected ? _selectedItemTextColor : e.Item.ForeColor;
                using (var brush = new SolidBrush(itemTextColor))
                {
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter; // Metin sığmazsa ... koyar
                    sf.FormatFlags = StringFormatFlags.NoWrap; // Metni sarmayı kapatır

                    Rectangle textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y, e.Bounds.Width - 10, e.Bounds.Height);
                    e.Graphics.DrawString(e.Item.Text, e.Item.Font, brush, textBounds, sf);
                }
            }

            // Odak dikdörtgenini çiz
            e.DrawFocusRectangle();
        }

        // Alt öğeleri (sütunlardaki hücreleri) çiz
        // Alt öğeleri (sütunlardaki hücreleri) çiz
        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // Arka planı çiz
            Color subItemBackColor = e.Item.Selected ? _selectedItemBackColor : _itemBackColor;
            using (var brush = new SolidBrush(subItemBackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Metni çiz
            Color subItemTextColor = e.Item.Selected ? _selectedItemTextColor : e.SubItem.ForeColor;
            using (var brush = new SolidBrush(subItemTextColor))
            {
                using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    // Sütun hizalamasına göre metni hizala
                    switch (e.Header.TextAlign)
                    {
                        case HorizontalAlignment.Left: sf.Alignment = StringAlignment.Near; break;
                        case HorizontalAlignment.Center: sf.Alignment = StringAlignment.Center; break;
                        case HorizontalAlignment.Right: sf.Alignment = StringAlignment.Far; break;
                    }

                    // Metin için biraz padding ekle
                    Rectangle textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y, e.Bounds.Width - 10, e.Bounds.Height);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, brush, textBounds, sf);
                }
            }

            // Çizgileri çiz:
            using (var pen = new Pen(_gridLinesColor))
            {
                // Dikey çizgi (her hücrenin sağ kenarı)
                e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Y, e.Bounds.Right - 1, e.Bounds.Bottom);

                // Yatay çizgi (her hücrenin alt kenarı)
                // Hata buradaydı: 'g' yerine 'e.Graphics' kullanılmalıydı.
                e.Graphics.DrawLine(pen, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Tüm çizimler için temel bir dikdörtgen belirleyelim, gölge payını göz önünde bulundurarak.
            Rectangle fullBounds = new Rectangle(
                _shadowDepth,
                _shadowDepth,
                Width - (_shadowDepth * 2) - 1,
                Height - (_shadowDepth * 2) - 1
            );

            if (fullBounds.Width <= 0 || fullBounds.Height <= 0) return;

            // 1. Ana arka planı çizin (tüm kontrolün gölge dahil ana şekli)
            using (var path = CreateRoundPath(fullBounds, _borderRadius))
            using (var brush = new SolidBrush(_backColor))
            {
                g.FillPath(brush, path);
            }

            // 2. Gölgeyi çizin (fullBounds içinde kalacak şekilde)
            DrawShadow(g);

            // 3. Başlık alanını çizin (sadece üst köşeler yuvarlak)
            Rectangle headerRect = new Rectangle(
                fullBounds.X,
                fullBounds.Y,
                fullBounds.Width + 1,
                _headerHeight
            );
            using (var path = CreateTopRoundPath(headerRect, _borderRadius))
            using (var brush = new SolidBrush(_headerBackColor))
            {
                g.FillPath(brush, path);
            }

            // 4. Başlık metnini çizin (KZ_ListView'ın kendi başlığı)
            using (var brush = new SolidBrush(_headerTextColor))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(_headerText, _headerFont, brush, headerRect, sf);
            }

            // 5. Kenarlığı çizin (ana şeklin etrafında)
            using (var path = CreateRoundPath(fullBounds, _borderRadius))
            using (var pen = new Pen(_borderColor, _borderSize))
            {
                g.DrawPath(pen, path);
            }

            // 6. Boş hücreleri çiz (DrawEmptyCells aktifse ve Detail görünümündeyse)
            if (_drawEmptyCells && _listView.View == View.Details && _listView.Columns.Count > 0)
            {
                // ListView'ın içindeki aktif çizim alanını alalım.
                Rectangle listViewClientArea = _listView.ClientRectangle;
                listViewClientArea.Offset(_listView.Location); // Parent (KZ_ListView) koordinatlarına çevir


                int actualColumnHeaderHeight = _columnHeaderActualHeight > 0 ? _columnHeaderActualHeight : (int)(_listView.Font.GetHeight() + 8);

                // Başlangıç Y koordinatı: Eğer öğeler varsa son öğeden sonra başla, yoksa başlık altından başla.
                int startY = _listView.Location.Y + actualColumnHeaderHeight;
                if (_listView.Items.Count > 0)
                {
                    // ListView'daki son öğenin alt sınırını al, KZ_ListView koordinatlarına çevir
                    startY = _listView.Items[_listView.Items.Count - 1].Bounds.Bottom + _listView.Location.Y;
                }

                int endY = _listView.Location.Y + _listView.Height; // ListView'ın alt sınırı

                // Varsayılan item yüksekliğini tahmin edelim.
                int itemHeight = (_listView.Items.Count > 0) ? _listView.Items[0].Bounds.Height : (int)(_listView.Font.GetHeight() + 8);
                if (itemHeight <= 0) itemHeight = 20; // Varsayılan minimum

                using (var gridPen = new Pen(_gridLinesColor))
                using (var cellBrush = new SolidBrush(_itemBackColor))
                {
                    // Sanal satırlar için döngü
                    for (int y = startY; y < endY; y += itemHeight)
                    {
                        int currentX = _listView.Location.X;
                        // Her sütun için döngü
                        foreach (ColumnHeader column in _listView.Columns)
                        {
                            Rectangle cellRect = new Rectangle(currentX, y, column.Width, itemHeight);

                            // Sadece KZ_ListView'ın çizim alanı içinde kalan kısımları çiz
                            Rectangle intersection = Rectangle.Intersect(cellRect, listViewClientArea);

                            if (intersection.Width > 0 && intersection.Height > 0)
                            {
                                // Hücre arka planını çiz
                                g.FillRectangle(cellBrush, intersection);

                                // Dikey çizgiyi çiz (sağ kenar)
                                g.DrawLine(gridPen, intersection.Right - 1, intersection.Y, intersection.Right - 1, intersection.Bottom);
                            }
                            currentX += column.Width;
                        }
                        // Her satır için yatay çizgi (alt kenar) - sadece ListView'ın genişliği kadar çiz
                        g.DrawLine(gridPen, _listView.Location.X, y + itemHeight - 1, _listView.Location.X + _listView.Width, y + itemHeight - 1);
                    }
                }
            }
        }

        private void DrawShadow(Graphics g)
        {
            if (_shadowDepth <= 0) return;

            Rectangle shadowBaseRect = new Rectangle(
                _shadowDepth,
                _shadowDepth,
                Width - (_shadowDepth * 2) - 1,
                Height - (_shadowDepth * 2) - 1
            );

            if (shadowBaseRect.Width <= 0 || shadowBaseRect.Height <= 0) return;

            for (int i = 0; i < _shadowDepth; i++)
            {
                // Gölge opaklığını kademeli olarak azalt
                int alpha = _shadowColor.A - (i * (_shadowColor.A / (_shadowDepth + 1)));
                if (alpha < 0) alpha = 0;
                using (var pen = new Pen(Color.FromArgb(alpha, _shadowColor), 1))
                {
                    Rectangle currentShadowRect = new Rectangle(
                        shadowBaseRect.X + i,
                        shadowBaseRect.Y + i,
                        shadowBaseRect.Width - (i * 2),
                        shadowBaseRect.Height - (i * 2)
                    );

                    if (currentShadowRect.Width <= 0 || currentShadowRect.Height <= 0) continue;

                    using (var path = CreateRoundPath(currentShadowRect, _borderRadius))
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
            UpdateListViewPosition();
            Invalidate();
        }
        #endregion

        #region Smart Tag
        // Bu sınıflar, KZ_ListView sınıfının doğrudan içinde yer almalıdır.
        public class KZ_ListViewDesigner : ControlDesigner
        {
            private DesignerActionListCollection _actionLists;

            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (_actionLists == null)
                    {
                        _actionLists = new DesignerActionListCollection();
                        _actionLists.Add(new KZ_ListViewActionList(Component));
                    }
                    return _actionLists;
                }
            }
        }

        public class KZ_ListViewActionList : DesignerActionList
        {
            private KZ_ListView _listView;

            public KZ_ListViewActionList(IComponent component) : base(component)
            {
                _listView = component as KZ_ListView;
            }

            public int BorderRadius
            {
                get => _listView.BorderRadius;
                set => SetProperty("BorderRadius", value);
            }

            public Color BorderColor
            {
                get => _listView.BorderColor;
                set => SetProperty("BorderColor", value);
            }

            public int BorderSize
            {
                get => _listView.BorderSize;
                set => SetProperty("BorderSize", value);
            }

            public int ShadowDepth
            {
                get => _listView.ShadowDepth;
                set => SetProperty("ShadowDepth", value);
            }

            public Color ShadowColor
            {
                get => _listView.ShadowColor;
                set => SetProperty("ShadowColor", value);
            }

            public Color HeaderBackColor
            {
                get => _listView.HeaderBackColor;
                set => SetProperty("HeaderBackColor", value);
            }

            public Color HeaderTextColor
            {
                get => _listView.HeaderTextColor;
                set => SetProperty("HeaderTextColor", value);
            }

            public string HeaderText
            {
                get => _listView.HeaderText;
                set => SetProperty("HeaderText", value);
            }

            public Font HeaderFont
            {
                get => _listView.HeaderFont;
                set => SetProperty("HeaderFont", value);
            }

            public int HeaderHeight
            {
                get => _listView.HeaderHeight;
                set => SetProperty("HeaderHeight", value);
            }

            public Color SelectedItemBackColor
            {
                get => _listView.SelectedItemBackColor;
                set => SetProperty("SelectedItemBackColor", value);
            }

            public Color SelectedItemTextColor
            {
                get => _listView.SelectedItemTextColor;
                set => SetProperty("SelectedItemTextColor", value);
            }

            public Color ItemBackColor
            {
                get => _listView.ItemBackColor;
                set => SetProperty("ItemBackColor", value);
            }

            public Color ColumnHeaderBackColor
            {
                get => _listView.ColumnHeaderBackColor;
                set => SetProperty("ColumnHeaderBackColor", value);
            }

            public Color ColumnHeaderTextColor
            {
                get => _listView.ColumnHeaderTextColor;
                set => SetProperty("ColumnHeaderTextColor", value);
            }

            public Color GridLinesColor
            {
                get => _listView.GridLinesColor;
                set => SetProperty("GridLinesColor", value);
            }

            public bool DrawEmptyCells
            {
                get => _listView.DrawEmptyCells;
                set => SetProperty("DrawEmptyCells", value);
            }

            public View View
            {
                get => _listView.View;
                set => SetProperty("View", value);
            }

            public bool FullRowSelect
            {
                get => _listView.FullRowSelect;
                set => SetProperty("FullRowSelect", value);
            }

            public ColumnHeaderStyle HeaderStyle
            {
                get => _listView.HeaderStyle;
                set => SetProperty("HeaderStyle", value);
            }

            public ListView.ListViewItemCollection Items
            {
                get => _listView.Items;
            }

            public ListView.ColumnHeaderCollection Columns
            {
                get => _listView.Columns;
            }

            public Color BackColor
            {
                get => _listView.BackColor;
                set => SetProperty("BackColor", value);
            }

            public Font Font
            {
                get => _listView.Font;
                set => SetProperty("Font", value);
            }

            public Color ForeColor
            {
                get => _listView.ForeColor;
                set => SetProperty("ForeColor", value);
            }


            private void SetProperty(string propName, object value)
            {
                PropertyDescriptor prop = TypeDescriptor.GetProperties(_listView)[propName];
                prop?.SetValue(_listView, value);
                _listView.Invalidate();
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
                items.Add(new DesignerActionPropertyItem("BackColor", "Ana Arkaplan Rengi", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("GridLinesColor", "Izgara Çizgi Rengi", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("DrawEmptyCells", "Boş Hücreleri Çiz", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("View", "Görünüm Modu", "Görünüm"));


                items.Add(new DesignerActionHeaderItem("Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderText", "Başlık Metni", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderBackColor", "Başlık Arkaplan", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderTextColor", "Başlık Yazı Rengi", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderFont", "Başlık Yazı Tipi", "Başlık"));
                items.Add(new DesignerActionPropertyItem("HeaderHeight", "Başlık Yüksekliği", "Başlık"));

                items.Add(new DesignerActionHeaderItem("Öğeler"));
                items.Add(new DesignerActionPropertyItem("ItemBackColor", "Öğe Arkaplan Rengi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("SelectedItemBackColor", "Seçili Öğe Arkaplan", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("SelectedItemTextColor", "Seçili Öğe Yazı Rengi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("Font", "Öğe Yazı Tipi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("ForeColor", "Öğe Yazı Rengi", "Öğeler"));
                items.Add(new DesignerActionPropertyItem("FullRowSelect", "Tüm Satırı Seç", "Öğeler"));


                items.Add(new DesignerActionHeaderItem("Sütunlar"));
                items.Add(new DesignerActionPropertyItem("ColumnHeaderBackColor", "Sütun Başlık Arkaplanı", "Sütunlar"));
                items.Add(new DesignerActionPropertyItem("ColumnHeaderTextColor", "Sütun Başlık Yazı Rengi", "Sütunlar"));
                items.Add(new DesignerActionPropertyItem("HeaderStyle", "Başlık Stili", "Sütunlar"));
                items.Add(new DesignerActionPropertyItem("Columns", "Sütunları Düzenle", "Sütunlar"));


                items.Add(new DesignerActionHeaderItem("Veri"));
                items.Add(new DesignerActionPropertyItem("Items", "Öğeleri Düzenle", "Veri"));


                return items;
            }
        }
        #endregion
    }
}