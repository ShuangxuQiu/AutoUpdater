using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using FSLib.App.SimpleUpdater;

namespace AutoUpdater
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            setLabelInfo();
            toolTip1.SetToolTip(this.btnCheckUpdate, "开始更新后将会关闭平台，请先保存您的文件");
        }
        private void setLabelInfo()
        {
            string strName = null;
            ArrayList al = getVersionInfo();
            lblVerno.Text = getCurrentVersion().ToString();
            switch (al[0].ToString())
            {
                case "Math":
                    strName = "初中数学学科平台";
                    break;
                case "Math_Office":
                    strName = "初中数学office平台";
                    break;
                case "Chem":
                    strName = "初中化学学科平台";
                    break;
                case "Chem_Office":
                    strName = "初中化学office平台";
                    break;
                case "Bio":
                    strName = "初中生物学科平台";
                    break;
                case "Bio_Office":
                    strName = "初中生物office平台";
                    break;
                default:
                    strName = "初中教学平台";
                    break;                 
            }
            lblSoftwarename.Text = strName;
        }
        private void setupUpdater()
        {
            ArrayList ar = getVersionInfo();
            string updateUrl = ar[2].ToString();
            var updater = Updater.Instance;
            updater.Context.CurrentVersion = getCurrentVersion();
            updater.Context.UpdateInfoFileName = "update_c.xml";
            updater.Context.UpdateDownloadUrl = updateUrl + "{0}";
            updater.Context.AutoEndProcessesWithinAppDir = true;
            updater.Context.EnableEmbedDialog = true;

            //处理更新检测的事件            
            updater.NoUpdatesFound += (s, e) =>
            {
                MessageBox.Show("暂时没有可用的更新!", "更新提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                Application.Exit();
            };
            updater.UpdatesFound += (s, e) =>
            {
                ArrayList al = getVersionInfo();
                string platformName = al[0].ToString();
                if (platformName == "Math" || platformName == "Chem"|| platformName == "Bio")
                {
                    if (MessageBox.Show("检测到新版本" + updater.Context.UpdateInfo.AppVersion + "。\n开始更新将关闭平台，请先保存您的文件。", "更新信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    if (MessageBox.Show("检测到新版本" + updater.Context.UpdateInfo.AppVersion + "。\n请您在更新完成后重启office以获取最新体验。", "更新信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }          
            };
            updater.Error += (s, e) =>
            {
                MessageBox.Show("更新发生了错误：" + updater.Context.Exception.Message, "错误提示");
                System.Environment.Exit(0);
            };
            //开始更新，捕获更新异常，强制退出
            updater.EnsureNoUpdate();           
            //updater.BeginCheckUpdateInProcess();
            Application.Exit();          
        }

        //获取当前版本号，返回一个version对象
        private Version getCurrentVersion()
        {
            string strVersion;
            Version version = new Version();
            try
            {
                strVersion = getVersionInfo()[1].ToString();
                version = new Version(Convert.ToInt32(strVersion.Substring(0, 1)), Convert.ToInt32(strVersion.Substring(2, 1)), Convert.ToInt32(strVersion.Substring(4, 1)));
            }
            catch(Exception e)
            {
                MessageBox.Show("读取版本信息出错！", "错误提示");
                version = new Version(0, 0, 0);
            }     
            return version;
        }
        //获取版本信息，返回一个ArrayList
        private ArrayList getVersionInfo()
        {
            ArrayList str = new ArrayList();
            string currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string root = System.IO.Path.GetDirectoryName(currentAssembly);
            string platformName = null;  //用于保存平台名称
            string updateUrl = null;  //对应的更新路径
            string version = null;
            List<string> lines = new List<string>();
            try
            {
                FileStream fs = new FileStream(root + "\\version.txt", FileMode.Open);
                StreamReader rd = new StreamReader(fs);
                string s;
                //读入文件所有行，存放到List<string>集合中
                while ((s = rd.ReadLine()) != null)
                {
                    lines.Add(s);
                }
                rd.Close();
                fs.Close();
                platformName = lines[0].Substring(9).Trim();
                version = lines[1].Substring(8).Trim();
                updateUrl = lines[2].Substring(10).Trim() + platformName + "/";
                str.Add(platformName);
                str.Add(version);
                str.Add(updateUrl);
            }
            catch (Exception e)
            {
                MessageBox.Show("读取版本信息文件出错！", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                //Application.Exit();      
                System.Environment.Exit(0);
            }            
            return str;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            setupUpdater();
        }
    }
}
