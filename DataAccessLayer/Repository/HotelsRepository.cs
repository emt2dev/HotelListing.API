using AutoMapper;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;
        public HotelsRepository(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        /*
        public async Task<Hotel> GetDetails(int? id)
        {
            return await _context.Hotels
        }
        */
    }
}
