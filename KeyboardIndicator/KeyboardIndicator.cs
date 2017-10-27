using KeyboardIndicator.Properties;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace KeyboardIndicator
{
    public partial class KeyboardIndicator : Form
    {
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 256;

        private const int NUM_KEYCODE = 144;//Num Lock键：VK_NUMLOCK (144)
        private const int CAPS_KEYCODE = 20;//Caps Lock键：VK_CAPITAL (20)

        private KeyboardIndicator.KeyBoardHookStruct kbh;
        private KeyboardIndicator.HookProc gHookProc;

        private int curCount;
        private bool isRunning;

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, KeyboardIndicator.HookProc lpfn, IntPtr hInstance, int threadId);

        private bool showNumLock = false;
        private bool showCapsLock = false;

        public KeyboardIndicator()
        {
            InitializeComponent();

            try
            {
                showNumLock = ConfigurationManager.AppSettings["NumLock"].ToUpper() == "Y";
            }
            catch (Exception) 
            {
                showNumLock = false;
            }

            try
            {
                showCapsLock = ConfigurationManager.AppSettings["CapsLock"].ToUpper() == "Y";
            }
            catch (Exception) 
            {
                showCapsLock = false;
            }

            if (!showNumLock && !showCapsLock)
            {
                showNumLock = true;
            }

            this.SetStatus();
            this.gHookProc = new KeyboardIndicator.HookProc(this.KeyBoardHookProc);
            KeyboardIndicator.SetWindowsHookEx(WH_KEYBOARD_LL, this.gHookProc, IntPtr.Zero, 0);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
            this.Dispose(true);
            Application.ExitThread();
        }

        public int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            this.kbh = (KeyboardIndicator.KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardIndicator.KeyBoardHookStruct));
            if (!this.isRunning && (this.kbh.vkCode == NUM_KEYCODE || this.kbh.vkCode == CAPS_KEYCODE))
            {
                ThreadPool.QueueUserWorkItem(delegate(object param0)
                {
                    this.isRunning = true;
                    this.curCount = 200;
                    while (this.curCount > 0)
                    {
                        this.SetStatus();
                        Thread.Sleep(20);
                        this.curCount--;
                    }
                    this.curCount = 0;
                    this.isRunning = false;
                });
            }
            return 0;
        }

        private void SetStatus()
        {
            this.notifyIconNUM.Visible = showNumLock;
            this.notifyIconCAPS.Visible = showCapsLock;

            if (showNumLock)
            {
                if (KeyboardIndicator.GetKeyState(NUM_KEYCODE) != 0)
                {
                    this.notifyIconNUM.Icon = Resources.NumLockOpen;
                    this.notifyIconNUM.Text = "NumLock On";
                }
                else
                {
                    this.notifyIconNUM.Icon = Resources.NumLockClose;
                    this.notifyIconNUM.Text = "NumLock Off";
                }
            }

            if (showCapsLock)
            {
                if (KeyboardIndicator.GetKeyState(CAPS_KEYCODE) != 0)
                {
                    this.notifyIconCAPS.Icon = Resources.CapsLockOpen;
                    this.notifyIconCAPS.Text = "CapsLock On";
                }
                else
                {
                    this.notifyIconCAPS.Icon = Resources.CapsLockClose;
                    this.notifyIconCAPS.Text = "CapsLock Off";
                }
            }
        }

    }
}
