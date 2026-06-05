using System;

namespace sipetra.Models
{
    public class Tiket
    {
        public int id { get; set; }
        public string kodetiket { get; set; }
        public string namaacara { get; set; }
        public string lokasi { get; set; }
        public DateTime tanggal { get; set; }
        public string kategori { get; set; }   // "reguler", "vip", "vvip"
        public decimal harga { get; set; }
        public int stok { get; set; }
        public string deskripsi { get; set; }
    }
}
