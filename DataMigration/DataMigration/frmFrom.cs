using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class frmFrom : Form
    {
        private Form frmProgress = new frmProgress();
        private bool bolButtonClose = false;

        private string ReturnCode;
        private Thread thread;
        delegate void Delegate();

        public frmFrom()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            byte[] buffer = new byte[65500];

            var pingReply = ping.Send("192.168.100.101", 500, buffer, new PingOptions(600, false));
            if (pingReply.Status == IPStatus.Success)
            {
                Process.Start("explorer.exe", "\\\\192.168.100.101\\C\\Users\\Public\\Downloads");
            }
            else
            {
                textBox1.Text = Program.strErrorMsg[1];
                MessageBox.Show(Program.strErrorMsg[1],
                                Program.strTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
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
                this.Close();
            }
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
                    ReturnCode = "";
                    ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[0], "Enable 192.168.100.100", Program.user, Program.psswd);
                    switch (ReturnCode)
                    {
                        case "0":
                            {
                                break;
                            }
                        case "1":
                            {
                                errorProvider1.Clear();
                                pictureBox2.BackgroundImage = DataMigration.Properties.Resources.onebit_34;
                                pictureBox2.Visible = true;
                                btnTrans.Enabled = true;
                                btnUninit.Enabled = true;
                                btnOpen.Enabled = true;
                                btnBack.Enabled = false;
                                btnBack.Text = Program.strItem[11];
                                btnInit.Enabled = false;
                                if (Program.strNormalMsg[3] != "")
                                {
                                    textBox1.Text = Program.strNormalMsg[3];
                                    MessageBox.Show(Program.strNormalMsg[3],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                break;
                            }
                        default:
                            {
                                errorProvider1.SetError(btnInit, Program.strErrorMsg[2] + "[" + ReturnCode.ToString() + "]");
                                pictureBox2.BackgroundImage = DataMigration.Properties.Resources.onebit_33;
                                pictureBox2.Visible = true;
                                btnTrans.Enabled = false;
                                btnUninit.Enabled = false;
                                btnOpen.Enabled = false;
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
            frmProgress.Hide();
            errorProvider1.Clear();
            pictureBox3.BackgroundImage = DataMigration.Properties.Resources.onebit_34;
            pictureBox3.Visible = true;
            btnUninit.Enabled = true;
            if (Program.strNormalMsg[5] != "")
            {
                textBox1.Text = Program.strNormalMsg[5];
                MessageBox.Show(Program.strNormalMsg[5],
                                Program.strTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        void Error()
        {
            frmProgress.Hide();
            errorProvider1.SetError(btnTrans, Program.strErrorMsg[4] + "[" + ReturnCode.ToString() + "]");
            pictureBox3.BackgroundImage = DataMigration.Properties.Resources.onebit_33;
            pictureBox3.Visible = true;
            btnUninit.Enabled = true;
            if (Program.strErrorMsg[4] != "")
            {
                textBox1.Text = Program.strErrorMsg[4] + "[" + ReturnCode.ToString() + "]";
                MessageBox.Show(Program.strErrorMsg[4],
                                Program.strTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        void Method()
        {
            ReturnCode = "";
            ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[1], "", "", "");
            switch (ReturnCode)
            {
                case "1":
                    {
                        Invoke(new Delegate(Success));
                        break;
                    }
                default:
                    {
                        Invoke(new Delegate(Error));
                        break;
                    }
            }
        }

        private void btnTrans_Click(object sender, EventArgs e)
        {
            DialogResult Result;

            if (Program.strNormalMsg[2] != "")
            {
                textBox1.Text = Program.strNormalMsg[2];
                Result = MessageBox.Show(Program.strNormalMsg[2],
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
                Ping ping = new Ping();
                byte[] buffer = new byte[65500];

                var pingReply = ping.Send("192.168.100.101", 500, buffer, new PingOptions(600, false));
                if (pingReply.Status == IPStatus.Success)
                {
                    if (Program.Checkhash(Program.currentdir + Program.strCommand[1], Program.strHash[1]))
                    {
                        thread = new Thread(new ThreadStart(Method));
                        thread.Start();

                        frmProgress.ShowDialog();
                    }
                }
                else
                {
                    textBox1.Text = Program.strErrorMsg[1];
                    MessageBox.Show(Program.strErrorMsg[1],
                                    Program.strTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
            }
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
                    ReturnCode = "";
                    ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[0], "Disable", Program.user, Program.psswd);
                    switch (ReturnCode)
                    {
                        case "0":
                            {
                                break;
                            }
                        case "1":
                            {
                                errorProvider1.Clear();
                                pictureBox4.BackgroundImage = DataMigration.Properties.Resources.onebit_34;
                                pictureBox4.Visible = true;
                                btnTrans.Enabled = false;
                                btnUninit.Enabled = false;
                                btnOpen.Enabled = false;
                                btnBack.Enabled = true;
                                if (Program.strNormalMsg[4] != "")
                                {
                                    textBox1.Text = Program.strNormalMsg[4];
                                    MessageBox.Show(Program.strNormalMsg[4],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                break;
                            }
                        default:
                            {
                                errorProvider1.SetError(btnUninit, Program.strErrorMsg[3] + "[" + ReturnCode.ToString() + "]");
                                pictureBox4.BackgroundImage = DataMigration.Properties.Resources.onebit_33;
                                pictureBox4.Visible = true;
                                btnTrans.Enabled = false;
                                btnOpen.Enabled = false;
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

        private void frmFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (bolButtonClose == false)
                {
                    e.Cancel = true;
                }
            }
        }

        private void frmFrom_Load(object sender, EventArgs e)
        {
            this.Text = Program.strTitle;
            label1.Text = Program.strItem[4];
            label2.Text = Program.strItem[5];
            btnInit.Text = Program.strItem[6];
            btnTrans.Text = Program.strItem[7];
            btnUninit.Text = Program.strItem[8];
            btnOpen.Text = Program.strItem[9];
            btnBack.Text = Program.strItem[10];
            textBox1.Text = Program.strNormalMsg[9];
        }
    }
}
