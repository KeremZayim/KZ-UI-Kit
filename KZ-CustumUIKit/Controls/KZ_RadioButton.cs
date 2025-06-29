using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [ProvideProperty("KZ RadioButton", typeof(Control))]
    [Description("Modern tasarım ve stil seçenekleri sunan özel bir radio button kontrolü.")]

    public class KZ_RadioButton : Control
    {
        // Özellikler
        private bool isChecked = false;
        private Color checkedColor = Color.MediumSlateBlue;
        private Color uncheckedColor = Color.Gray;
        private Color innerCircleColor = Color.White;

        // Grup için statik liste
        private static List<KZ_RadioButton> radioButtonGroup = new List<KZ_RadioButton>();

        // Yapıcı
        public KZ_RadioButton()
        {
            this.Size = new Size(30, 30); // Varsayılan boyut
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular); // Modern bir font

            // Yeni buton ekleme
            radioButtonGroup.Add(this);
        }

        [Category("KZ Appearance")]
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    // Seçilen butonu işaretle
                    isChecked = value;
                    Invalidate();

                    // Diğer butonları kontrol et, yalnızca bir tanesi seçili olabilir
                    if (isChecked)
                    {
                        foreach (var radioButton in radioButtonGroup)
                        {
                            // Seçili olmayan tüm butonları işaretle
                            if (radioButton != this)
                            {
                                radioButton.Checked = false;
                            }
                        }
                    }
                }
            }
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

        // Mouse tıklama olayı
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // Eğer bu butona tıklanıyorsa seçili yap
            if (!isChecked)
            {
                Checked = true; // Seçili yap
            }
        }

        // Çizim işlemi
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dış çerçeve (yuvarlak)
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

            // İç yuvarlak çerçeve (daire)
            int innerRadius = this.Height - 8;
            int innerCircleX = Checked ? this.Width - innerRadius - 4 : 4;

            using (SolidBrush innerBrush = new SolidBrush(innerCircleColor))
            {
                g.FillEllipse(innerBrush, innerCircleX, 4, innerRadius, innerRadius);
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
