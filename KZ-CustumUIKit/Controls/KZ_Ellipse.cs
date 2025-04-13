using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    public class KZ_RadiusControl : Component
    {
        // Özellikler
        private Control targetControl;
        private int radius = 5;

        // Constructor
        public KZ_RadiusControl()
        {
        }

        // Target Control
        [Category("KZ Appearance")]
        public Control TargetControl
        {
            get { return targetControl; }
            set
            {
                targetControl = value;
                ApplyRadius(); // Yeni target kontrolü seçildiğinde radius uygula
            }
        }

        // Radius
        [Category("KZ Appearance")]
        public int Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                if (targetControl != null)
                    ApplyRadius(); // Radius değeri değiştiğinde uygula
            }
        }

        // Radius Uygulama
        private void ApplyRadius()
        {
            if (targetControl == null)
                return;

            // Eğer target form ise, başlık çubuğu None olmalı
            if (targetControl is Form form)
            {
                form.FormBorderStyle = FormBorderStyle.None;
            }

            // Target kontrolün köşelerini yuvarlat
            targetControl.Region = new Region(CreateRoundedRectangle(targetControl.ClientRectangle, radius));
        }

        // Yuvarlak dikdörtgen oluşturma (köşeler için)
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
