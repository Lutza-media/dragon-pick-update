using System;
using System.Collections.Generic;
using Npgsql;
using sipetra.Models;

namespace sipetra.Helpers
{
    public class TiketContext
    {
        public List<Tiket> GetAllTiket()
        {
            var list = new List<Tiket>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, kodetiket, namaacara, lokasi, tanggal, kategori, harga, stok, deskripsi FROM tiket ORDER BY tanggal";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapTiket(reader));
                    }
                }
            }
            return list;
        }

        public Tiket GetTiketById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, kodetiket, namaacara, lokasi, tanggal, kategori, harga, stok, deskripsi FROM tiket WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapTiket(reader);
                    }
                }
            }
            return null;
        }

        public void AddTiket(Tiket t)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO tiket (kodetiket, namaacara, lokasi, tanggal, kategori, harga, stok, deskripsi)
                                 VALUES (@KodeTiket, @NamaAcara, @Lokasi, @Tanggal, @Kategori, @Harga, @Stok, @Deskripsi)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KodeTiket", t.kodetiket);
                    cmd.Parameters.AddWithValue("@NamaAcara", t.namaacara);
                    cmd.Parameters.AddWithValue("@Lokasi", t.lokasi);
                    cmd.Parameters.AddWithValue("@Tanggal", t.tanggal);
                    cmd.Parameters.AddWithValue("@Kategori", t.kategori);
                    cmd.Parameters.AddWithValue("@Harga", t.harga);
                    cmd.Parameters.AddWithValue("@Stok", t.stok);
                    cmd.Parameters.AddWithValue("@Deskripsi", t.deskripsi ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTiket(Tiket t)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE tiket SET kodetiket=@KodeTiket, namaacara=@NamaAcara, lokasi=@Lokasi,
                                 tanggal=@Tanggal, kategori=@Kategori, harga=@Harga, stok=@Stok, deskripsi=@Deskripsi
                                 WHERE id=@Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", t.id);
                    cmd.Parameters.AddWithValue("@KodeTiket", t.kodetiket);
                    cmd.Parameters.AddWithValue("@NamaAcara", t.namaacara);
                    cmd.Parameters.AddWithValue("@Lokasi", t.lokasi);
                    cmd.Parameters.AddWithValue("@Tanggal", t.tanggal);
                    cmd.Parameters.AddWithValue("@Kategori", t.kategori);
                    cmd.Parameters.AddWithValue("@Harga", t.harga);
                    cmd.Parameters.AddWithValue("@Stok", t.stok);
                    cmd.Parameters.AddWithValue("@Deskripsi", t.deskripsi ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTiket(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM tiket WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Tiket MapTiket(NpgsqlDataReader reader)
        {
            return new Tiket
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                kodetiket = reader.GetString(reader.GetOrdinal("kodetiket")),
                namaacara = reader.GetString(reader.GetOrdinal("namaacara")),
                lokasi = reader.GetString(reader.GetOrdinal("lokasi")),
                tanggal = reader.GetDateTime(reader.GetOrdinal("tanggal")),
                kategori = reader.GetString(reader.GetOrdinal("kategori")),
                harga = reader.GetDecimal(reader.GetOrdinal("harga")),
                stok = reader.GetInt32(reader.GetOrdinal("stok")),
                deskripsi = reader.IsDBNull(reader.GetOrdinal("deskripsi")) ? "" : reader.GetString(reader.GetOrdinal("deskripsi"))
            };
        }
    }
}
