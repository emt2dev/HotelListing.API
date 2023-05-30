using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataLayer
{
    public class HotelListingDbContext : DbContext
    {
        public HotelListingDbContext(DbContextOptions options) : base (options)
        {
        
        }
    }
}
