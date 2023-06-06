using HotelListing.API.DataAccessLayer.Configurations;
using HotelListing.API.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataLayer
{
    public class HotelListingDbContext : IdentityDbContext<APIUser>
    {
        public HotelListingDbContext(DbContextOptions options) : base (options)
        {
        
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Role Seeder */
            modelBuilder.ApplyConfiguration(new RoleConfiguration()); // seeds the roles table

            /* Country, Hotel  Seeder */
            modelBuilder.ApplyConfiguration(new CountriesConfiguration()); // seeds the roles table
            modelBuilder.ApplyConfiguration(new HotelsConfigurations()); // seeds the roles table


        }
    }
}
