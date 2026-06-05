namespace sipetra.Views
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tbEmail = new TextBox();
            tbPassword = new TextBox();
            btnLogin = new Button();
            label1 = new Label();
            btnDaftar = new Button();
            SuspendLayout();
            // 
            // tbEmail
            // 
            tbEmail.BorderStyle = BorderStyle.None;
            tbEmail.Location = new Point(871, 485);
            tbEmail.Name = "tbEmail";
            tbEmail.Size = new Size(520, 24);
            tbEmail.TabIndex = 2;
            // 
            // tbPassword
            // 
            tbPassword.BorderStyle = BorderStyle.None;
            tbPassword.Location = new Point(871, 635);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new Size(520, 24);
            tbPassword.TabIndex = 3;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = SystemColors.ButtonHighlight;
            btnLogin.BackgroundImage = (Image)resources.GetObject("btnLogin.BackgroundImage");
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.ForeColor = SystemColors.Desktop;
            btnLogin.Location = new Point(855, 735);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(560, 74);
            btnLogin.TabIndex = 5;
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(997, 836);
            label1.Name = "label1";
            label1.Size = new Size(166, 25);
            label1.TabIndex = 6;
            label1.Text = "Belum punya akun?";
            // 
            // btnDaftar
            // 
            btnDaftar.BackColor = SystemColors.ButtonHighlight;
            btnDaftar.FlatAppearance.BorderSize = 0;
            btnDaftar.FlatStyle = FlatStyle.Flat;
            btnDaftar.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            btnDaftar.ForeColor = Color.Purple;
            btnDaftar.Location = new Point(1158, 834);
            btnDaftar.Name = "btnDaftar";
            btnDaftar.Size = new Size(124, 34);
            btnDaftar.TabIndex = 8;
            btnDaftar.Text = "Daftar di sini";
            btnDaftar.UseVisualStyleBackColor = false;
            btnDaftar.Click += btnDaftar_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1505, 982);
            Controls.Add(btnDaftar);
            Controls.Add(label1);
            Controls.Add(btnLogin);
            Controls.Add(tbPassword);
            Controls.Add(tbEmail);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox tbEmail;
        private TextBox tbPassword;
        private Button btnLogin;
        private Label label1;
        private Button btnDaftar;
    }
}
