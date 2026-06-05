// ============================================================
// SHEVA - Halaman Pembayaran
// File: Views/FormPembayaran.cs
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using sipetra.Helpers;
using sipetra.Models;

namespace sipetra.Views
{
    public partial class FormPembayaran : Form
    {
        private readonly Transaksi _transaksi;
        private readonly TransaksiContext _transaksiContext = new TransaksiContext();

        public FormPembayaran(Transaksi transaksi)
        {
            _transaksi = transaksi;
            InitializeComponent();
            TampilkanRingkasan();
        }

        private void TampilkanRingkasan()
        {
            lblKodeBooking.Text = _transaksi.kodebooking;
            lblNamaAcara.Text = _transaksi.namaacara;
            lblJumlahTiket.Text = $"{_transaksi.jumlahtiket} tiket";
            lblTotalTagihan.Text = _transaksi.totalharga.ToString("C0", new System.Globalization.CultureInfo("id-ID"));
        }

        private void btnBayar_Click(object sender, EventArgs e)
        {
            string metode = "";
            if (rbTransfer.Checked) metode = "Transfer Bank";
            else if (rbEWallet.Checked) metode = "E-Wallet";
            else if (rbTunai.Checked) metode = "Tunai";
            else
            {
                MessageBox.Show("Pilih metode pembayaran terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var konfirmasi = MessageBox.Show(
                $"Konfirmasi Pembayaran:\n\n" +
                $"Kode Booking: {_transaksi.kodebooking}\n" +
                $"Total: {_transaksi.totalharga:C0}\n" +
                $"Metode: {metode}\n\n" +
                $"Konfirmasi pembayaran?",
                "Konfirmasi Bayar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            try
            {
                _transaksiContext.UpdateStatusPembayaran(_transaksi.id, "lunas", metode);

                // Tampilkan struk sukses
                pnlMetode.Visible = false;
                btnBayar.Visible = false;
                pnlSukses.Visible = true;
                lblSuksesDetail.Text = $"Pembayaran berhasil!\n\nKode Booking: {_transaksi.kodebooking}\nMetode: {metode}\nTotal: {_transaksi.totalharga:C0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memproses pembayaran: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.Text = "Pembayaran";
            this.Size = new Size(480, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(60, 140, 60) };
            var lblTitle = new Label { Text = "Pembayaran Tiket", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblTitle);

            // Ringkasan tagihan
            var pnlTagihan = new Panel { Location = new Point(20, 75), Size = new Size(430, 150), BackColor = Color.FromArgb(240, 255, 240), BorderStyle = BorderStyle.FixedSingle };
            var lblTagihanTitle = new Label { Text = "Ringkasan Tagihan", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true, ForeColor = Color.DarkGreen };
            pnlTagihan.Controls.Add(lblTagihanTitle);

            void TambahBaris(string label, ref Label valLbl, int yPos)
            {
                pnlTagihan.Controls.Add(new Label { Text = label, Location = new Point(10, yPos), AutoSize = true, ForeColor = Color.Gray });
                valLbl = new Label { Location = new Point(200, yPos), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                pnlTagihan.Controls.Add(valLbl);
            }

            TambahBaris("Kode Booking:", ref lblKodeBooking, 35);
            TambahBaris("Nama Acara:", ref lblNamaAcara, 60);
            TambahBaris("Jumlah Tiket:", ref lblJumlahTiket, 85);
            TambahBaris("Total Tagihan:", ref lblTotalTagihan, 110);
            lblTotalTagihan.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTotalTagihan.ForeColor = Color.DarkGreen;

            // Panel metode pembayaran
            pnlMetode = new Panel { Location = new Point(20, 240), Size = new Size(430, 220) };
            var lblMetodeTitle = new Label { Text = "Pilih Metode Pembayaran:", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(0, 0), AutoSize = true };
            pnlMetode.Controls.Add(lblMetodeTitle);

            rbTransfer = new RadioButton { Text = "Transfer Bank (BCA / Mandiri / BNI)", Location = new Point(10, 35), AutoSize = true, Font = new Font("Segoe UI", 10) };
            rbEWallet = new RadioButton { Text = "E-Wallet (GoPay / OVO / Dana)", Location = new Point(10, 70), AutoSize = true, Font = new Font("Segoe UI", 10) };
            rbTunai = new RadioButton { Text = "Tunai (di loket)", Location = new Point(10, 105), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlMetode.Controls.AddRange(new Control[] { rbTransfer, rbEWallet, rbTunai });

            btnBayar = new Button
            {
                Text = "✅ Konfirmasi Bayar",
                Location = new Point(10, 155),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(60, 140, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnBayar.FlatAppearance.BorderSize = 0;
            btnBayar.Click += btnBayar_Click;
            pnlMetode.Controls.Add(btnBayar);

            // Panel sukses (tersembunyi awalnya)
            pnlSukses = new Panel { Location = new Point(20, 240), Size = new Size(430, 220), Visible = false, BackColor = Color.FromArgb(240, 255, 240), BorderStyle = BorderStyle.FixedSingle };
            var lblSuksesIcon = new Label { Text = "✅", Font = new Font("Segoe UI", 36), Location = new Point(180, 20), AutoSize = true };
            lblSuksesDetail = new Label { Location = new Point(15, 80), Size = new Size(400, 120), Font = new Font("Segoe UI", 10), ForeColor = Color.DarkGreen };
            pnlSukses.Controls.AddRange(new Control[] { lblSuksesIcon, lblSuksesDetail });

            btnTutup = new Button
            {
                Text = "Selesai",
                Location = new Point(360, 510),
                Size = new Size(100, 40),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnTutup.Click += btnTutup_Click;

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlTagihan);
            this.Controls.Add(pnlMetode);
            this.Controls.Add(pnlSukses);
            this.Controls.Add(btnTutup);
        }

        private Label lblKodeBooking, lblNamaAcara, lblJumlahTiket, lblTotalTagihan, lblSuksesDetail;
        private RadioButton rbTransfer, rbEWallet, rbTunai;
        private Button btnBayar, btnTutup;
        private Panel pnlMetode, pnlSukses;
    }
}
