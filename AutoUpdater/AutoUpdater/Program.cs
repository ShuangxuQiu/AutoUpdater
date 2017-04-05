using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpdater
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //判断程序是否已经在运行中
            bool isRunning;
            Mutex mutex = new Mutex(true, "OnlyRunOneInstance", out isRunning);
            if(isRunning)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //判断是否是管理员，如果不是，请求管理员权限
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                //WindowsIdentity identity = new WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                //判断是否是管理员登录
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Application.Run(new MainForm());
                }
                //如果不是，创建启动对象
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = Application.ExecutablePath;
                    startInfo.Verb = "runas";
                    try
                    {
                        Process.Start(startInfo);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("升级程序遇到错误：" + e.ToString(),"错误信息",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        return;
                    }
                    Application.Exit();
                }
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("升级程序已经启动！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }         
        }
    }
}
