using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JASP.Forms
{
    public partial class Errors : Form
    {
        public Errors()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void SetFirstLine(int line)
        {
            this.firstline = line;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        public void SelectFirstLine()
        {
            SelectLine(firstline);
            textBox1.Text = firstline.ToString();
            listBox1.SetSelected(0, true);
            Application.Run(this);
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string line = this.listBox1.SelectedItem.ToString();
            line = line.Substring(5, line.IndexOf(':') - 5);
            this.textBox1.Text = line;
            this.SelectLine(Convert.ToInt32(line));
        }
    }
}
