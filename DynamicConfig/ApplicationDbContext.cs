using DynamicConfig.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Config> Configs => Set<Config>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Entity<Config>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).UseIdentityByDefaultColumn();                                                                    
                b.HasIndex(x => new { x.ApplicationName, x.Name }).IsUnique();
            });
        }
    }
}
