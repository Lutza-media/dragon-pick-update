using System;

namespace sipetra.Models
{
    public class Transaksi
    {
        public int id { get; set; }
        public string kodebooking { get; set; }
        public int userId { get; set; }
        public int tiketId { get; set; }
        public int jumlahtiket { get; set; }
        public decimal totalharga { get; set; }
        public string statuspembayaran { get; set; }  // "pending", "lunas", "batal"
        public string metodepembayaran { get; set; }  // "transfer", "tunai", "ewallet"
        public DateTime tanggaltransaksi { get; set; }

        // JOIN fields (tidak disimpan ke DB, diisi saat query)
        public string namaacara { get; set; }
        public string lokasitiket { get; set; }
        public DateTime tanggalacara { get; set; }
        public string kategoritiket { get; set; }
        public string namapembeli { get; set; }
    }
}
