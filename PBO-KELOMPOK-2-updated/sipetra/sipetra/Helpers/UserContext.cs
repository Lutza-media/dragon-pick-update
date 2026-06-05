using System;
using System.Collections.Generic;
using Npgsql;
using sipetra.Models;

namespace sipetra.Helpers
{
    public class UserContext
    {
        public void AddUser(User user)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO users (nama, email, password, is_admin) VALUES (@Nama, @Email, @Password, @IsAdmin)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nama", user.nama);
                    cmd.Parameters.AddWithValue("@Email", user.email);
                    cmd.Parameters.AddWithValue("@Password", user.password);
                    cmd.Parameters.AddWithValue("@IsAdmin", user.isAdmin);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUserByEmailAndPassword(string email, string password)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, nama, email, password, no_hp, alamat, foto_profil, is_admin FROM users WHERE LOWER(email) = LOWER(@Email) AND password = @Password";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email?.Trim() ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Password", password?.Trim() ?? string.Empty);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapUser(reader);
                    }
                }
            }
        }

        public User GetUserById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, nama, email, password, no_hp, alamat, foto_profil, is_admin FROM users WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapUser(reader);
                    }
                }
            }
            return null;
        }

        public void UpdateProfil(User user)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE users SET nama=@Nama, email=@Email, no_hp=@NoHp, alamat=@Alamat WHERE id=@Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nama", user.nama);
                    cmd.Parameters.AddWithValue("@Email", user.email);
                    cmd.Parameters.AddWithValue("@NoHp", user.noHp ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Alamat", user.alamat ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", user.id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePassword(int userId, string passwordBaru)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE users SET password=@Password WHERE id=@Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", passwordBaru);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                nama = reader.GetString(reader.GetOrdinal("nama")),
                email = reader.GetString(reader.GetOrdinal("email")),
                password = reader.GetString(reader.GetOrdinal("password")),
                noHp = reader.IsDBNull(reader.GetOrdinal("no_hp")) ? "" : reader.GetString(reader.GetOrdinal("no_hp")),
                alamat = reader.IsDBNull(reader.GetOrdinal("alamat")) ? "" : reader.GetString(reader.GetOrdinal("alamat")),
                isAdmin = reader.GetBoolean(reader.GetOrdinal("is_admin"))
            };
        }
    }
}
