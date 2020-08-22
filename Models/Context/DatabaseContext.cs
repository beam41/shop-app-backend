using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ShopAppBackend.Models.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<User> User { get; set; }
        
    }
}
