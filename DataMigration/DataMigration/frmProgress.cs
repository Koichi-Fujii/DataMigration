using System;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class frmProgress : Form
    {
        public frmProgress()
        {
            InitializeComponent();
        }

        private void frmProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }

        private void frmProgress_Load(object sender, EventArgs e)
        {
            this.Text = Program.strTitle;
            label1.Text = Program.strItem[13];
            label2.Text = Program.strItem[20];
        }
    }
}
