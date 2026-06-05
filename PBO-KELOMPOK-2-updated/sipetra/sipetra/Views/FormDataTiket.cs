// ============================================================
// ZONA - Halaman Data Tiket (Admin)
// File: Views/FormDataTiket.cs
// ============================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormDataTiket : Form
    {
        private readonly TiketContext _tiketContext = new TiketContext();
        private List<Tiket> _daftarTiket;
        private Tiket _tiketDipilih = null;

        public FormDataTiket()
        {
            InitializeComponent();
            MuatData();
        }

        private void MuatData()
        {
            try
            {
                _daftarTiket = _tiketContext.GetAllTiket();
                var table = new System.Data.DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("Kode Tiket", typeof(string));
                table.Columns.Add("Nama Acara", typeof(string));
                table.Columns.Add("Lokasi", typeof(string));
                table.Columns.Add("Tanggal", typeof(string));
                table.Columns.Add("Kategori", typeof(string));
                table.Columns.Add("Harga", typeof(string));
                table.Columns.Add("Stok", typeof(int));

                foreach (var t in _daftarTiket)
                {
                    table.Rows.Add(t.id, t.kodetiket, t.namaacara, t.lokasi,
                        t.tanggal.ToString("dd/MM/yyyy"),
                        t.kategori.ToUpper(),
                        t.harga.ToString("C0", new System.Globalization.CultureInfo("id-ID")),
                        t.stok);
                }

                dgvTiket.DataSource = table;
                dgvTiket.Columns["ID"].Visible = false;

                // Warnai kategori
                foreach (DataGridViewRow row in dgvTiket.Rows)
                {
                    string kat = row.Cells["Kategori"].Value?.ToString();
                    if (kat == "VIP") row.DefaultCellStyle.BackColor = Color.LightYellow;
                    else if (kat == "VVIP") row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

                    int stok = (int)(row.Cells["Stok"].Value ?? 0);
                    if (stok == 0) row.DefaultCellStyle.ForeColor = Color.Red;
                }

                lblJumlah.Text = $"Total: {_daftarTiket.Count} tiket";
                BersihkanForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTiket_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTiket.SelectedRows.Count == 0) return;
            int id = (int)dgvTiket.SelectedRows[0].Cells["ID"].Value;
            _tiketDipilih = _daftarTiket.Find(t => t.id == id);
            if (_tiketDipilih != null) IsiForm(_tiketDipilih);
        }

        private void IsiForm(Tiket t)
        {
            tbKode.Text = t.kodetiket;
            tbNamaAcara.Text = t.namaacara;
            tbLokasi.Text = t.lokasi;
            dtpTanggal.Value = t.tanggal;
            cmbKategori.SelectedItem = t.kategori;
            nudHarga.Value = t.harga;
            nudStok.Value = t.stok;
            tbDeskripsi.Text = t.deskripsi;
        }

        private void BersihkanForm()
        {
            _tiketDipilih = null;
            tbKode.Clear(); tbNamaAcara.Clear(); tbLokasi.Clear();
            dtpTanggal.Value = DateTime.Now;
            cmbKategori.SelectedIndex = 0;
            nudHarga.Value = 0; nudStok.Value = 0;
            tbDeskripsi.Clear();
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (!ValidasiInput()) return;
            try
            {
                var tiket = AmbilDariForm();
                _tiketContext.AddTiket(tiket);
                MessageBox.Show("Tiket berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MuatData();
            }
            catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_tiketDipilih == null) { MessageBox.Show("Pilih tiket yang ingin diupdate."); return; }
            if (!ValidasiInput()) return;
            try
            {
                var tiket = AmbilDariForm();
                tiket.id = _tiketDipilih.id;
                _tiketContext.UpdateTiket(tiket);
                MessageBox.Show("Tiket berhasil diupdate!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MuatData();
            }
            catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (_tiketDipilih == null) { MessageBox.Show("Pilih tiket yang ingin dihapus."); return; }
            var konfirmasi = MessageBox.Show($"Hapus tiket '{_tiketDipilih.namaacara}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (konfirmasi != DialogResult.Yes) return;
            try
            {
                _tiketContext.DeleteTiket(_tiketDipilih.id);
                MessageBox.Show("Tiket berhasil dihapus.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MuatData();
            }
            catch (Exception ex) { MessageBox.Show("Gagal menghapus: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnBersih_Click(object sender, EventArgs e) { BersihkanForm(); }

        private Tiket AmbilDariForm()
        {
            return new Tiket
            {
                kodetiket = tbKode.Text.Trim(),
                namaacara = tbNamaAcara.Text.Trim(),
                lokasi = tbLokasi.Text.Trim(),
                tanggal = dtpTanggal.Value,
                kategori = cmbKategori.SelectedItem?.ToString() ?? "reguler",
                harga = nudHarga.Value,
                stok = (int)nudStok.Value,
                deskripsi = tbDeskripsi.Text.Trim()
            };
        }

        private bool ValidasiInput()
        {
            if (string.IsNullOrWhiteSpace(tbKode.Text)) { MessageBox.Show("Kode tiket wajib diisi."); return false; }
            if (string.IsNullOrWhiteSpace(tbNamaAcara.Text)) { MessageBox.Show("Nama acara wajib diisi."); return false; }
            if (string.IsNullOrWhiteSpace(tbLokasi.Text)) { MessageBox.Show("Lokasi wajib diisi."); return false; }
            if (nudHarga.Value <= 0) { MessageBox.Show("Harga harus lebih dari 0."); return false; }
            return true;
        }

        private void InitializeComponent()
        {
            this.Text = "Data Tiket (Admin)";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(180, 100, 0) };
            pnlHeader.Controls.Add(new Label { Text = "🎫 Manajemen Data Tiket", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(15, 14) });

            // Left: DataGridView
            var pnlKiri = new Panel { Location = new Point(0, 55), Size = new Size(620, 625), Dock = DockStyle.None };
            var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.WhiteSmoke };
            lblJumlah = new Label { AutoSize = true, Location = new Point(10, 12), ForeColor = Color.DimGray };
            var btnRefresh = new Button { Text = "🔄 Refresh", Location = new Point(500, 7), Width = 100, BackColor = Color.SlateGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += (s, e) => MuatData();
            pnlToolbar.Controls.AddRange(new Control[] { lblJumlah, btnRefresh });

            dgvTiket = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(180, 100, 0), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            dgvTiket.SelectionChanged += dgvTiket_SelectionChanged;
            pnlKiri.Controls.Add(dgvTiket);
            pnlKiri.Controls.Add(pnlToolbar);

            // Right: Form input
            var pnlKanan = new Panel { Location = new Point(625, 55), Size = new Size(465, 625), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            pnlKanan.Controls.Add(new Label { Text = "Form Tiket", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true, ForeColor = Color.FromArgb(180, 100, 0) });

            int y = 50;
            Label BuatLabel(string text) => new Label { Text = text, AutoSize = true, Location = new Point(15, y), ForeColor = Color.Gray, Font = new Font("Segoe UI", 9) };

            Control[] BuatBaris(string label, Control ctrl)
            {
                var lbl = BuatLabel(label);
                ctrl.Location = new Point(15, y + 20);
                y += 55;
                return new Control[] { lbl, ctrl };
            }

            tbKode = new TextBox { Width = 430, Font = new Font("Segoe UI", 10) };
            tbNamaAcara = new TextBox { Width = 430, Font = new Font("Segoe UI", 10) };
            tbLokasi = new TextBox { Width = 430, Font = new Font("Segoe UI", 10) };
            dtpTanggal = new DateTimePicker { Width = 430, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", ShowUpDown = false };
            cmbKategori = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cmbKategori.Items.AddRange(new[] { "reguler", "vip", "vvip" });
            cmbKategori.SelectedIndex = 0;
            nudHarga = new NumericUpDown { Width = 200, Maximum = 99999999, DecimalPlaces = 0, Font = new Font("Segoe UI", 10) };
            nudStok = new NumericUpDown { Width = 100, Maximum = 9999, Font = new Font("Segoe UI", 10) };
            tbDeskripsi = new TextBox { Width = 430, Height = 60, Multiline = true, Font = new Font("Segoe UI", 9) };

            foreach (var controls in new[] {
                BuatBaris("Kode Tiket *", tbKode),
                BuatBaris("Nama Acara *", tbNamaAcara),
                BuatBaris("Lokasi *", tbLokasi),
                BuatBaris("Tanggal Acara", dtpTanggal),
                BuatBaris("Kategori", cmbKategori),
                BuatBaris("Harga (Rp) *", nudHarga),
                BuatBaris("Stok", nudStok),
                BuatBaris("Deskripsi", tbDeskripsi)
            })
            {
                pnlKanan.Controls.AddRange(controls);
            }

            y += 10;
            btnTambah = new Button { Text = "➕ Tambah", Location = new Point(15, y), Size = new Size(95, 38), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnTambah.Click += btnTambah_Click;
            btnUpdate = new Button { Text = "✏️ Update", Location = new Point(120, y), Size = new Size(95, 38), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnUpdate.Click += btnUpdate_Click;
            btnHapus = new Button { Text = "🗑️ Hapus", Location = new Point(225, y), Size = new Size(95, 38), BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnHapus.Click += btnHapus_Click;
            btnBersih = new Button { Text = "🔄 Bersih", Location = new Point(330, y), Size = new Size(95, 38), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBersih.Click += btnBersih_Click;

            pnlKanan.Controls.AddRange(new Control[] { btnTambah, btnUpdate, btnHapus, btnBersih });

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlKiri);
            this.Controls.Add(pnlKanan);

            // Atur layout
            pnlKiri.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            pnlKiri.Size = new Size(620, this.ClientSize.Height - 55);
            pnlKanan.Size = new Size(460, this.ClientSize.Height - 55);
            pnlKanan.Location = new Point(630, 55);
        }

        private DataGridView dgvTiket;
        private TextBox tbKode, tbNamaAcara, tbLokasi, tbDeskripsi;
        private DateTimePicker dtpTanggal;
        private ComboBox cmbKategori;
        private NumericUpDown nudHarga, nudStok;
        private Button btnTambah, btnUpdate, btnHapus, btnBersih;
        private Label lblJumlah;
    }
}
