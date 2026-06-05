// ============================================================
// DINI - Halaman Profil User
// File: Views/FormProfil.cs
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormProfil : Form
    {
        private readonly UserContext _userContext = new UserContext();
        private User _user;

        public FormProfil(User user)
        {
            _user = user;
            InitializeComponent();
            TampilkanProfil();
        }

        private void TampilkanProfil()
        {
            lblNamaProfil.Text = _user.nama;
            lblEmailProfil.Text = _user.email;
            lblRoleProfil.Text = _user.isAdmin ? "Administrator" : "Pengguna";
            lblRoleProfil.ForeColor = _user.isAdmin ? Color.DarkRed : Color.DarkGreen;

            // Inisial avatar
            lblAvatar.Text = _user.nama.Length > 0 ? _user.nama[0].ToString().ToUpper() : "?";
        }

        private void btnEditProfil_Click(object sender, EventArgs e)
        {
            var editForm = new FormEditProfil(_user);
            editForm.ShowDialog();
            // Reload data user
            var updated = _userContext.GetUserById(_user.id);
            if (updated != null)
            {
                _user = updated;
                UserSession.Instance.SetUser(updated);
                TampilkanProfil();
            }
        }

        private void btnRiwayat_Click(object sender, EventArgs e)
        {
            var riwayatForm = new FormRiwayatTransaksi(_user.id);
            riwayatForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var konfirmasi = MessageBox.Show("Yakin ingin keluar?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (konfirmasi == DialogResult.Yes)
            {
                UserSession.Instance.ClearSession();
                this.Close();
                // Kembali ke form login
                new Form1().Show();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Profil Saya";
            this.Size = new Size(450, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.FromArgb(102, 51, 153) };

            // Avatar lingkaran
            var pnlAvatar = new Panel { Size = new Size(80, 80), BackColor = Color.White, Location = new Point(185, 20) };
            pnlAvatar.Paint += (s, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.White), 0, 0, 78, 78);
            };
            lblAvatar = new Label
            {
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(102, 51, 153),
                AutoSize = false,
                Size = new Size(80, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlAvatar.Controls.Add(lblAvatar);
            pnlHeader.Controls.Add(pnlAvatar);

            lblNamaProfil = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 110),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 450
            };
            lblEmailProfil = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LavenderBlush,
                Location = new Point(0, 135),
                Width = 450
            };
            pnlHeader.Controls.AddRange(new Control[] { lblNamaProfil, lblEmailProfil });

            // Body
            int y = 180;

            var pnlRole = new Panel { Location = new Point(30, y), Size = new Size(380, 50), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            pnlRole.Controls.Add(new Label { Text = "Role:", Location = new Point(10, 15), AutoSize = true, ForeColor = Color.Gray });
            lblRoleProfil = new Label { Location = new Point(70, 12), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            pnlRole.Controls.Add(lblRoleProfil);
            this.Controls.Add(pnlRole); y += 70;

            // Tombol aksi
            btnEditProfil = new Button
            {
                Text = "✏️ Edit Profil",
                Location = new Point(30, y),
                Size = new Size(380, 50),
                BackColor = Color.FromArgb(102, 51, 153),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnEditProfil.FlatAppearance.BorderSize = 0;
            btnEditProfil.Click += btnEditProfil_Click;
            this.Controls.Add(btnEditProfil); y += 60;

            btnRiwayat = new Button
            {
                Text = "📋 Riwayat Transaksi",
                Location = new Point(30, y),
                Size = new Size(380, 50),
                BackColor = Color.FromArgb(70, 130, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnRiwayat.FlatAppearance.BorderSize = 0;
            btnRiwayat.Click += btnRiwayat_Click;
            this.Controls.Add(btnRiwayat); y += 60;

            btnLogout = new Button
            {
                Text = "🚪 Keluar",
                Location = new Point(30, y),
                Size = new Size(380, 50),
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += btnLogout_Click;
            this.Controls.Add(btnLogout);

            this.Controls.Add(pnlHeader);
        }

        private Label lblAvatar, lblNamaProfil, lblEmailProfil, lblRoleProfil;
        private Button btnEditProfil, btnRiwayat, btnLogout;
    }
}
