// ============================================================
// Form3 - Dashboard Utama (setelah login)
// File: Views/Form3.cs
// Navigasi ke semua form lainnya
// ============================================================
using System;
using System.Windows.Forms;
using sipetra.Models;
using sipetra.Views;

namespace sipetra.Views
{
    public partial class Form3 : Form
    {
        private User _userLogin;

        public Form3()
        {
            InitializeComponent();
        }

        public Form3(User user) : this()
        {
            _userLogin = user;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Kamu bisa gunakan ini untuk menampilkan nama user di header/label
            // Contoh: lblWelcome.Text = $"Selamat datang, {_userLogin?.nama}!";
        }

        // button1 = Beranda (halaman ini sendiri / refresh)
        // button2 = Riwayat Transaksi (Lutza)
        // button3 = Profil (Dini)
        // button4 = Logout

        private void button2_Click(object sender, EventArgs e)
        {
            // Riwayat Transaksi (LUTZA)
            if (_userLogin == null) return;
            var form = new FormRiwayatTransaksi(_userLogin.id);
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Profil (DINI)
            if (_userLogin == null) return;
            var form = new FormProfil(_userLogin);
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Logout
            var konfirmasi = MessageBox.Show("Yakin ingin keluar?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (konfirmasi == DialogResult.Yes)
            {
                sipetra.Helpers.UserSession.Instance.ClearSession();
                Form1 loginForm = new Form1();
                loginForm.Show();
                this.Close();
            }
        }

        // Metode publik untuk dipanggil dari form lain
        public void NavigasiKePemesanan(Tiket tiket)
        {
            var form = new FormPemesanan(tiket, _userLogin);
            form.ShowDialog();
        }
    }
}
