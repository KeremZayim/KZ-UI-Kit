using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [ProvideProperty("KZ GradientPanel", typeof(Control))]
    [Designer(typeof(KZ_GradientPanelDesigner))]
    [Description("Arka planı degrade (gradient) renklerle boyayan, görsel açıdan zenginleştirilmiş özel bir panel bileşeni.")]
    public class KZ_GradientPanel : Panel
    {
        // Default colors
        private Color _topLeftColor = Color.FromArgb(0, 150, 199);
        private Color _topRightColor = Color.FromArgb(0, 180, 216);
        private Color _bottomLeftColor = Color.FromArgb(144, 224, 239);
        private Color _bottomRightColor = Color.FromArgb(202, 240, 248);
        private int _radius = 10;

        [Category("KZ Appearance")]
        public Color TopLeftColor
        {
            get => _topLeftColor;
            set { _topLeftColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color TopRightColor
        {
            get => _topRightColor;
            set { _topRightColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color BottomLeftColor
        {
            get => _bottomLeftColor;
            set { _bottomLeftColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color BottomRightColor
        {
            get => _bottomRightColor;
            set { _bottomRightColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public int Radius
        {
            get => _radius;
            set { _radius = Math.Max(0, value); Invalidate(); }
        }

        public KZ_GradientPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.UserPaint |
                   ControlStyles.ResizeRedraw |
                   ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rect = ClientRectangle;

            using (GraphicsPath path = CreateRoundedPath(rect, _radius))
            {
                // Create a texture brush for perfect corner-to-corner gradients
                using (Bitmap gradientTexture = CreateGradientTexture(rect.Width, rect.Height))
                using (TextureBrush brush = new TextureBrush(gradientTexture, WrapMode.Clamp))
                {
                    g.FillPath(brush, path);
                }

                // Optional: Draw border
                using (Pen borderPen = new Pen(Color.FromArgb(80, Color.Black), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        private Bitmap CreateGradientTexture(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Create four gradient brushes for each edge
                using (LinearGradientBrush topBrush = new LinearGradientBrush(
                    new Point(0, 0), new Point(width, 0), _topLeftColor, _topRightColor))
                using (LinearGradientBrush bottomBrush = new LinearGradientBrush(
                    new Point(0, 0), new Point(width, 0), _bottomLeftColor, _bottomRightColor))
                using (LinearGradientBrush leftBrush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, height), _topLeftColor, _bottomLeftColor))
                using (LinearGradientBrush rightBrush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, height), _topRightColor, _bottomRightColor))
                {
                    // Fill top and bottom edges
                    g.FillRectangle(topBrush, 0, 0, width, 1);
                    g.FillRectangle(bottomBrush, 0, height - 1, width, 1);

                    // Fill left and right edges
                    g.FillRectangle(leftBrush, 0, 0, 1, height);
                    g.FillRectangle(rightBrush, width - 1, 0, 1, height);

                    // Interpolate the middle pixels
                    for (int y = 1; y < height - 1; y++)
                    {
                        float verticalRatio = y / (float)(height - 1);

                        for (int x = 1; x < width - 1; x++)
                        {
                            float horizontalRatio = x / (float)(width - 1);

                            // Interpolate between all four corners
                            Color color = InterpolateColors(
                                _topLeftColor, _topRightColor,
                                _bottomLeftColor, _bottomRightColor,
                                horizontalRatio, verticalRatio);

                            bmp.SetPixel(x, y, color);
                        }
                    }
                }
            }

            return bmp;
        }

        private Color InterpolateColors(Color topLeft, Color topRight,
                                      Color bottomLeft, Color bottomRight,
                                      float xRatio, float yRatio)
        {
            // Horizontal interpolation for top and bottom edges
            Color top = InterpolateColor(topLeft, topRight, xRatio);
            Color bottom = InterpolateColor(bottomLeft, bottomRight, xRatio);

            // Vertical interpolation between top and bottom
            return InterpolateColor(top, bottom, yRatio);
        }

        private Color InterpolateColor(Color start, Color end, float ratio)
        {
            int r = (int)(start.R + (end.R - start.R) * ratio);
            int g = (int)(start.G + (end.G - start.G) * ratio);
            int b = (int)(start.B + (end.B - start.B) * ratio);
            int a = (int)(start.A + (end.A - start.A) * ratio);

            return Color.FromArgb(a, r, g, b);
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            radius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90); // Top-left
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90); // Top-right
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90); // Bottom-right
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90); // Bottom-left
            path.CloseFigure();

            return path;
        }

        // Smart Tag properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GradientPanelSettings PanelSettings
        {
            get => new GradientPanelSettings(this);
            set
            {
                if (value != null)
                {
                    this.TopLeftColor = value.TopLeftColor;
                    this.TopRightColor = value.TopRightColor;
                    this.BottomLeftColor = value.BottomLeftColor;
                    this.BottomRightColor = value.BottomRightColor;
                    this.Radius = value.Radius;
                }
            }
        }
    }

    // Smart Tag settings class
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GradientPanelSettings
    {
        private KZ_GradientPanel _panel;

        public GradientPanelSettings(KZ_GradientPanel panel)
        {
            _panel = panel;
        }

        [Description("Sol üst köşe rengi")]
        public Color TopLeftColor
        {
            get => _panel.TopLeftColor;
            set => _panel.TopLeftColor = value;
        }

        [Description("Sağ üst köşe rengi")]
        public Color TopRightColor
        {
            get => _panel.TopRightColor;
            set => _panel.TopRightColor = value;
        }

        [Description("Sol alt köşe rengi")]
        public Color BottomLeftColor
        {
            get => _panel.BottomLeftColor;
            set => _panel.BottomLeftColor = value;
        }

        [Description("Sağ alt köşe rengi")]
        public Color BottomRightColor
        {
            get => _panel.BottomRightColor;
            set => _panel.BottomRightColor = value;
        }

        [Description("Köşe yuvarlaklık yarıçapı")]
        public int Radius
        {
            get => _panel.Radius;
            set => _panel.Radius = value;
        }

        public override string ToString() => "Panel Ayarları";
    }

    // Designer class for Smart Tag
    public class KZ_GradientPanelDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new KZ_GradientPanelActionList(Component));
                }
                return _actionLists;
            }
        }
    }

    // Smart Tag action list
    public class KZ_GradientPanelActionList : DesignerActionList
    {
        private KZ_GradientPanel _panel;

        public KZ_GradientPanelActionList(IComponent component) : base(component)
        {
            _panel = component as KZ_GradientPanel;
        }

        public Color TopLeftColor
        {
            get => _panel.TopLeftColor;
            set
            {
                SetProperty("TopLeftColor", value);
                _panel.Invalidate();
            }
        }

        public Color TopRightColor
        {
            get => _panel.TopRightColor;
            set
            {
                SetProperty("TopRightColor", value);
                _panel.Invalidate();
            }
        }

        public Color BottomLeftColor
        {
            get => _panel.BottomLeftColor;
            set
            {
                SetProperty("BottomLeftColor", value);
                _panel.Invalidate();
            }
        }

        public Color BottomRightColor
        {
            get => _panel.BottomRightColor;
            set
            {
                SetProperty("BottomRightColor", value);
                _panel.Invalidate();
            }
        }

        public int Radius
        {
            get => _panel.Radius;
            set
            {
                SetProperty("Radius", value);
                _panel.Invalidate();
            }
        }

        private void SetProperty(string propName, object value)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(_panel)[propName];
            prop?.SetValue(_panel, value);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Renk Ayarları"));
            items.Add(new DesignerActionPropertyItem("TopLeftColor", "Sol Üst Renk:", "Renk Ayarları"));
            items.Add(new DesignerActionPropertyItem("TopRightColor", "Sağ Üst Renk:", "Renk Ayarları"));
            items.Add(new DesignerActionPropertyItem("BottomLeftColor", "Sol Alt Renk:", "Renk Ayarları"));
            items.Add(new DesignerActionPropertyItem("BottomRightColor", "Sağ Alt Renk:", "Renk Ayarları"));

            items.Add(new DesignerActionHeaderItem("Şekil Ayarları"));
            items.Add(new DesignerActionPropertyItem("Radius", "Köşe Yuvarlaklık:", "Şekil Ayarları"));

            return items;
        }
    }
}