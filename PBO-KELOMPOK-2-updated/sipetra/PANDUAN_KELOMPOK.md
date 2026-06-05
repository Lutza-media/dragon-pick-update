# 📋 PANDUAN SIPETRA - Sistem Pemesanan Tiket
## PBO Kelompok 2

---

## 🗂️ STRUKTUR FILE & SIAPA YANG MENGERJAKAN

```
sipetra/sipetra/
│
├── Models/
│   ├── User.cs                  ← GANTI dengan versi baru (tambah field noHp, alamat)
│   ├── Tiket.cs                 ← BARU - buat file ini
│   └── Transaksi.cs             ← BARU - buat file ini
│
├── Helpers/
│   ├── DatabaseHelper.cs        ← Tidak perlu diubah (sudah ada)
│   ├── UserSession.cs           ← Tidak perlu diubah (sudah ada)
│   ├── UserContext.cs           ← GANTI dengan versi baru (tambah UpdateProfil, dll)
│   ├── TiketContext.cs          ← BARU - buat file ini
│   └── TransaksiContext.cs      ← BARU - buat file ini
│
├── Controllers/
│   └── UserControllers.cs       ← Tidak perlu diubah (sudah ada)
│
├── Views/
│   ├── Form1.cs (.Designer.cs)  ← Tidak diubah (login - sudah beres)
│   ├── Form2.cs                 ← GANTI isi method dengan Form2_updated.cs
│   ├── Form3.cs                 ← GANTI isi method dengan Form3_updated.cs
│   │
│   ├── [LUTZA]
│   ├── FormRiwayatTransaksi.cs  ← BARU - buat file ini
│   └── FormDetailTransaksi.cs   ← BARU - buat file ini
│   │
│   ├── [SHEVA]
│   ├── FormPemesanan.cs         ← BARU - buat file ini
│   └── FormPembayaran.cs        ← BARU - buat file ini
│   │
│   ├── [DINI]
│   ├── FormProfil.cs            ← BARU - buat file ini
│   └── FormDataTransaksi.cs     ← BARU - buat file ini
│   │
│   └── [ZONA]
│       ├── FormDataTiket.cs     ← BARU - buat file ini
│       └── FormEditProfil.cs    ← BARU - buat file ini
│
├── Program.cs                   ← Tidak diubah
└── database_schema.sql          ← BARU - jalankan di pgAdmin/psql
```

---

## 🗄️ SETUP DATABASE (SEMUA ANGGOTA HARUS LAKUKAN INI)

1. Buka **pgAdmin** atau **psql**
2. Pastikan database `sipetra` sudah ada, atau buat dulu:
   ```sql
   CREATE DATABASE sipetra;
   ```
3. Jalankan file **`database_schema.sql`** yang sudah disediakan
4. Ini akan membuat tabel: `users`, `tiket`, `transaksi`
5. Data dummy tiket sudah otomatis dimasukkan

> Catatan: Kolom `no_hp`, `alamat`, `foto_profil` ditambah ke tabel `users`.
> Kalau sudah ada tabel `users` sebelumnya, jalankan:
> ```sql
> ALTER TABLE users ADD COLUMN IF NOT EXISTS no_hp VARCHAR(20);
> ALTER TABLE users ADD COLUMN IF NOT EXISTS alamat TEXT;
> ALTER TABLE users ADD COLUMN IF NOT EXISTS foto_profil TEXT;
> ```

---

## 👩‍💻 PETUNJUK PER ANGGOTA

---

### 🔵 LUTZA — Riwayat Transaksi & Detail Transaksi

**File yang dibuat:**
- `Views/FormRiwayatTransaksi.cs`
- `Views/FormDetailTransaksi.cs`

**Cara menyambungkan ke Form3:**
Di `Form3.cs`, di method `button2_Click`, sudah ada kode untuk membuka `FormRiwayatTransaksi`.
Pastikan `button2` di Form3 sudah dihubungkan ke event `button2_Click`.

**Fitur yang sudah ada:**
- Tabel daftar transaksi user yang login
- Filter berdasarkan status (Semua / Pending / Lunas / Batal)
- Double-click baris → buka detail transaksi
- Warna baris sesuai status
- Di detail transaksi: tombol "Bayar Sekarang" untuk yang masih pending

---

### 🟢 SHEVA — Pembayaran & Transaksi Pemesanan

**File yang dibuat:**
- `Views/FormPemesanan.cs`
- `Views/FormPembayaran.cs`

**Cara membuka FormPemesanan:**
Dari halaman daftar tiket (Form4 atau halaman lainnya), panggil:
```csharp
var tiket = /* tiket yang dipilih user */;
var form = new FormPemesanan(tiket, _userLogin);
form.ShowDialog();
```

**Alur:**
1. User pilih tiket → Form4 buka `FormPemesanan`
2. User pilih jumlah tiket & klik "Pesan Sekarang"
3. Otomatis pindah ke `FormPembayaran`
4. User pilih metode bayar → klik "Konfirmasi Bayar"
5. Status transaksi berubah jadi "lunas"

**Di Form4 (halaman daftar tiket):**
Tambahkan kode berikut saat user klik tombol "Pesan":
```csharp
private void btnPesanTiket_Click(object sender, EventArgs e)
{
    // tiketDipilih = tiket yang dipilih dari tabel
    var formPemesanan = new FormPemesanan(tiketDipilih, UserSession.Instance.CurrentUser);
    formPemesanan.ShowDialog();
}
```

---

### 🟣 DINI — Halaman Profil & Data Transaksi (Admin)

**File yang dibuat:**
- `Views/FormProfil.cs`
- `Views/FormDataTransaksi.cs`

**Cara membuka FormProfil dari Form3:**
Di `Form3.cs`, tambahkan handler untuk `button3`:
```csharp
private void button3_Click(object sender, EventArgs e)
{
    var form = new FormProfil(_userLogin);
    form.ShowDialog();
}
```
Lalu di `Form3.Designer.cs`, sambungkan event:
```csharp
button3.Click += button3_Click;
```

**FormDataTransaksi** adalah halaman admin.
Buka dari Form4 (admin panel) atau dari menu khusus admin:
```csharp
var form = new FormDataTransaksi();
form.ShowDialog();
```

**Fitur:**
- FormProfil: tampilkan data user, tombol edit profil, riwayat transaksi, logout
- FormDataTransaksi: tabel semua transaksi, statistik (lunas/pending/batal/pendapatan), search

---

### 🟠 ZONA — Data Tiket & Edit Profil

**File yang dibuat:**
- `Views/FormDataTiket.cs`
- `Views/FormEditProfil.cs`

**FormDataTiket** adalah halaman admin untuk CRUD tiket.
Buka dari Form4 (admin panel):
```csharp
var form = new FormDataTiket();
form.ShowDialog();
```

**FormEditProfil** dibuka dari FormProfil:
```csharp
// Sudah otomatis dipanggil dari tombol "Edit Profil" di FormProfil
var editForm = new FormEditProfil(_user);
editForm.ShowDialog();
```

**Fitur FormDataTiket:**
- Tabel daftar tiket dengan warna kategori (VIP = kuning, VVIP = emas)
- Form input di kanan: Tambah / Update / Hapus tiket
- Klik baris tabel → data otomatis terisi di form

**Fitur FormEditProfil:**
- Edit nama, email, nomor HP, alamat
- Ganti password (dengan validasi password lama)

---

## 🔗 KONEKSI ANTAR FORM (RINGKASAN)

```
Form1 (Login)
    ↓ login berhasil
Form3 (Dashboard)
    ├── button2 → FormRiwayatTransaksi [LUTZA]
    │       └── double click → FormDetailTransaksi [LUTZA]
    │                └── "Bayar Sekarang" → FormPembayaran [SHEVA]
    ├── button3 → FormProfil [DINI]
    │       └── "Edit Profil" → FormEditProfil [ZONA]
    └── button4 → Logout → Form1

Form4 (Daftar Tiket / Admin Panel)
    ├── Klik tiket → FormPemesanan [SHEVA]
    │       └── setelah pesan → FormPembayaran [SHEVA]
    ├── (Admin) → FormDataTiket [ZONA]
    └── (Admin) → FormDataTransaksi [DINI]
```

---

## ⚠️ CATATAN PENTING

1. **Namespace**: Beberapa form lama pakai `namespace sipetra`, yang baru pakai `namespace sipetra.Views`. Pastikan konsisten.

2. **Form2.cs** — ganti hanya bagian isi method `button2_Click` dan `Form2_Load`. Jangan ubah file `Form2.Designer.cs`.

3. **Form3.cs** — cukup tambah handler untuk button3 (profil). Handler button2 (riwayat) dan button4 (logout) sudah disediakan.

4. **Database password** — di `DatabaseHelper.cs` password adalah `k3qingwanggy`. Sesuaikan dengan password PostgreSQL masing-masing.

5. **Setiap form baru** tidak punya file `.Designer.cs` karena UI dibuat langsung di code (di method `InitializeComponent()`). Ini legal dan aman — Visual Studio tetap bisa membukanya.
