using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    public class KZ_RadioButton : Control
    {
        // Özellikler
        private bool isChecked = false;
        private Color checkedColor = Color.MediumSlateBlue;
        private Color uncheckedColor = Color.Gray;
        private Color innerCircleColor = Color.White;
        private int borderRadius = 10;

        // Yapıcı
        public KZ_RadioButton()
        {
            this.Size = new Size(30, 30); // Varsayılan boyut
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular); // Modern bir font
        }

        [Category("KZ Appearance")]
        public bool Checked
        {
            get { return isChecked; }
            set { isChecked = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color CheckedColor
        {
            get { return checkedColor; }
            set { checkedColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color UncheckedColor
        {
            get { return uncheckedColor; }
            set { uncheckedColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color InnerCircleColor
        {
            get { return innerCircleColor; }
            set { innerCircleColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        // Mouse tıklama olayı
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Checked = !Checked;
        }

        // Çizim işlemi
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dış çerçeve
            Rectangle outerRect = new Rectangle(0, 0, this.Height, this.Height); // Yuvarlak dış çerçeve
            using (SolidBrush brush = new SolidBrush(Checked ? checkedColor : uncheckedColor))
            {
                // Dış çerçeveyi yuvarlatılmış köşelerle çiziyoruz
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(outerRect);
                    g.FillPath(brush, path);
                }
            }

            // İç yuvarlak çerçeve
            int innerRadius = this.Height - 8;
            int innerCircleX = Checked ? this.Width - innerRadius - 4 : 4;

            using (SolidBrush innerBrush = new SolidBrush(innerCircleColor))
            {
                g.FillEllipse(innerBrush, innerCircleX, 4, innerRadius, innerRadius);
            }

            // Metni sağ tarafa hizala, dikeyde ortala
            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                string text = this.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    SizeF textSize = g.MeasureString(text, this.Font);
                    float textX = this.Height + 4; // Butonun sağ tarafına yerleştiriyoruz
                    float textY = (this.Height - textSize.Height) / 2; // Dikeyde ortalıyoruz
                    g.DrawString(text, this.Font, textBrush, textX, textY);
                }
            }
        }

        // Boyut değişimi olayları
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Boyutların orantılı olmasını sağla (Yuvarlak dairesel butonlar için)
            if (this.Width != this.Height)
            {
                this.Width = this.Height;
            }
        }
    }
}
