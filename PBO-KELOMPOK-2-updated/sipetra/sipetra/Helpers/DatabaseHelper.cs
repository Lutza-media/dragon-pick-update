using Npgsql;
using System.Collections.Generic;


namespace sipetra.Helpers
{
    public class DatabaseHelper
    {
        private static string connString = "Host=localhost;Port=5432;Database=sipetra;Username=postgres;Password=k3qingwanggy";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connString);
        }
    }
}