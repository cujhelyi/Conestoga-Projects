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
    public partial class Form2 : Form
    {
        public Form2(bool player)
        {
            InitializeComponent();
            if (player)
            {
                pictureBox1.Image = Properties.Resources.Pieces_White_Queen;
                pictureBox2.Image = Properties.Resources.Pieces_White_Rook;
                pictureBox3.Image = Properties.Resources.Pieces_White_Bishop;
                pictureBox4.Image = Properties.Resources.Pieces_White_Knight;
            }
            else
            {
                pictureBox1.Image = Properties.Resources.Pieces_Black_Queen;
                pictureBox2.Image = Properties.Resources.Pieces_Black_Rook;
                pictureBox3.Image = Properties.Resources.Pieces_Black_Bishop;
                pictureBox4.Image = Properties.Resources.Pieces_Black_Knight;
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
