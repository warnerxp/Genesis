using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiCore.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options)
        :base(options)
        {

        }
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountSecurity> AccountSecurity { get; set; }
        public DbSet<Phone> Phone { get; set; }
        public DbSet<Token> Tokens { get; set; }
       
    }
}
