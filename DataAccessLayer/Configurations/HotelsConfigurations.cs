using HotelListing.API.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.DataAccessLayer.Configurations
{
    public class HotelsConfigurations : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
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
