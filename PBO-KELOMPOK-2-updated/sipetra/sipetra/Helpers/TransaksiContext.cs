using System;
using System.Collections.Generic;
using Npgsql;
using sipetra.Models;

namespace sipetra.Helpers
{
    public class TransaksiContext
    {
        // Ambil semua transaksi milik user tertentu (dengan detail tiket)
        public List<Transaksi> GetTransaksiByUser(int userId)
        {
            var list = new List<Transaksi>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT t.id, t.kodebooking, t.user_id, t.tiket_id, t.jumlahtiket,
                           t.totalharga, t.statuspembayaran, t.metodepembayaran, t.tanggaltransaksi,
                           tk.namaacara, tk.lokasi AS lokasitiket, tk.tanggal AS tanggalacara, tk.kategori AS kategoritiket
                    FROM transaksi t
                    JOIN tiket tk ON t.tiket_id = tk.id
                    WHERE t.user_id = @UserId
                    ORDER BY t.tanggaltransaksi DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) list.Add(MapTransaksi(reader));
                    }
                }
            }
            return list;
        }

        // Ambil semua transaksi (untuk admin)
        public List<Transaksi> GetAllTransaksi()
        {
            var list = new List<Transaksi>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT t.id, t.kodebooking, t.user_id, t.tiket_id, t.jumlahtiket,
                           t.totalharga, t.statuspembayaran, t.metodepembayaran, t.tanggaltransaksi,
                           tk.namaacara, tk.lokasi AS lokasitiket, tk.tanggal AS tanggalacara, tk.kategori AS kategoritiket,
                           u.nama AS namapembeli
                    FROM transaksi t
                    JOIN tiket tk ON t.tiket_id = tk.id
                    JOIN users u ON t.user_id = u.id
                    ORDER BY t.tanggaltransaksi DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tr = MapTransaksi(reader);
                        if (!reader.IsDBNull(reader.GetOrdinal("namapembeli")))
                            tr.namapembeli = reader.GetString(reader.GetOrdinal("namapembeli"));
                        list.Add(tr);
                    }
                }
            }
            return list;
        }

        // Ambil satu transaksi berdasarkan ID
        public Transaksi GetTransaksiById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT t.id, t.kodebooking, t.user_id, t.tiket_id, t.jumlahtiket,
                           t.totalharga, t.statuspembayaran, t.metodepembayaran, t.tanggaltransaksi,
                           tk.namaacara, tk.lokasi AS lokasitiket, tk.tanggal AS tanggalacara, tk.kategori AS kategoritiket,
                           u.nama AS namapembeli
                    FROM transaksi t
                    JOIN tiket tk ON t.tiket_id = tk.id
                    JOIN users u ON t.user_id = u.id
                    WHERE t.id = @Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var tr = MapTransaksi(reader);
                            if (!reader.IsDBNull(reader.GetOrdinal("namapembeli")))
                                tr.namapembeli = reader.GetString(reader.GetOrdinal("namapembeli"));
                            return tr;
                        }
                    }
                }
            }
            return null;
        }

        // Buat transaksi baru, kurangi stok tiket
        public int CreateTransaksi(Transaksi t)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Cek stok
                        string cekStok = "SELECT stok FROM tiket WHERE id = @TiketId FOR UPDATE";
                        using (var cmd = new NpgsqlCommand(cekStok, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@TiketId", t.tiketId);
                            var stok = (int)cmd.ExecuteScalar();
                            if (stok < t.jumlahtiket)
                                throw new Exception("Stok tiket tidak mencukupi!");
                        }

                        // Kurangi stok
                        string kurangiStok = "UPDATE tiket SET stok = stok - @Jumlah WHERE id = @TiketId";
                        using (var cmd = new NpgsqlCommand(kurangiStok, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Jumlah", t.jumlahtiket);
                            cmd.Parameters.AddWithValue("@TiketId", t.tiketId);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert transaksi
                        string insertQuery = @"
                            INSERT INTO transaksi (kodebooking, user_id, tiket_id, jumlahtiket, totalharga, statuspembayaran, metodepembayaran)
                            VALUES (@KodeBooking, @UserId, @TiketId, @Jumlah, @Total, @Status, @Metode)
                            RETURNING id";
                        int newId;
                        using (var cmd = new NpgsqlCommand(insertQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@KodeBooking", t.kodebooking);
                            cmd.Parameters.AddWithValue("@UserId", t.userId);
                            cmd.Parameters.AddWithValue("@TiketId", t.tiketId);
                            cmd.Parameters.AddWithValue("@Jumlah", t.jumlahtiket);
                            cmd.Parameters.AddWithValue("@Total", t.totalharga);
                            cmd.Parameters.AddWithValue("@Status", t.statuspembayaran);
                            cmd.Parameters.AddWithValue("@Metode", t.metodepembayaran ?? (object)DBNull.Value);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        tx.Commit();
                        return newId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Update status pembayaran
        public void UpdateStatusPembayaran(int transaksiId, string status, string metode)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE transaksi SET statuspembayaran=@Status, metodepembayaran=@Metode WHERE id=@Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Metode", metode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", transaksiId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Transaksi MapTransaksi(NpgsqlDataReader reader)
        {
            return new Transaksi
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                kodebooking = reader.GetString(reader.GetOrdinal("kodebooking")),
                userId = reader.GetInt32(reader.GetOrdinal("user_id")),
                tiketId = reader.GetInt32(reader.GetOrdinal("tiket_id")),
                jumlahtiket = reader.GetInt32(reader.GetOrdinal("jumlahtiket")),
                totalharga = reader.GetDecimal(reader.GetOrdinal("totalharga")),
                statuspembayaran = reader.GetString(reader.GetOrdinal("statuspembayaran")),
                metodepembayaran = reader.IsDBNull(reader.GetOrdinal("metodepembayaran")) ? "" : reader.GetString(reader.GetOrdinal("metodepembayaran")),
                tanggaltransaksi = reader.GetDateTime(reader.GetOrdinal("tanggaltransaksi")),
                namaacara = reader.GetString(reader.GetOrdinal("namaacara")),
                lokasitiket = reader.GetString(reader.GetOrdinal("lokasitiket")),
                tanggalacara = reader.GetDateTime(reader.GetOrdinal("tanggalacara")),
                kategoritiket = reader.GetString(reader.GetOrdinal("kategoritiket"))
            };
        }
    }
}
