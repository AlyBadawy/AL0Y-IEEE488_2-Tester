using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AL0Y_IEEE488_2_Tester
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text =  IEEE488Bus.read("26");
            
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            freqLabel.Text = "LOOOOOL";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
        }

        private void label48_Click(object sender, EventArgs e)
        {

        }
    }
}
