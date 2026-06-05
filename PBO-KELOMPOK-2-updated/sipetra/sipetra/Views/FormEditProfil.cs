// ============================================================
// ZONA - Halaman Edit Profil
// File: Views/FormEditProfil.cs
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormEditProfil : Form
    {
        private readonly UserContext _userContext = new UserContext();
        private readonly User _user;

        public FormEditProfil(User user)
        {
            _user = user;
            InitializeComponent();
            IsiDataAwal();
        }

        private void IsiDataAwal()
        {
            tbNama.Text = _user.nama;
            tbEmail.Text = _user.email;
            tbNoHp.Text = _user.noHp;
            tbAlamat.Text = _user.alamat;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNama.Text))
            {
                MessageBox.Show("Nama tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show("Email tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _user.nama = tbNama.Text.Trim();
                _user.email = tbEmail.Text.Trim();
                _user.noHp = tbNoHp.Text.Trim();
                _user.alamat = tbAlamat.Text.Trim();

                _userContext.UpdateProfil(_user);
                MessageBox.Show("Profil berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGantiPassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbPasswordLama.Text))
            {
                MessageBox.Show("Masukkan password lama.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tbPasswordLama.Text != _user.password)
            {
                MessageBox.Show("Password lama tidak sesuai.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(tbPasswordBaru.Text))
            {
                MessageBox.Show("Password baru tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tbPasswordBaru.Text != tbKonfirmPassword.Text)
            {
                MessageBox.Show("Konfirmasi password tidak cocok.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tbPasswordBaru.Text.Length < 6)
            {
                MessageBox.Show("Password minimal 6 karakter.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _userContext.UpdatePassword(_user.id, tbPasswordBaru.Text);
                _user.password = tbPasswordBaru.Text;
                MessageBox.Show("Password berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPasswordLama.Clear(); tbPasswordBaru.Clear(); tbKonfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengubah password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.Text = "Edit Profil";
            this.Size = new Size(480, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(102, 51, 153) };
            pnlHeader.Controls.Add(new Label { Text = "✏️ Edit Profil", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(15, 14) });

            int y = 75;

            Control[] BuatBaris(string labelText, Control ctrl, bool password = false)
            {
                var lbl = new Label { Text = labelText, AutoSize = true, Location = new Point(30, y), Font = new Font("Segoe UI", 9), ForeColor = Color.Gray };
                ctrl.Location = new Point(30, y + 20);
                if (ctrl is TextBox tb) { tb.Width = 410; tb.Font = new Font("Segoe UI", 11); if (password) tb.PasswordChar = '●'; }
                y += 60;
                return new Control[] { lbl, ctrl };
            }

            tbNama = new TextBox();
            tbEmail = new TextBox();
            tbNoHp = new TextBox();
            tbAlamat = new TextBox { Height = 60, Multiline = true };

            foreach (var ctrls in new[] {
                BuatBaris("Nama Lengkap *", tbNama),
                BuatBaris("Email *", tbEmail),
                BuatBaris("No. HP", tbNoHp),
            })
                this.Controls.AddRange(ctrls);

            var lblAlamat = new Label { Text = "Alamat", AutoSize = true, Location = new Point(30, y), Font = new Font("Segoe UI", 9), ForeColor = Color.Gray };
            tbAlamat.Location = new Point(30, y + 20); tbAlamat.Width = 410; tbAlamat.Font = new Font("Segoe UI", 10);
            this.Controls.AddRange(new Control[] { lblAlamat, tbAlamat });
            y += 90;

            btnSimpan = new Button { Text = "💾 Simpan Profil", Location = new Point(30, y), Size = new Size(160, 42), BackColor = Color.FromArgb(102, 51, 153), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += btnSimpan_Click;
            btnBatal = new Button { Text = "Batal", Location = new Point(200, y), Size = new Size(90, 42), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat };
            btnBatal.Click += btnBatal_Click;
            this.Controls.AddRange(new Control[] { btnSimpan, btnBatal });
            y += 65;

            // Ganti password
            var sepLabel = new Label { Text = "─── Ganti Password ───", AutoSize = true, Location = new Point(30, y), ForeColor = Color.Gray, Font = new Font("Segoe UI", 9, FontStyle.Italic) };
            this.Controls.Add(sepLabel); y += 25;

            tbPasswordLama = new TextBox();
            tbPasswordBaru = new TextBox();
            tbKonfirmPassword = new TextBox();

            foreach (var ctrls in new[] {
                BuatBaris("Password Lama", tbPasswordLama, true),
                BuatBaris("Password Baru (min. 6 karakter)", tbPasswordBaru, true),
                BuatBaris("Konfirmasi Password Baru", tbKonfirmPassword, true)
            })
                this.Controls.AddRange(ctrls);

            btnGantiPassword = new Button { Text = "🔒 Ubah Password", Location = new Point(30, y), Size = new Size(180, 42), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btnGantiPassword.FlatAppearance.BorderSize = 0;
            btnGantiPassword.Click += btnGantiPassword_Click;
            this.Controls.Add(btnGantiPassword);

            this.Controls.Add(pnlHeader);
            this.Height = y + 110;
        }

        private TextBox tbNama, tbEmail, tbNoHp, tbAlamat;
        private TextBox tbPasswordLama, tbPasswordBaru, tbKonfirmPassword;
        private Button btnSimpan, btnBatal, btnGantiPassword;
    }
}
