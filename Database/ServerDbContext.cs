using Microsoft.EntityFrameworkCore;
using TransportRental.Car;
using TransportRental.Model;

namespace TransportRental.Database {
    public class ServerDbContext : DbContext {
        private const string Server = "localhost";
        private const string Username = "root";
        private const string Password = "";
        private const string DatabaseName = "transportrental";

        public DbSet<Car.Car> Transports { get; set; }
        public DbSet<RentedCar> RentedTransports { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Client> Clients { get; set; }

        public ServerDbContext()
        {
            EnsureDatabaseCreated();
        }

        public void EnsureDatabaseCreated() {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql($"server={Server};UserId={Username};Password={Password};database={DatabaseName};");
        }
    }
}