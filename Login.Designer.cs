﻿namespace PlexWalk
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.xmltext2 = new System.Windows.Forms.Label();
            this.xmlurl = new System.Windows.Forms.TextBox();
            this.xmlbox = new System.Windows.Forms.CheckBox();
            this.xmltext1 = new System.Windows.Forms.Label();
            this.lblPW = new System.Windows.Forms.Label();
            this.lblUN = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // xmltext2
            // 
            this.xmltext2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmltext2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(140)))), ((int)(((byte)(139)))));
            this.xmltext2.Location = new System.Drawing.Point(-4, 306);
            this.xmltext2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.xmltext2.Name = "xmltext2";
            this.xmltext2.Size = new System.Drawing.Size(204, 16);
            this.xmltext2.TabIndex = 20;
            this.xmltext2.Text = "What\'s the URL?";
            this.xmltext2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.xmltext2.Visible = false;
            // 
            // xmlurl
            // 
            this.xmlurl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(101)))), ((int)(((byte)(104)))));
            this.xmlurl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xmlurl.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlurl.ForeColor = System.Drawing.Color.White;
            this.xmlurl.Location = new System.Drawing.Point(0, 348);
            this.xmlurl.Margin = new System.Windows.Forms.Padding(4);
            this.xmlurl.Name = "xmlurl";
            this.xmlurl.Size = new System.Drawing.Size(192, 29);
            this.xmlurl.TabIndex = 19;
            this.xmlurl.Visible = false;
            this.xmlurl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.field_KeyUp);
            // 
            // xmlbox
            // 
            this.xmlbox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.xmlbox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlbox.Location = new System.Drawing.Point(0, 324);
            this.xmlbox.Margin = new System.Windows.Forms.Padding(4);
            this.xmlbox.Name = "xmlbox";
            this.xmlbox.Size = new System.Drawing.Size(192, 30);
            this.xmlbox.TabIndex = 18;
            this.xmlbox.UseVisualStyleBackColor = true;
            this.xmlbox.Click += new System.EventHandler(this.xmlbox_Checked);
            // 
            // xmltext1
            // 
            this.xmltext1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmltext1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(140)))), ((int)(((byte)(139)))));
            this.xmltext1.Location = new System.Drawing.Point(-5, 306);
            this.xmltext1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.xmltext1.Name = "xmltext1";
            this.xmltext1.Size = new System.Drawing.Size(204, 16);
            this.xmltext1.TabIndex = 16;
            this.xmltext1.Text = "Load URL?";
            this.xmltext1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPW
            // 
            this.lblPW.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(140)))), ((int)(((byte)(139)))));
            this.lblPW.Location = new System.Drawing.Point(0, 215);
            this.lblPW.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPW.Name = "lblPW";
            this.lblPW.Size = new System.Drawing.Size(204, 16);
            this.lblPW.TabIndex = 15;
            this.lblPW.Text = "Password";
            this.lblPW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUN
            // 
            this.lblUN.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(140)))), ((int)(((byte)(139)))));
            this.lblUN.Location = new System.Drawing.Point(0, 167);
            this.lblUN.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUN.Name = "lblUN";
            this.lblUN.Size = new System.Drawing.Size(204, 16);
            this.lblUN.TabIndex = 14;
            this.lblUN.Text = "User Name";
            this.lblUN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // password
            // 
            this.password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(101)))), ((int)(((byte)(104)))));
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password.ForeColor = System.Drawing.Color.White;
            this.password.Location = new System.Drawing.Point(0, 235);
            this.password.Margin = new System.Windows.Forms.Padding(4);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(192, 29);
            this.password.TabIndex = 12;
            this.password.KeyUp += new System.Windows.Forms.KeyEventHandler(this.field_KeyUp);
            // 
            // username
            // 
            this.username.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(101)))), ((int)(((byte)(104)))));
            this.username.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.username.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username.ForeColor = System.Drawing.Color.White;
            this.username.Location = new System.Drawing.Point(0, 187);
            this.username.Margin = new System.Windows.Forms.Padding(4);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(192, 29);
            this.username.TabIndex = 11;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::PlexWalk.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(-3, -1);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(192, 165);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(123)))), ((int)(((byte)(25)))));
            this.btnLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.Black;
            this.btnLogin.Image = global::PlexWalk.Properties.Resources.login;
            this.btnLogin.Location = new System.Drawing.Point(-1, 377);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(195, 28);
            this.btnLogin.TabIndex = 13;
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(80, 285);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(29, 22);
            this.checkBox1.TabIndex = 22;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(140)))), ((int)(((byte)(139)))));
            this.label1.Location = new System.Drawing.Point(13, 264);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 16);
            this.label1.TabIndex = 21;
            this.label1.Text = "Remember me?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.ClientSize = new System.Drawing.Size(192, 404);
            this.Controls.Add(this.xmlurl);
            this.Controls.Add(this.xmlbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblPW);
            this.Controls.Add(this.lblUN);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.xmltext1);
            this.Controls.Add(this.xmltext2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login to Plex";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label xmltext2;
        private System.Windows.Forms.TextBox xmlurl;
        private System.Windows.Forms.CheckBox xmlbox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label xmltext1;
        private System.Windows.Forms.Label lblPW;
        private System.Windows.Forms.Label lblUN;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
    }
}