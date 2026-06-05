using sipetra.Controllers;
using sipetra.Models;
using sipetra.Helpers;
using sipetra.Views;
using System;
using System.Windows.Forms;

namespace sipetra.Views
{
    public partial class Form1 : Form
    {
        UserControllers userController = new UserControllers();
        public Form1()
        {
            InitializeComponent();
        }

        private bool ValidateLoginInput()
        {
            if (string.IsNullOrWhiteSpace(tbEmail.Text) || 
                string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                MessageBox.Show("Email dan Password tidak boleh kosong.");
                return false;
            }
            return true;
        }

        private User GetLoginRequest()
        {
            return new User
            {
                
                email = tbEmail.Text.Trim(),
                password = tbPassword.Text.Trim()
            };
        }

        private bool HandleLoginResult(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Login gagal. Email atau Password salah.");
                return false;
            }
            UserSession.Instance.SetUser(user);
            MessageBox.Show($"Login berhasil. Selamat datang, {user.nama}!");
            return true;
        }

         private void RedirectAfterLogin(User user) {

            Form next = new sipetra.Views.Form3(user);
            next.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginInput())
                return;

            var req = GetLoginRequest();
            var result = userController.Login(req);

            if (!HandleLoginResult(result))
                return;

            RedirectAfterLogin(result);
        }

        private void btnDaftar_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}
