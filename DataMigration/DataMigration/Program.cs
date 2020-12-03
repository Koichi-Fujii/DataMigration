using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DataMigration
{
    public static class Program
    {
        public static string user;
        public static string psswd;
        public static string currentdir = Application.StartupPath + "\\"; //AppDomain.CurrentDomain.BaseDirectory;
        public static string strTitle = "データ移行ツール/Data migration tool";
        public static bool emflag = false;

        public static string[] strItem = new string[25];
        public static string[] strNormalMsg = new string[15];
        public static string[] strErrorMsg = new string[15];
        public static string[] strCommand = new string[2];
        public static string[] strHash = new string[2];

        public static bool Checkhash(string file, string hash)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] bs = sha256.ComputeHash(fs);
            sha256.Clear();
            fs.Close();

            StringBuilder result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }

            if (result.ToString() != hash)
            {
                MessageBox.Show(Program.strErrorMsg[5],
                                Program.strTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string Runas(string file, string arg, string user, string psswd)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            Process p = new Process();

            psi.CreateNoWindow = true;
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = file;
            psi.Arguments = arg;
            
            if (user != "")
            {
                psi.UseShellExecute = false;
                psi.Verb = "runas";
                psi.UserName = user;
                
                SecureString pw = new SecureString();
                foreach (char Char in psswd.ToCharArray())
                {
                    pw.AppendChar(Char);
                }
                psi.Password = pw;
            }

            try
            {
                p = Process.Start(psi);
                p.WaitForExit();
                return p.ExitCode.ToString();
            }
            catch
            {
                return "99";
            }
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isFirstInstance;
            using (new Mutex(false, "MUTEX: YOUR_MUTEX_NAME", out isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    return;
                }

                string xmlfile = currentdir + Application.ProductName + ".xml";

                try
                {    
                    XElement xml = XElement.Load(xmlfile);
                    IEnumerable<XElement> s = from item in xml.Elements("Settings") select item;

                    foreach (XElement item in s)
                    {
                        strTitle = item.Element("title").Value;
                        for (int i = 0; i < 25; i += 1)
                        {
                            strItem[i] = item.Element("item" + i).Value;
                        }
                        for (int i = 0; i < 15; i += 1)
                        {
                            strNormalMsg[i] = (item.Element("normalmsg" + i).Value).Replace("\\n", "\r\n");
                            strErrorMsg[i] = (item.Element("errormsg" + i).Value).Replace("\\n", "\r\n");
                        }
                        for (int i = 0; i < 2; i += 1)
                        {
                            strCommand[i] = (item.Element("command" + i).Value).Replace("\\n", "\r\n");
                            strHash[i] = (item.Element("sha" + i).Value).Replace("\\n", "\r\n");
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("設定ファイルを読み込めません\nUnable to read configuration file.",
                                    Program.strTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    return;
                }

                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline)
                {
                    if (SystemInformation.PowerStatus.BatteryLifePercent <= 0.75)
                    {
                        MessageBox.Show(Program.strErrorMsg[0],
                                        Program.strTitle,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    emflag = true;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMenu());
            }
        }
    }
}
