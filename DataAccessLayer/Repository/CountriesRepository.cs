using AutoMapper;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;
        // means we use implementations from the inheritance interfaces/classes
        // ICountriesRepository uses GenericRepository<Country> to fill the types
        public CountriesRepository(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Country> GetDetails(int? id)
        {

            return await _context.Countries.Include(q => q.Hotels)
                .FirstOrDefaultAsync(q => q.Id == id); // returns one or null if doesn't exist.
        }
    }
}
