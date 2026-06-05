-- =============================================
-- SIPETRA - Sistem Pemesanan Tiket
-- Database: PostgreSQL
-- Jalankan script ini di pgAdmin atau psql
-- =============================================

-- Buat database (jalankan terpisah jika perlu)
-- CREATE DATABASE sipetra;

-- Tabel users (sudah ada, pastikan strukturnya sesuai)
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    nama VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    no_hp VARCHAR(20),
    alamat TEXT,
    foto_profil TEXT,
    is_admin BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel tiket
CREATE TABLE IF NOT EXISTS tiket (
    id SERIAL PRIMARY KEY,
    kodetiket VARCHAR(20) UNIQUE NOT NULL,
    namaacara VARCHAR(200) NOT NULL,
    lokasi VARCHAR(200) NOT NULL,
    tanggal TIMESTAMP NOT NULL,
    kategori VARCHAR(20) NOT NULL CHECK (kategori IN ('reguler', 'vip', 'vvip')),
    harga NUMERIC(12,2) NOT NULL,
    stok INTEGER NOT NULL DEFAULT 0,
    deskripsi TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel transaksi
CREATE TABLE IF NOT EXISTS transaksi (
    id SERIAL PRIMARY KEY,
    kodebooking VARCHAR(20) UNIQUE NOT NULL,
    user_id INTEGER NOT NULL REFERENCES users(id),
    tiket_id INTEGER NOT NULL REFERENCES tiket(id),
    jumlahtiket INTEGER NOT NULL DEFAULT 1,
    totalharga NUMERIC(12,2) NOT NULL,
    statuspembayaran VARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (statuspembayaran IN ('pending', 'lunas', 'batal')),
    metodepembayaran VARCHAR(30),
    tanggaltransaksi TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Data contoh tiket
INSERT INTO tiket (kodetiket, namaacara, lokasi, tanggal, kategori, harga, stok, deskripsi)
VALUES
  ('TKT-001', 'Konser Dewa 19', 'GBK Jakarta', '2025-08-15 19:00', 'reguler', 250000, 100, 'Konser reuni Dewa 19'),
  ('TKT-002', 'Konser Dewa 19', 'GBK Jakarta', '2025-08-15 19:00', 'vip', 500000, 50, 'Area VIP dengan kursi premium'),
  ('TKT-003', 'Festival Musik Nusantara', 'Alun-alun Bandung', '2025-09-01 17:00', 'reguler', 150000, 200, 'Festival musik berbagai genre'),
  ('TKT-004', 'Pertandingan Bulu Tangkis', 'Istora Senayan', '2025-07-20 10:00', 'reguler', 100000, 300, 'Kejuaraan nasional bulu tangkis'),
  ('TKT-005', 'Pameran Seni Rupa', 'Galeri Nasional', '2025-07-25 09:00', 'reguler', 50000, 500, 'Pameran karya seniman lokal');

-- Data contoh user admin
INSERT INTO users (nama, email, password, is_admin)
VALUES ('Admin Sipetra', 'admin@sipetra.com', 'admin123', TRUE)
ON CONFLICT (email) DO NOTHING;
