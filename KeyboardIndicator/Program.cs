using System;
using System.Threading;
using System.Windows.Forms;
namespace KeyboardIndicator
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool flag;
            using (new Mutex(true, "KeyboardIndicator", out flag))
            {
                if (flag)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Form myform = new KeyboardIndicator();
                    Application.Run();

                }
            }
        }
    }
}
