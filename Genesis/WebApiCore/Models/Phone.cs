using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Value { get; set; }
    }
}
