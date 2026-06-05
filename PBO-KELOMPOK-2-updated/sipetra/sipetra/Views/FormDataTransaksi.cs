// ============================================================
// DINI - Halaman Data Transaksi (Admin)
// File: Views/FormDataTransaksi.cs
// ============================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormDataTransaksi : Form
    {
        private readonly TransaksiContext _transaksiContext = new TransaksiContext();
        private List<Transaksi> _daftarTransaksi;

        public FormDataTransaksi()
        {
            InitializeComponent();
            MuatData();
        }

        private void MuatData()
        {
            try
            {
                _daftarTransaksi = _transaksiContext.GetAllTransaksi();
                RefreshTabel(_daftarTransaksi);
                HitungStatistik();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshTabel(List<Transaksi> list)
        {
            var table = new System.Data.DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Kode Booking", typeof(string));
            table.Columns.Add("Nama Pembeli", typeof(string));
            table.Columns.Add("Nama Acara", typeof(string));
            table.Columns.Add("Jml Tiket", typeof(int));
            table.Columns.Add("Total Harga", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Metode", typeof(string));
            table.Columns.Add("Tanggal", typeof(string));

            foreach (var t in list)
            {
                table.Rows.Add(
                    t.id, t.kodebooking, t.namapembeli ?? "-", t.namaacara,
                    t.jumlahtiket,
                    t.totalharga.ToString("C0", new System.Globalization.CultureInfo("id-ID")),
                    t.statuspembayaran.ToUpper(),
                    string.IsNullOrEmpty(t.metodepembayaran) ? "-" : t.metodepembayaran,
                    t.tanggaltransaksi.ToString("dd/MM/yyyy HH:mm")
                );
            }

            dgvTransaksi.DataSource = table;
            dgvTransaksi.Columns["ID"].Visible = false;

            foreach (DataGridViewRow row in dgvTransaksi.Rows)
            {
                string status = row.Cells["Status"].Value?.ToString();
                if (status == "LUNAS") row.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (status == "PENDING") row.DefaultCellStyle.BackColor = Color.LightYellow;
                else if (status == "BATAL") row.DefaultCellStyle.BackColor = Color.LightCoral;
            }

            lblTotal.Text = $"Total: {list.Count} transaksi";
        }

        private void HitungStatistik()
        {
            decimal totalPendapatan = 0;
            int lunas = 0, pending = 0, batal = 0;
            foreach (var t in _daftarTransaksi)
            {
                if (t.statuspembayaran == "lunas") { lunas++; totalPendapatan += t.totalharga; }
                else if (t.statuspembayaran == "pending") pending++;
                else batal++;
            }

            pnlStats.Controls.Clear();
            void TambahStat(string label, string value, Color color, int x)
            {
                var pnl = new Panel { Location = new Point(x, 5), Size = new Size(145, 60), BackColor = color, BorderStyle = BorderStyle.FixedSingle };
                pnl.Controls.Add(new Label { Text = value, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, Location = new Point(5, 5), AutoSize = true });
                pnl.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 8), ForeColor = Color.WhiteSmoke, Location = new Point(5, 38), AutoSize = true });
                pnlStats.Controls.Add(pnl);
            }

            TambahStat("Lunas", lunas.ToString(), Color.Green, 10);
            TambahStat("Pending", pending.ToString(), Color.DarkOrange, 165);
            TambahStat("Batal", batal.ToString(), Color.Crimson, 320);
            TambahStat("Pendapatan", totalPendapatan.ToString("C0", new System.Globalization.CultureInfo("id-ID")), Color.DarkBlue, 475);
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            string keyword = tbCari.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword)) { MuatData(); return; }

            var hasil = _daftarTransaksi.FindAll(t =>
                t.kodebooking.ToLower().Contains(keyword) ||
                (t.namapembeli?.ToLower().Contains(keyword) == true) ||
                t.namaacara.ToLower().Contains(keyword));

            RefreshTabel(hasil);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            tbCari.Clear();
            MuatData();
        }

        private void tbCari_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnCari_Click(sender, e);
        }

        private void InitializeComponent()
        {
            this.Text = "Data Transaksi (Admin)";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(30, 30, 80) };
            pnlHeader.Controls.Add(new Label { Text = "📊 Data Transaksi", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) });

            // Statistik
            pnlStats = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.WhiteSmoke };

            // Search bar
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.White, Padding = new Padding(10) };
            tbCari = new TextBox { Location = new Point(10, 10), Width = 300, Font = new Font("Segoe UI", 10), PlaceholderText = "Cari kode booking / nama pembeli / acara..." };
            tbCari.KeyDown += tbCari_KeyDown;
            btnCari = new Button { Text = "🔍 Cari", Location = new Point(320, 8), Width = 80, BackColor = Color.FromArgb(30, 30, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCari.Click += btnCari_Click;
            btnRefresh = new Button { Text = "🔄", Location = new Point(410, 8), Width = 40, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += btnRefresh_Click;
            lblTotal = new Label { AutoSize = true, Location = new Point(470, 12), ForeColor = Color.DimGray };
            pnlSearch.Controls.AddRange(new Control[] { tbCari, btnCari, btnRefresh, lblTotal });

            // DataGridView
            dgvTransaksi = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(30, 30, 80), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            this.Controls.Add(dgvTransaksi);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlStats);
            this.Controls.Add(pnlHeader);
        }

        private DataGridView dgvTransaksi;
        private Panel pnlStats;
        private TextBox tbCari;
        private Button btnCari, btnRefresh;
        private Label lblTotal;
    }
}
