using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class frmMenu : Form
    {
        private string ReturnCode;

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            this.Text = Program.strTitle;
            label1.Text = Program.strItem[0];
            label2.Text = Program.strItem[1];
            btnFrom.Text = Program.strItem[2];
            btnTo.Text = Program.strItem[3];

            if (Program.emflag)
            {
                label2.Text = Program.strItem[22];
                btnEm.Text = Program.strItem[23];
                btnFrom.Visible = false;
                btnTo.Visible = false;
                btnEm.Visible = true;
            }

            if ((Environment.OSVersion.Version.Major <= 6) && (Environment.OSVersion.Version.Minor <= 1))
            {
                btnTo.Enabled = false;
                Program.user = "user";
                Program.psswd = "pass";
                Program.psswd = "pass";
            }
            else
            {
                Program.user = "user";
                Program.psswd = "pass";
                Program.psswd = "pass";
            }
        }

        private void btnFrom_Click(object sender, EventArgs e)
        {
            this.Hide();
            new frmFrom().ShowDialog();
            this.Close();
        }

        private void btnTo_Click(object sender, EventArgs e)
        {
            this.Hide();
            new frmTo().ShowDialog();
            this.Close();
        }

        private void btnEm_Click(object sender, EventArgs e)
        {
            this.TopMost = false;

            DialogResult Result;

            if (Program.strNormalMsg[1] != "")
            {
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
                    ReturnCode = Program.Runas(Program.currentdir + Program.strCommand[0], "Disable Force 10", Program.user, Program.psswd);
                    switch (ReturnCode)
                    {
                        case "0":
                            {
                                break;
                            }
                        case "1":
                            {
                                errorProvider1.Clear();
                                if (Program.strNormalMsg[10] != "")
                                {
                                    MessageBox.Show(Program.strNormalMsg[10],
                                                    Program.strTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                break;
                            }
                        default:
                            {
                                errorProvider1.SetError(btnEm, Program.strErrorMsg[3] + "[" + ReturnCode.ToString() + "]");
                                if (Program.strErrorMsg[3] != "")
                                {
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
    }
}
