using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    [ToolboxItem(true)]
    [Description("Açık/Kapalı durumlarını görsel bir anahtar şeklinde temsil eden özel toggle switch bileşeni.")]
    [ProvideProperty("KZ ToggleSwitch", typeof(Control))]

    public class KZ_ToggleSwitch : Control
    {
        // Özellikler
        private bool isChecked = false;
        private Color onBackColor = Color.MediumSlateBlue;
        private Color offBackColor = Color.Gray;
        private Color onToggleColor = Color.WhiteSmoke;
        private Color offToggleColor = Color.Gainsboro;
        private float togglePosition = 2f; // Toggle başlangıç pozisyonu
        private Timer transitionTimer;

        // Yapıcı
        public KZ_ToggleSwitch()
        {
            this.Size = new Size(36, 18); // Başlangıç boyutları
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;

            // Geçiş animasyonu için Timer başlatma
            transitionTimer = new Timer();
            transitionTimer.Interval = 10; // Geçiş süresi (animasyon hızını belirler)
            transitionTimer.Tick += TransitionTimer_Tick;

            // Mouse tıklama olayını tanımla
            this.Click += ToggleSwitch_Click;
        }

        // Checked özelliği
        [Category("KZ Appearance")]
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    transitionTimer.Start(); // Geçiş animasyonunu başlat
                }
            }
        }

        [Category("KZ Appearance")]
        public Color OnBackColor
        {
            get { return onBackColor; }
            set { onBackColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color OffBackColor
        {
            get { return offBackColor; }
            set { offBackColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color OnToggleColor
        {
            get { return onToggleColor; }
            set { onToggleColor = value; Invalidate(); }
        }

        [Category("KZ Appearance")]
        public Color OffToggleColor
        {
            get { return offToggleColor; }
            set { offToggleColor = value; Invalidate(); }
        }

        // Boyutlandırma işlemi
        public override Size MinimumSize
        {
            get { return new Size(20, 10); } // Minimum boyut, 2:1 oranını korur
        }

        public override Size MaximumSize
        {
            get { return new Size(1000, 500); } // Maksimum boyut, sınırları yok
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Boyutların 2:1 oranında olduğundan emin ol
            if (this.Width != this.Height * 2)
            {
                this.Width = this.Height * 2;
            }
        }

        // Geçiş animasyonunun her adımında yapılacaklar
        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            float targetPosition = isChecked ? this.Width - this.Height + 2 : 2;
            if (Math.Abs(togglePosition - targetPosition) > 1)
            {
                // Kayma efektini sağlayacak şekilde pozisyonu değiştiriyoruz
                togglePosition += (targetPosition - togglePosition) / 6;
                Invalidate(); // Yeniden çizim
            }
            else
            {
                // Hedef pozisyona ulaşıldığında animasyonu durduruyoruz
                togglePosition = targetPosition;
                transitionTimer.Stop();
            }
        }

        // Mouse tıklama olayında durum değiştirme
        private void ToggleSwitch_Click(object sender, EventArgs e)
        {
            // Durumun değişmesini sağla
            Checked = !Checked;
        }

        // Çizim işlemi
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Arkaplan çizimi
            Rectangle controlRect = this.ClientRectangle;
            int toggleSize = controlRect.Height - 5;

            g.Clear(this.Parent.BackColor);

            // Radius değerini hesapla (2:1 oranında)
            int baseWidth = 80; // Başlangıç genişliği
            int baseHeight = 40; // Başlangıç yüksekliği
            int baseRadius = 20; // Başlangıç radius değeri

            float widthRatio = (float)this.Width / baseWidth;  // Yeni genişlik oranı
            float heightRatio = (float)this.Height / baseHeight; // Yeni yükseklik oranı
            float ratio = Math.Min(widthRatio, heightRatio); // En küçük oranı al (en iyi uyumu sağlamak için)

            int radius = (int)(baseRadius * ratio); // Radius'u orantılı olarak ayarla

            // Arkaplan renk
            Color backColor = isChecked ? onBackColor : offBackColor;
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, radius * 2, radius * 2, 180, 90); // Sol üst köşe
                    path.AddArc(controlRect.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90); // Sağ üst köşe
                    path.AddArc(controlRect.Width - radius * 2, controlRect.Height - radius * 2, radius * 2, radius * 2, 0, 90); // Sağ alt köşe
                    path.AddArc(0, controlRect.Height - radius * 2, radius * 2, radius * 2, 90, 90); // Sol alt köşe
                    path.CloseAllFigures();

                    // Arkaplanı yuvarlak şekilde çiziyoruz
                    g.FillPath(brush, path);
                }
            }

            // Toggle yuvarlağı renk ve konum
            Color toggleColor = isChecked ? onToggleColor : offToggleColor;

            using (SolidBrush brush = new SolidBrush(toggleColor))
            {
                g.FillEllipse(brush, togglePosition, 2, toggleSize, toggleSize);
            }
        }
    }
}
