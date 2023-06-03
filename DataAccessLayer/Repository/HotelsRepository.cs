using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListingDbContext _context;
        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        /*
        public async Task<Hotel> GetDetails(int? id)
        {
            return await _context.Hotels
        }
        */
    }
}
