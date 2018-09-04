using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
    public class AccountSecurity
    {
        public int Id { get; set; }
        
        public Guid UserId { get; set;  }
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public List<Phone> Phones { get; set; }

    }
}
