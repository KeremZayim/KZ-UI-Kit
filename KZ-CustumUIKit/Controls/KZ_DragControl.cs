using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KZ_CustumUIKit.Controls
{
    [ProvideProperty("KZ TargetControl", typeof(Control))]
    public class KZ_DragControl : Component
    {
        private Control targetControl;

        [Category("KZ Appearance")]
        [Description("Drag işlemi uygulanacak kontrol.")]
        public Control TargetControl
        {
            get { return targetControl; }
            set
            {
                if (targetControl != null)
                {
                    targetControl.MouseDown -= TargetControl_MouseDown;
                }

                targetControl = value;

                if (targetControl != null)
                {
                    targetControl.MouseDown += TargetControl_MouseDown;
                }
            }
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void TargetControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (targetControl != null && targetControl.FindForm() != null)
            {
                ReleaseCapture();
                SendMessage(targetControl.FindForm().Handle, 0x112, 0xf012, 0);
            }
        }
    }
}
