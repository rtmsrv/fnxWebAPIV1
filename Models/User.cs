namespace fnxWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; } // For demonstration only, avoid storing plain text passwords
    }
}
