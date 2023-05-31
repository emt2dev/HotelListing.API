using HotelListing.API.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataLayer
{
    public class HotelListingDbContext : DbContext
    {
        public HotelListingDbContext(DbContextOptions options) : base (options)
        {
        
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    Abbreviation = "JM"
                },
                 new Country
                 {
                     Id = 2,
                     Name = "South Korea",
                     Abbreviation = "KOR"
                 },
                new Country
                {
                    Id = 3,
                    Name = "France",
                    Abbreviation = "FR"
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Seoul Wyndam",
                    Address = "1234 Main Street, Virginia Beach, VA 23452",
                    Rating = 3.5,
                    CountryId = 1,
                },
                new Hotel
                {
                    Id = 2,
                    Name = "The DOG Street Inn",
                    Address = "896 Duke Of Gloucester Street, Williamsburg, VA 23185",
                    Rating = 4.5,
                    CountryId = 2,
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Champee All I Say",
                    Address = "455 Champse El-Assey, Paris 33442",
                    Rating = 5,
                    CountryId = 3,
                }
            );
        }
    }
}
