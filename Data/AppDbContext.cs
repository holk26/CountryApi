// CountryApi/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace CountryApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define las propiedades DbSet para las entidades (Country, Hotel, Restaurant)
        public DbSet<CountryApi.Models.Country> Countries { get; set; }
        public DbSet<CountryApi.Models.Hotel> Hotels { get; set; }
        public DbSet<CountryApi.Models.Restaurant> Restaurants { get; set; }
    }
}
