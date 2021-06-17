using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class UserAccountDbContext : DbContext
    {
        public UserAccountDbContext(DbContextOptions<UserAccountDbContext> options) : base(options) { }

        public DbSet<UserAccount> Account { get; set; }
    }
}
