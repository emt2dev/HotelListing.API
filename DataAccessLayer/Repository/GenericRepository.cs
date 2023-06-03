using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        public GenericRepository(HotelListingDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            // async keyword automatically maps to correct type _context.Countries.AddAsync(country)
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync(); // saves to the db

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var EntitySearchingFor = await GetAsync(id);
            _context.Set<T>().Remove(EntitySearchingFor); // this cannot be called asynchronously

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            var EntitySearchingFor = await GetAsync(id);

            return EntitySearchingFor != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync(); // gets the dbset of type given
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id is null) return null;

            return await _context.Set<T>().FindAsync(id); // returns one
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity); // updates entity state to modified
            await _context.SaveChangesAsync();
        }
    }
}
