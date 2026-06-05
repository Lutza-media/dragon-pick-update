namespace sipetra.Models
{
    public class User
    {
        public int id { get; set; }
        public string nama { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string noHp { get; set; }
        public string alamat { get; set; }
        public string fotoProfil { get; set; }
        public bool isAdmin { get; set; }
    }
}
