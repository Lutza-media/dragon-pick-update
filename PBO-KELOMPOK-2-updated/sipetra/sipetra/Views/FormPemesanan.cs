// ============================================================
// SHEVA - Halaman Transaksi Pemesanan
// File: Views/FormPemesanan.cs
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormPemesanan : Form
    {
        private readonly Tiket _tiket;
        private readonly User _userLogin;
        private readonly TransaksiContext _transaksiContext = new TransaksiContext();

        public FormPemesanan(Tiket tiket, User user)
        {
            _tiket = tiket;
            _userLogin = user;
            InitializeComponent();
            TampilkanInfoTiket();
        }

        private void TampilkanInfoTiket()
        {
            lblNamaAcara.Text = _tiket.namaacara;
            lblLokasi.Text = _tiket.lokasi;
            lblTanggal.Text = _tiket.tanggal.ToString("dddd, dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID"));
            lblKategori.Text = _tiket.kategori.ToUpper();
            lblHargaSatuan.Text = _tiket.harga.ToString("C0", new System.Globalization.CultureInfo("id-ID"));
            lblStok.Text = $"Stok tersedia: {_tiket.stok}";
            HitungTotal();
        }

        private void HitungTotal()
        {
            int jumlah = (int)nudJumlahTiket.Value;
            decimal total = jumlah * _tiket.harga;
            lblTotal.Text = total.ToString("C0", new System.Globalization.CultureInfo("id-ID"));
        }

        private void nudJumlahTiket_ValueChanged(object sender, EventArgs e)
        {
            HitungTotal();
        }

        private void btnPesan_Click(object sender, EventArgs e)
        {
            int jumlah = (int)nudJumlahTiket.Value;
            if (jumlah <= 0)
            {
                MessageBox.Show("Jumlah tiket minimal 1.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (jumlah > _tiket.stok)
            {
                MessageBox.Show($"Stok tidak cukup! Stok tersedia: {_tiket.stok}", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var konfirmasi = MessageBox.Show(
                $"Konfirmasi Pemesanan:\n\n" +
                $"Acara: {_tiket.namaacara}\n" +
                $"Kategori: {_tiket.kategori.ToUpper()}\n" +
                $"Jumlah: {jumlah} tiket\n" +
                $"Total: {(jumlah * _tiket.harga):C0}\n\n" +
                $"Lanjutkan pemesanan?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            try
            {
                var transaksi = new Transaksi
                {
                    kodebooking = GenerateKodeBooking(),
                    userId = _userLogin.id,
                    tiketId = _tiket.id,
                    jumlahtiket = jumlah,
                    totalharga = jumlah * _tiket.harga,
                    statuspembayaran = "pending",
                    metodepembayaran = null
                };

                int newId = _transaksiContext.CreateTransaksi(transaksi);
                transaksi.id = newId;
                transaksi.namaacara = _tiket.namaacara;
                transaksi.lokasitiket = _tiket.lokasi;
                transaksi.tanggalacara = _tiket.tanggal;
                transaksi.kategoritiket = _tiket.kategori;

                MessageBox.Show($"Pemesanan berhasil!\nKode Booking: {transaksi.kodebooking}\n\nSilakan lakukan pembayaran.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Langsung ke halaman pembayaran
                var formPembayaran = new FormPembayaran(transaksi);
                formPembayaran.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memesan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GenerateKodeBooking()
        {
            return "BKG-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100, 999);
        }

        private void InitializeComponent()
        {
            this.Text = "Pemesanan Tiket";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(102, 51, 153) };
            var lblTitle = new Label { Text = "Form Pemesanan Tiket", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblTitle);

            int y = 80;
            int xLabel = 30, xValue = 200;

            Label BuatLabel(string text, int yPos, bool bold = false)
            {
                return new Label { Text = text, Location = new Point(xLabel, yPos), AutoSize = true, Font = new Font("Segoe UI", bold ? 10 : 9, bold ? FontStyle.Bold : FontStyle.Regular), ForeColor = bold ? Color.Black : Color.Gray };
            }

            this.Controls.Add(BuatLabel("Nama Acara:", y));
            lblNamaAcara = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(102, 51, 153) };
            this.Controls.Add(lblNamaAcara); y += 35;

            this.Controls.Add(BuatLabel("Lokasi:", y));
            lblLokasi = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(lblLokasi); y += 30;

            this.Controls.Add(BuatLabel("Tanggal:", y));
            lblTanggal = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(lblTanggal); y += 30;

            this.Controls.Add(BuatLabel("Kategori:", y));
            lblKategori = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(lblKategori); y += 30;

            this.Controls.Add(BuatLabel("Harga/Tiket:", y));
            lblHargaSatuan = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DarkGreen };
            this.Controls.Add(lblHargaSatuan); y += 30;

            lblStok = new Label { Location = new Point(xLabel, y), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Italic) };
            this.Controls.Add(lblStok); y += 40;

            // Separator
            var sep = new Label { Location = new Point(xLabel, y), Size = new Size(420, 2), BackColor = Color.LightGray };
            this.Controls.Add(sep); y += 15;

            // Jumlah tiket
            this.Controls.Add(BuatLabel("Jumlah Tiket:", y, true));
            nudJumlahTiket = new NumericUpDown
            {
                Location = new Point(xValue, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 10,
                Value = 1,
                Font = new Font("Segoe UI", 11)
            };
            nudJumlahTiket.ValueChanged += nudJumlahTiket_ValueChanged;
            this.Controls.Add(nudJumlahTiket); y += 45;

            this.Controls.Add(BuatLabel("Total Harga:", y, true));
            lblTotal = new Label { Location = new Point(xValue, y), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.DarkGreen };
            this.Controls.Add(lblTotal); y += 55;

            // Tombol
            btnPesan = new Button
            {
                Text = "🎫 Pesan Sekarang",
                Location = new Point(xLabel, y),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(102, 51, 153),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnPesan.FlatAppearance.BorderSize = 0;
            btnPesan.Click += btnPesan_Click;

            btnBatal = new Button
            {
                Text = "Batal",
                Location = new Point(xLabel + 220, y),
                Size = new Size(100, 45),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnBatal.Click += btnBatal_Click;

            this.Controls.Add(btnPesan);
            this.Controls.Add(btnBatal);
            this.Controls.Add(pnlHeader);
        }

        private Label lblNamaAcara, lblLokasi, lblTanggal, lblKategori, lblHargaSatuan, lblStok, lblTotal;
        private NumericUpDown nudJumlahTiket;
        private Button btnPesan, btnBatal;
    }
}
