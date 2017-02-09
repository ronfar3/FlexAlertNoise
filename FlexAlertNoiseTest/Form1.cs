using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlexAlert;

namespace FlexAlertNoiseTest
{
    public partial class Form1 : Form
    {
        FlexAlertNoise flexAlertNoise = null;

        public Form1()
        {
            InitializeComponent();
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            button3.Click += new EventHandler(button3_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flexAlertNoise = new FlexAlertNoise(Convert.ToInt32(textBox2.Text), Convert.ToUInt32(textBox3.Text), checkBox1.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flexAlertNoise.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                flexAlertNoise.Play();
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
            }
        }

    }
}
