using HotelListing.API.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HotelListing.API.DataAccessLayer.Configurations
{
    public class CountriesConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
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
        }
    }
}
