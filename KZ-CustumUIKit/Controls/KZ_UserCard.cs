using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design; // ImageEditor için

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [Designer(typeof(KZ_UserCard.KZ_UserCardDesigner))]
    [Description("Profil resmi, isim ve unvan içeren şık bir kullanıcı kartı bileşeni.")]
    public class KZ_UserCard : Control
    {
        #region Alanlar (Fields)

        private Image _profilePicture;
        private string _userName = "Kullanıcı Adı";
        private string _userRole = "Unvan / Rol";
        private string _description = "Kısa açıklama veya durum metni.";

        private Font _userNameFont = new Font("Segoe UI", 12f, FontStyle.Bold);
        private Color _userNameColor = Color.FromArgb(60, 60, 60); // Koyu Gri

        private Font _userRoleFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        private Color _userRoleColor = Color.FromArgb(100, 100, 100); // Açık Koyu Gri

        private Font _descriptionFont = new Font("Segoe UI", 8.5f, FontStyle.Italic);
        private Color _descriptionColor = Color.FromArgb(120, 120, 120); // Daha açık Gri

        private int _borderRadius = 15;
        private Color _backColor = Color.White;
        private Color _borderColor = Color.MediumSlateBlue; // Mor tonlu kenarlık
        private int _borderSize = 1;
        private int _shadowDepth = 5;
        private Color _shadowColor = Color.FromArgb(40, 0, 0, 0); // Hafif siyah gölge

        private int _pictureSize = 70; // Profil fotoğrafı boyutu
        private int _padding = 15; // İç boşluklar

        #endregion

        #region Yapıcı (Constructor)
        public KZ_UserCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(220, 150); // Varsayılan boyut
            MinimumSize = new Size(150, 100);
        }
        #endregion

        #region Özellikler (Properties)

        // --- KZ Veri Özellikleri ---
        [Category("KZ Veri")]
        [Description("Kullanıcı kartı için profil resmini ayarlar.")]
        [Browsable(true)]
        [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
        public Image ProfilePicture
        {
            get => _profilePicture;
            set
            {
                if (_profilePicture != value)
                {
                    _profilePicture?.Dispose();
                    if (value != null)
                    {
                        try { _profilePicture = new Bitmap(value); }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Profil resmi atanırken hata: {ex.Message}");
                            _profilePicture = null;
                        }
                    }
                    else { _profilePicture = null; }
                    Invalidate();
                }
            }
        }

        [Category("KZ Veri")]
        [Description("Kart üzerinde gösterilecek kullanıcı adını ayarlar.")]
        public string UserName
        {
            get => _userName;
            set { _userName = value; Invalidate(); }
        }

        [Category("KZ Veri")]
        [Description("Kart üzerinde gösterilecek kullanıcının unvanını veya rolünü ayarlar.")]
        public string UserRole
        {
            get => _userRole;
            set { _userRole = value; Invalidate(); }
        }

        [Category("KZ Veri")]
        [Description("Kullanıcı için isteğe bağlı bir açıklama veya durum metni ayarlar.")]
        public string Description
        {
            get => _description;
            set { _description = value; Invalidate(); }
        }

        // --- KZ Metin Stilleri ---
        [Category("KZ Metin Stilleri")]
        [Description("Kullanıcı adı için kullanılan yazı tipini ayarlar.")]
        public Font UserNameFont
        {
            get => _userNameFont;
            set { _userNameFont = value; Invalidate(); }
        }

        [Category("KZ Metin Stilleri")]
        [Description("Kullanıcı adının rengini ayarlar.")]
        public Color UserNameColor
        {
            get => _userNameColor;
            set { _userNameColor = value; Invalidate(); }
        }

        [Category("KZ Metin Stilleri")]
        [Description("Kullanıcı unvanı/rolü için kullanılan yazı tipini ayarlar.")]
        public Font UserRoleFont
        {
            get => _userRoleFont;
            set { _userRoleFont = value; Invalidate(); }
        }

        [Category("KZ Metin Stilleri")]
        [Description("Kullanıcı unvanı/rolünün rengini ayarlar.")]
        public Color UserRoleColor
        {
            get => _userRoleColor;
            set { _userRoleColor = value; Invalidate(); }
        }

        [Category("KZ Metin Stilleri")]
        [Description("Açıklama/durum metni için kullanılan yazı tipini ayarlar.")]
        public Font DescriptionFont
        {
            get => _descriptionFont;
            set { _descriptionFont = value; Invalidate(); }
        }

        [Category("KZ Metin Stilleri")]
        [Description("Açıklama/durum metninin rengini ayarlar.")]
        public Color DescriptionColor
        {
            get => _descriptionColor;
            set { _descriptionColor = value; Invalidate(); }
        }

        // --- KZ Görünüm ---
        [Category("KZ Görünüm")]
        [DefaultValue(15)]
        [Description("Kartın köşelerinin yuvarlaklık yarıçapını ayarlar.")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [Description("Kartın arka plan rengini ayarlar.")]
        public override Color BackColor
        {
            get => _backColor;
            set { _backColor = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [Description("Kartın kenarlık rengini ayarlar.")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(1)]
        [Description("Kartın kenarlık kalınlığını ayarlar.")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(5)]
        [Description("Kartın gölge efektinin derinliğini ayarlar.")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [Description("Kartın gölge rengini ayarlar.")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(70)]
        [Description("Profil resminin boyutunu (genişlik ve yükseklik) ayarlar.")]
        public int PictureSize
        {
            get => _pictureSize;
            set { _pictureSize = value; Invalidate(); }
        }

        [Category("KZ Görünüm")]
        [DefaultValue(15)]
        [Description("İçerik etrafındaki iç boşluk miktarını ayarlar.")]
        public int Padding
        {
            get => _padding;
            set { _padding = value; Invalidate(); }
        }

        // ForeColor'u gizli tutuyoruz, metin renkleri kendi property'leri üzerinden kontrol ediliyor
        [Browsable(false)]
        public override Color ForeColor { get => base.ForeColor; set => base.ForeColor = value; }

        #endregion

        #region Metotlar (Methods)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Arka planı temizle
            g.Clear(Parent?.BackColor ?? SystemColors.Control); // Parent'ın arka plan rengiyle temizle

            Rectangle cardRect = new Rectangle(
                _shadowDepth, // Gölge için boşluk bırak
                _shadowDepth, // Gölge için boşluk bırak
                Width - _shadowDepth * 2 - _borderSize, // Gölge ve kenarlık için boyutu ayarla
                Height - _shadowDepth * 2 - _borderSize);

            // 1. Gölgeyi Çiz
            DrawShadow(g, new Rectangle(
                _shadowDepth,
                _shadowDepth,
                Width - _shadowDepth * 2,
                Height - _shadowDepth * 2
            ));

            // 2. Kartın Arka Planını Çiz (Rounded)
            using (var path = CreateRoundPath(cardRect, _borderRadius))
            using (var brush = new SolidBrush(_backColor))
            {
                g.FillPath(brush, path);
            }

            // 3. Kartın Kenarlığını Çiz (Rounded)
            if (_borderSize > 0)
            {
                using (var path = CreateRoundPath(cardRect, _borderRadius))
                using (var pen = new Pen(_borderColor, _borderSize))
                {
                    g.DrawPath(pen, path);
                }
            }

            // 4. İçerikleri Çiz
            int currentY = _padding;

            // Profil Resmini Çiz
            RectangleF profilePicRect = new RectangleF(_padding, currentY, _pictureSize, _pictureSize);
            if (_profilePicture != null)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(profilePicRect);
                    g.SetClip(path); // Kırpma alanı ayarla (yuvarlak profil resmi için)
                    g.DrawImage(_profilePicture, profilePicRect);
                    g.ResetClip(); // Kırpma alanını sıfırla
                }
            }
            else
            {
                // Varsayılan boş profil resmi veya yer tutucu çiz
                g.FillEllipse(new SolidBrush(Color.LightGray), profilePicRect);
                g.DrawString("?", new Font("Arial", _pictureSize / 2, FontStyle.Bold), Brushes.DarkGray,
                    profilePicRect.X + profilePicRect.Width / 2 - g.MeasureString("?", new Font("Arial", _pictureSize / 2, FontStyle.Bold)).Width / 2,
                    profilePicRect.Y + profilePicRect.Height / 2 - g.MeasureString("?", new Font("Arial", _pictureSize / 2, FontStyle.Bold)).Height / 2);
            }

            // Metinlerin başlangıç X pozisyonu
            float textStartX = _padding + _pictureSize + _padding;

            // Kullanıcı Adını Çiz
            using (var nameBrush = new SolidBrush(_userNameColor))
            {
                g.DrawString(_userName, _userNameFont, nameBrush, textStartX, currentY + (_pictureSize - _userNameFont.Height - _userRoleFont.Height - _descriptionFont.Height) / 2);
            }
            currentY += _userNameFont.Height;

            // Kullanıcı Unvanını/Rolünü Çiz
            using (var roleBrush = new SolidBrush(_userRoleColor))
            {
                g.DrawString(_userRole, _userRoleFont, roleBrush, textStartX, currentY + (_pictureSize - _userNameFont.Height - _userRoleFont.Height - _descriptionFont.Height) / 2 + 5); // Hafif boşluk
            }
            currentY += _userRoleFont.Height;

            // Açıklama Metnini Çiz (profil resminin altında, ortalanmış)
            RectangleF descriptionRect = new RectangleF(
                _padding,
                _padding + _pictureSize + _padding / 2, // Profil resminin altından başla
                Width - _padding * 2,
                Height - (_padding + _pictureSize + _padding / 2) - _padding); // Kalan alanı kapla

            using (var descBrush = new SolidBrush(_descriptionColor))
            using (var stringFormat = new StringFormat())
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Near;
                stringFormat.Trimming = StringTrimming.EllipsisWord;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap; // Tek satırda kalması için

                // Eğer açıklama metni tek satırda sığmazsa, metni sığdıracak şekilde ayarla
                SizeF textSize = g.MeasureString(_description, _descriptionFont, (int)descriptionRect.Width, stringFormat);
                if (textSize.Width > descriptionRect.Width)
                {
                    // Metni sığdırmak için LineAlignment'ı değiştirip çok satır destekle
                    stringFormat.FormatFlags = 0; // NoWrap'ı kaldır
                    stringFormat.LineAlignment = StringAlignment.Center; // Dikeyde ortala
                }


                g.DrawString(_description, _descriptionFont, descBrush, descriptionRect, stringFormat);
            }
        }

        private void DrawShadow(Graphics g, Rectangle rect)
        {
            if (_shadowDepth <= 0 || DesignMode) return;

            for (int i = 0; i < _shadowDepth; i++)
            {
                using (var path = CreateRoundPath(new Rectangle(rect.X + i, rect.Y + i, rect.Width - i * 2, rect.Height - i * 2), _borderRadius))
                using (var pen = new Pen(Color.FromArgb(12 * (_shadowDepth - i), _shadowColor), 1))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _profilePicture?.Dispose();
                _userNameFont?.Dispose();
                _userRoleFont?.Dispose();
                _descriptionFont?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Akıllı Etiket (Smart Tag)
        public class KZ_UserCardDesigner : ControlDesigner
        {
            private DesignerActionListCollection _actionLists;

            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (_actionLists == null)
                    {
                        _actionLists = new DesignerActionListCollection();
                        _actionLists.Add(new KZ_UserCardActionList(Component, "KZ Kullanıcı Kartı Özellikleri"));
                    }
                    return _actionLists;
                }
            }
        }

        public class KZ_UserCardActionList : DesignerActionList
        {
            private KZ_UserCard _userCard;
            private string _smartTagDisplayName;

            public KZ_UserCardActionList(IComponent component, string smartTagDisplayName) : base(component)
            {
                _userCard = component as KZ_UserCard;
                _smartTagDisplayName = smartTagDisplayName;
            }

            public override int GetHashCode()
            {
                return _smartTagDisplayName?.GetHashCode() ?? base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is KZ_UserCardActionList other)
                {
                    return string.Equals(_smartTagDisplayName, other._smartTagDisplayName);
                }
                return false;
            }

            public override string ToString()
            {
                return _smartTagDisplayName ?? base.ToString();
            }

            private void SetProperty(string propName, object value)
            {
                PropertyDescriptor prop = TypeDescriptor.GetProperties(_userCard)[propName];
                prop?.SetValue(_userCard, value);
                _userCard.Invalidate();
            }

            // --- KZ Veri Özellikleri (Akıllı Etiket) ---
            [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
            public Image ProfilePicture { get => _userCard.ProfilePicture; set => SetProperty("ProfilePicture", value); }
            public string UserName { get => _userCard.UserName; set => SetProperty("UserName", value); }
            public string UserRole { get => _userCard.UserRole; set => SetProperty("UserRole", value); }
            public string Description { get => _userCard.Description; set => SetProperty("Description", value); }

            // --- KZ Metin Stilleri Özellikleri (Akıllı Etiket) ---
            public Font UserNameFont { get => _userCard.UserNameFont; set => SetProperty("UserNameFont", value); }
            public Color UserNameColor { get => _userCard.UserNameColor; set => SetProperty("UserNameColor", value); }
            public Font UserRoleFont { get => _userCard.UserRoleFont; set => SetProperty("UserRoleFont", value); }
            public Color UserRoleColor { get => _userCard.UserRoleColor; set => SetProperty("UserRoleColor", value); }
            public Font DescriptionFont { get => _userCard.DescriptionFont; set => SetProperty("DescriptionFont", value); }
            public Color DescriptionColor { get => _userCard.DescriptionColor; set => SetProperty("DescriptionColor", value); }

            // --- KZ Görünüm Özellikleri (Akıllı Etiket) ---
            public int BorderRadius { get => _userCard.BorderRadius; set => SetProperty("BorderRadius", value); }
            public Color BackColor { get => _userCard.BackColor; set => SetProperty("BackColor", value); }
            public Color BorderColor { get => _userCard.BorderColor; set => SetProperty("BorderColor", value); }
            public int BorderSize { get => _userCard.BorderSize; set => SetProperty("BorderSize", value); }
            public int ShadowDepth { get => _userCard.ShadowDepth; set => SetProperty("ShadowDepth", value); }
            public Color ShadowColor { get => _userCard.ShadowColor; set => SetProperty("ShadowColor", value); }
            public int PictureSize { get => _userCard.PictureSize; set => SetProperty("PictureSize", value); }
            public int Padding { get => _userCard.Padding; set => SetProperty("Padding", value); }


            public override DesignerActionItemCollection GetSortedActionItems()
            {
                var items = new DesignerActionItemCollection();

                items.Add(new DesignerActionHeaderItem("Veri"));
                items.Add(new DesignerActionPropertyItem("ProfilePicture", "Profil Resmi:", "Veri"));
                items.Add(new DesignerActionPropertyItem("UserName", "Kullanıcı Adı:", "Veri"));
                items.Add(new DesignerActionPropertyItem("UserRole", "Unvan / Rol:", "Veri"));
                items.Add(new DesignerActionPropertyItem("Description", "Açıklama:", "Veri"));

                items.Add(new DesignerActionHeaderItem("Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("UserNameFont", "Ad Yazı Tipi:", "Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("UserNameColor", "Ad Rengi:", "Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("UserRoleFont", "Unvan Yazı Tipi:", "Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("UserRoleColor", "Unvan Rengi:", "Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("DescriptionFont", "Açıklama Yazı Tipi:", "Metin Stilleri"));
                items.Add(new DesignerActionPropertyItem("DescriptionColor", "Açıklama Rengi:", "Metin Stilleri"));

                items.Add(new DesignerActionHeaderItem("Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderRadius", "Kenarlık Yuvarlaklığı:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BackColor", "Arka Plan Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderColor", "Kenarlık Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("BorderSize", "Kenarlık Kalınlığı:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowDepth", "Gölge Derinliği:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("ShadowColor", "Gölge Rengi:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("PictureSize", "Resim Boyutu:", "Görünüm"));
                items.Add(new DesignerActionPropertyItem("Padding", "İç Boşluk:", "Görünüm"));

                return items;
            }
        }
        #endregion
    }
}