using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace PlexWalk
{
    public partial class Login : Form
    {
        public ICredentials creds;
        public string headerAuth;
        
        public Login()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            acceptCreds();
        }
        private void acceptCreds()
        {
            creds = new NetworkCredential(username.Text, password.Text);
            headerAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes(username.Text + ":" + password.Text));
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void field_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode ==Keys.Enter)
                acceptCreds();
        }
    }
}
