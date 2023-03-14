using Microsoft.EntityFrameworkCore;

using Reservations.Data.Models;

namespace Reservations.Data
{
    public class ReservationsContext : DbContext
    {
        public DbSet<AvailabilityWindow> AvailabilityWindow { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public ReservationsContext(DbContextOptions<ReservationsContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Get the directory containing the executing assembly
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dbPath = Path.Combine(assemblyDirectory, "reservations.db");

            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .Property(r => r.Expired)
                .HasComputedColumnSql("CASE WHEN CreatedAt < DATETIME('now', '-30 minutes') THEN 1 ELSE 0 END");
        }
    }
}
