using Microsoft.EntityFrameworkCore;
using Infrastructure.Storage.Entities;

namespace Infrastructure.Storage
{
    public class StorageContext : DbContext
    {
        public DbSet<BookingEntity>? BookingEntity { get; set; }
        public DbSet<EquipmentActivityEntity>? EquipmentActivity { get; set; }

        private string DbPath { get; }

        public StorageContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "app.db");

            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        // https://entityframeworkcore.com/providers-sqlite
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingEntity>().ToTable("BookingEntity");
            modelBuilder.Entity<EquipmentActivityEntity>().ToTable("EquipmentActivity");
        }
    }
}
