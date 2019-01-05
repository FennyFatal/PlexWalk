using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlexWalk
{
    public partial class Search : Form
    {
        public string query;
        public Search()
        {
            InitializeComponent();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            accept();
        }
        private void accept()
        {
            query = textBox1.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                accept();
        }
    }
}
