// ============================================================
// LUTZA - Halaman Riwayat Transaksi
// File: Views/FormRiwayatTransaksi.cs
// ============================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormRiwayatTransaksi : Form
    {
        private readonly TransaksiContext _transaksiContext = new TransaksiContext();
        private readonly int _userId;
        private List<Transaksi> _daftarTransaksi;

        public FormRiwayatTransaksi(int userId)
        {
            _userId = userId;
            InitializeComponent();
            MuatDataRiwayat();
        }

        private void MuatDataRiwayat()
        {
            try
            {
                _daftarTransaksi = _transaksiContext.GetTransaksiByUser(_userId);
                dgvRiwayat.DataSource = null;

                // Buat DataTable untuk tampilan yang lebih rapi
                var table = new System.Data.DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("Kode Booking", typeof(string));
                table.Columns.Add("Nama Acara", typeof(string));
                table.Columns.Add("Tanggal Acara", typeof(string));
                table.Columns.Add("Jumlah Tiket", typeof(int));
                table.Columns.Add("Total Harga", typeof(string));
                table.Columns.Add("Status", typeof(string));
                table.Columns.Add("Tanggal Pesan", typeof(string));

                foreach (var t in _daftarTransaksi)
                {
                    table.Rows.Add(
                        t.id,
                        t.kodebooking,
                        t.namaacara,
                        t.tanggalacara.ToString("dd MMM yyyy"),
                        t.jumlahtiket,
                        t.totalharga.ToString("C0", new System.Globalization.CultureInfo("id-ID")),
                        t.statuspembayaran.ToUpper(),
                        t.tanggaltransaksi.ToString("dd MMM yyyy HH:mm")
                    );
                }

                dgvRiwayat.DataSource = table;
                dgvRiwayat.Columns["ID"].Visible = false;

                // Warnai baris berdasarkan status
                WarnaiStatusBaris();

                lblJumlah.Text = $"Total transaksi: {_daftarTransaksi.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WarnaiStatusBaris()
        {
            foreach (DataGridViewRow row in dgvRiwayat.Rows)
            {
                string status = row.Cells["Status"].Value?.ToString();
                if (status == "LUNAS")
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (status == "PENDING")
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                else if (status == "BATAL")
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }

        private void dgvRiwayat_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = (int)dgvRiwayat.Rows[e.RowIndex].Cells["ID"].Value;
            var transaksi = _daftarTransaksi.Find(t => t.id == id);
            if (transaksi != null)
            {
                var detailForm = new FormDetailTransaksi(transaksi);
                detailForm.ShowDialog();
                MuatDataRiwayat(); // refresh setelah kembali
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            MuatDataRiwayat();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string filter = cmbFilter.SelectedItem?.ToString()?.ToLower();
            if (string.IsNullOrEmpty(filter) || filter == "semua")
            {
                MuatDataRiwayat();
                return;
            }

            var filtered = _daftarTransaksi.FindAll(t => t.statuspembayaran == filter);
            var table = new System.Data.DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Kode Booking", typeof(string));
            table.Columns.Add("Nama Acara", typeof(string));
            table.Columns.Add("Tanggal Acara", typeof(string));
            table.Columns.Add("Jumlah Tiket", typeof(int));
            table.Columns.Add("Total Harga", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Tanggal Pesan", typeof(string));

            foreach (var t in filtered)
            {
                table.Rows.Add(t.id, t.kodebooking, t.namaacara,
                    t.tanggalacara.ToString("dd MMM yyyy"), t.jumlahtiket,
                    t.totalharga.ToString("C0", new System.Globalization.CultureInfo("id-ID")),
                    t.statuspembayaran.ToUpper(),
                    t.tanggaltransaksi.ToString("dd MMM yyyy HH:mm"));
            }

            dgvRiwayat.DataSource = table;
            dgvRiwayat.Columns["ID"].Visible = false;
            WarnaiStatusBaris();
            lblJumlah.Text = $"Menampilkan: {filtered.Count} transaksi";
        }

        private void InitializeComponent()
        {
            this.Text = "Riwayat Transaksi";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(102, 51, 153) };
            var lblTitle = new Label
            {
                Text = "Riwayat Transaksi Saya",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlHeader.Controls.Add(lblTitle);

            // Filter panel
            var pnlFilter = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.WhiteSmoke, Padding = new Padding(10, 10, 10, 5) };
            var lblFilter = new Label { Text = "Filter Status:", AutoSize = true, Location = new Point(10, 15) };
            cmbFilter = new ComboBox { Location = new Point(90, 12), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilter.Items.AddRange(new[] { "Semua", "Pending", "Lunas", "Batal" });
            cmbFilter.SelectedIndex = 0;
            btnFilter = new Button { Text = "Terapkan", Location = new Point(220, 11), Width = 80, BackColor = Color.FromArgb(102, 51, 153), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnFilter.Click += btnFilter_Click;
            btnRefresh = new Button { Text = "🔄 Refresh", Location = new Point(315, 11), Width = 90, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += btnRefresh_Click;
            lblJumlah = new Label { AutoSize = true, Location = new Point(420, 15), ForeColor = Color.DimGray };
            pnlFilter.Controls.AddRange(new Control[] { lblFilter, cmbFilter, btnFilter, btnRefresh, lblJumlah });

            // DataGridView
            dgvRiwayat = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(230, 210, 255), Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            dgvRiwayat.CellDoubleClick += dgvRiwayat_CellDoubleClick;

            var lblInfo = new Label
            {
                Text = "💡 Klik dua kali pada baris untuk melihat detail transaksi",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightCyan,
                ForeColor = Color.DarkSlateBlue
            };

            this.Controls.Add(dgvRiwayat);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblInfo);
        }

        private DataGridView dgvRiwayat;
        private ComboBox cmbFilter;
        private Button btnFilter, btnRefresh;
        private Label lblJumlah;
    }
}
