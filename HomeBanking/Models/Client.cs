using System.Collections.Generic;

namespace HomeBanking.Models
{
    public class Client
    {
        //escribir prop es un atajo para crear atributos
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
