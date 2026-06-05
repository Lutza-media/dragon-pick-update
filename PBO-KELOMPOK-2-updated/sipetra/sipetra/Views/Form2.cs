// ============================================================
// Form2 - Halaman Registrasi (Daftar Akun Baru)
// File: Views/Form2.cs  ← GANTI ISI FILE LAMA
// ============================================================
using System;
using System.Windows.Forms;
using sipetra.Controllers;
using sipetra.Models;

namespace sipetra
{
    public partial class Form2 : Form
    {
        private readonly UserControllers _userController = new UserControllers();

        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // textBox1 = Nama, textBox2 = Email, textBox3 = Password
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Semua field harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var user = new User
                {
                    nama = textBox1.Text.Trim(),
                    email = textBox2.Text.Trim(),
                    password = textBox3.Text.Trim(),
                    isAdmin = false
                };

                _userController.RegisterUser(user);
                MessageBox.Show("Registrasi berhasil! Silakan login.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Kembali ke halaman login
                sipetra.Views.Form1 form1 = new sipetra.Views.Form1();
                form1.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mendaftar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Atur password char pada textBox3
            textBox3.PasswordChar = '●';
        }
    }
}
