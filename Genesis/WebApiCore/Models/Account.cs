using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
    public class Account
    {
        public int Id { get; set; }
        public Guid IdUser { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdateOn { get; set; }
        public DateTime LastLoginOn { get; set; }
        public Guid TokenId { get; set; }
        
    }
}
