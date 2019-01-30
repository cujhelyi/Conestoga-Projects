using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGUI_2
{
    public partial class CustomMsg : Form
    {
        public CustomMsg()
        {
            InitializeComponent();
        }
        static CustomMsg MsgBox;
        static DialogResult result = DialogResult.No;
        public static DialogResult Show(string text)
        {
            MsgBox = new CustomMsg();
            MsgBox.lblMsg.Text = text;
            MsgBox.ShowDialog();
            return result;
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            result = DialogResult.OK;
            MsgBox.Close();
        }
    }
}
