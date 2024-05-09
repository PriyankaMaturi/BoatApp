namespace Boat_Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public byte[] Salt { get; set; }
        public byte[] Hash { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
 
        public string Phonenumber { get; set; }
        public string Address { get; set; }

        public string Cart { get; set; }

    }
}
