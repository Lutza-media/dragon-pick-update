// ============================================================
// LUTZA - Halaman Detail Transaksi
// File: Views/FormDetailTransaksi.cs
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormDetailTransaksi : Form
    {
        private readonly Transaksi _transaksi;
        private readonly TransaksiContext _transaksiContext = new TransaksiContext();

        public FormDetailTransaksi(Transaksi transaksi)
        {
            _transaksi = transaksi;
            InitializeComponent();
            TampilkanDetail();
        }

        private void TampilkanDetail()
        {
            lblKodeBooking.Text = _transaksi.kodebooking;
            lblNamaAcara.Text = _transaksi.namaacara;
            lblLokasi.Text = _transaksi.lokasitiket;
            lblTanggalAcara.Text = _transaksi.tanggalacara.ToString("dddd, dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID"));
            lblKategori.Text = _transaksi.kategoritiket?.ToUpper();
            lblJumlahTiket.Text = _transaksi.jumlahtiket.ToString();
            lblTotal.Text = _transaksi.totalharga.ToString("C0", new System.Globalization.CultureInfo("id-ID"));
            lblMetode.Text = string.IsNullOrEmpty(_transaksi.metodepembayaran) ? "-" : _transaksi.metodepembayaran;
            lblTanggalPesan.Text = _transaksi.tanggaltransaksi.ToString("dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID"));

            // Tampilan status dengan warna
            lblStatus.Text = _transaksi.statuspembayaran.ToUpper();
            switch (_transaksi.statuspembayaran)
            {
                case "lunas": lblStatus.ForeColor = Color.Green; break;
                case "pending": lblStatus.ForeColor = Color.DarkOrange; break;
                case "batal": lblStatus.ForeColor = Color.Red; break;
            }

            // Sembunyikan tombol bayar jika sudah lunas atau batal
            btnBayarSekarang.Visible = _transaksi.statuspembayaran == "pending";
        }

        private void btnBayarSekarang_Click(object sender, EventArgs e)
        {
            var formPembayaran = new FormPembayaran(_transaksi);
            formPembayaran.ShowDialog();
            // Reload data transaksi
            var updated = _transaksiContext.GetTransaksiById(_transaksi.id);
            if (updated != null)
            {
                _transaksi.statuspembayaran = updated.statuspembayaran;
                _transaksi.metodepembayaran = updated.metodepembayaran;
                TampilkanDetail();
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.Text = "Detail Transaksi";
            this.Size = new Size(520, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(102, 51, 153) };
            var lblTitle = new Label { Text = "Detail Transaksi", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblTitle);

            // Konten utama
            var pnlBody = new Panel { Location = new Point(30, 80), Width = 440, Height = 380 };

            int y = 0;
            int gap = 40;

            void TambahBaris(string label, ref Label valueLabel)
            {
                var lbl = new Label { Text = label, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(0, y), AutoSize = true };
                valueLabel = new Label { Font = new Font("Segoe UI", 11), ForeColor = Color.Black, Location = new Point(0, y + 20), AutoSize = true };
                pnlBody.Controls.Add(lbl);
                pnlBody.Controls.Add(valueLabel);
                y += gap;
            }

            TambahBaris("KODE BOOKING", ref lblKodeBooking);
            lblKodeBooking.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblKodeBooking.ForeColor = Color.FromArgb(102, 51, 153);
            TambahBaris("NAMA ACARA", ref lblNamaAcara);
            TambahBaris("LOKASI", ref lblLokasi);
            TambahBaris("TANGGAL ACARA", ref lblTanggalAcara);
            TambahBaris("KATEGORI TIKET", ref lblKategori);
            TambahBaris("JUMLAH TIKET", ref lblJumlahTiket);
            TambahBaris("TOTAL HARGA", ref lblTotal);
            lblTotal.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            TambahBaris("METODE PEMBAYARAN", ref lblMetode);
            TambahBaris("STATUS", ref lblStatus);
            lblStatus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            TambahBaris("TANGGAL PESAN", ref lblTanggalPesan);

            // Tombol
            btnBayarSekarang = new Button
            {
                Text = "💳 Bayar Sekarang",
                Location = new Point(30, 490),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(102, 51, 153),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnBayarSekarang.FlatAppearance.BorderSize = 0;
            btnBayarSekarang.Click += btnBayarSekarang_Click;

            btnTutup = new Button
            {
                Text = "Tutup",
                Location = new Point(380, 490),
                Size = new Size(100, 40),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnTutup.Click += btnTutup_Click;

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBody);
            this.Controls.Add(btnBayarSekarang);
            this.Controls.Add(btnTutup);
        }

        private Label lblKodeBooking, lblNamaAcara, lblLokasi, lblTanggalAcara;
        private Label lblKategori, lblJumlahTiket, lblTotal, lblMetode, lblStatus, lblTanggalPesan;
        private Button btnBayarSekarang, btnTutup;
    }
}
