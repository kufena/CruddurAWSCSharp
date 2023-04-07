using CruddurSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruddurSQL
{
    public class ActivitiesDbContext : DbContext
    {
        private string connectionString;

        public ActivitiesDbContext(string connectionString) {
            this.connectionString = connectionString;
        }

        public DbSet<ActivitiesModel>? activties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivitiesModel>()
                .HasKey(obj => obj.uuid);
            modelBuilder.Entity<ActivitiesModel>()
                .Property(obj => obj.uuid).ValueGeneratedOnAdd();
            modelBuilder.Entity<ActivitiesModel>()
                .Property(x => x.uuid)
                .HasColumnType("uuid")
                .HasConversion<Guid>();
            modelBuilder.Entity<ActivitiesModel>()
                .Property(x => x.user_uuid)
                .HasColumnType("uuid")
                .HasConversion<Guid>();
        }

    }
}
