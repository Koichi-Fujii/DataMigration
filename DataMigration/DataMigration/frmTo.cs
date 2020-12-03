using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class frmTo : Form
    {
        private bool bolButtonClose = false;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, uint wParam, IntPtr lParam);

        private const uint WM_USER = 0x400;
        private const uint PBM_SETSTATE = WM_USER + 16;
        private const uint PBST_NORMAL = 0x0001;
        private const uint PBST_ERROR = 0x0002;
        private const uint PBST_PAUSED = 0x0003;

        private Thread thread;
        delegate void Delegate();

        public frmTo()
        {
            InitializeComponent();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            DialogResult Result;

            if (Program.strNormalMsg[0] != "")
            {
                textBox1.Text = Program.strNormalMsg[0];
                Result = MessageBox.Show(Program.strNormalMsg[0],
                                Program.strTitle,
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2);
            }
            else
            {
                Result = DialogResult.OK;
            }

            if (Result == DialogResult.OK)
            {
                if (Program.Checkhash(Program.currentdir + Program.strCommand[0], Program.strHash[0]))
                {
                    File.WriteAllText(Program.currentdir + "uac.status", "[0]", Encoding.ASCII);
                    string ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[0], "Enable 192.168.100.101 10", Program.user, Program.psswd);
                    switch (ReturnCode)
                    {
                        case "0":
                            {
                                
                                break;
                            }
                        case "1":
                            {
                                thread = new Thread(new ThreadStart(Method));
                                thread.Start();
                                
                                errorProvider1.Clear();
                                pictureBox2.BackgroundImage = DataMigration.Properties.Resources.onebit_34;
                                pictureBox2.Visible = true;
                                btnUninit.Enabled = true;
                                btnBack.Enabled = false;
                                btnBack.Text = Program.strItem[11];
                                btnInit.Enabled = false;
                                label3.Text = Program.strItem[21];
                                label3.Visible = true;
                                progressBar1.Visible = true;
                                if (Program.strNormalMsg[6] != "")
                                {
                                    textBox1.Text = Program.strNormalMsg[6];
                                    MessageBox.Show(Program.strNormalMsg[6],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                break;
                            }
                        default:
                            {
                                errorProvider1.SetError(btnInit, Program.strErrorMsg[2] + "[" + ReturnCode.ToString()+ "]");
                                pictureBox2.BackgroundImage = DataMigration.Properties.Resources.onebit_33;
                                pictureBox2.Visible = true;
                                btnUninit.Enabled = false;
                                if (Program.strErrorMsg[2] != "")
                                {
                                    textBox1.Text = Program.strErrorMsg[2] + "[" + ReturnCode.ToString() + "]";
                                    MessageBox.Show(Program.strErrorMsg[2],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Exclamation);
                                }
                                break;
                            }
                    }
                }
            }
            else
            {
                textBox1.Text = Program.strNormalMsg[9];
            }
        }

        void Success()
        {
            label3.Text = Program.strItem[12];
            SendMessage(new HandleRef(progressBar1, progressBar1.Handle), PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 100;
            if (!pictureBox3.Visible)
            {
                textBox1.Text = Program.strNormalMsg[5]; 
                btnUninit.Enabled = true;
            }
        }

        void Transfer()
        {
            label3.Text = Program.strItem[13];
            SendMessage(new HandleRef(progressBar1, progressBar1.Handle), PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Value = 0;
            if (!pictureBox3.Visible)
            {
                textBox1.Text = Program.strNormalMsg[8]; 
                btnUninit.Enabled = false;
            }
        }

        void Error()
        {
            label3.Text = Program.strItem[14];
            SendMessage(new HandleRef(progressBar1, progressBar1.Handle), PBM_SETSTATE, PBST_ERROR, IntPtr.Zero);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 100;
            if (!pictureBox3.Visible)
            {
                textBox1.Text = Program.strErrorMsg[6]; 
                btnUninit.Enabled = true;
            }
        }

        void Method()
        {
            do
            {
                if (System.IO.File.Exists(Program.strItem[15]))
                {
                    Invoke(new Delegate(Success));
                }
                if (System.IO.File.Exists(Program.strItem[16]))
                {
                    Invoke(new Delegate(Transfer));
                }
                if (System.IO.File.Exists(Program.strItem[17]))
                {
                    Invoke(new Delegate(Error));
                }
                Thread.Sleep(500);
            } while (true);
        }

        private void btnUninit_Click(object sender, EventArgs e)
        {
            DialogResult Result;

            if (Program.strNormalMsg[1] != "")
            {
                textBox1.Text = Program.strNormalMsg[1];
                Result = MessageBox.Show(Program.strNormalMsg[1],
                            Program.strTitle,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2);
            }
            else
            {
                Result = DialogResult.OK;
            }

            if (Result == DialogResult.OK)
            {
                if (Program.Checkhash(Program.currentdir + Program.strCommand[0], Program.strHash[0]))
                {
                    File.WriteAllText(Program.currentdir + "uac.status", "[0]", Encoding.ASCII);
                    string ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[0], "Disable 192.168.100.101 10", Program.user, Program.psswd);
                    switch (ReturnCode)
                    {
                        case "0":
                            {
                                break;
                            }
                        case "1":
                            {
                                thread.Abort();
                                errorProvider1.Clear();
                                pictureBox3.BackgroundImage = DataMigration.Properties.Resources.onebit_34;
                                pictureBox3.Visible = true;
                                btnUninit.Enabled = false;
                                btnBack.Enabled = true;
                                if (Program.strNormalMsg[7] != "")
                                {
                                    textBox1.Text = Program.strNormalMsg[7];
                                    MessageBox.Show(Program.strNormalMsg[7],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                break;
                            }
                        default:
                            {
                                errorProvider1.SetError(btnUninit, Program.strErrorMsg[3] + "[" + ReturnCode.ToString() + "]");
                                pictureBox3.BackgroundImage = DataMigration.Properties.Resources.onebit_33;
                                pictureBox3.Visible = true;
                                if (Program.strErrorMsg[3] != "")
                                {
                                    textBox1.Text = Program.strErrorMsg[3] + "[" + ReturnCode.ToString() + "]";
                                    MessageBox.Show(Program.strErrorMsg[3],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Exclamation);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            bolButtonClose = true;

            if (btnBack.Text == Program.strItem[10])
            {
                this.Hide();
                new frmMenu().ShowDialog();
                this.Close();
            }
            else
            {
                thread.Abort();
                this.Close();
            }
        }

        private void frmTo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (bolButtonClose == false)
                {
                    e.Cancel = true;
                }
            }
        }

        private void frmTo_Load(object sender, EventArgs e)
        {
            this.Text = Program.strTitle;
            label1.Text = Program.strItem[18];
            label2.Text = Program.strItem[19];
            btnInit.Text = Program.strItem[6];
            btnUninit.Text = Program.strItem[8];
            btnBack.Text = Program.strItem[10];
            textBox1.Text = Program.strNormalMsg[9];
        }
    }
}
