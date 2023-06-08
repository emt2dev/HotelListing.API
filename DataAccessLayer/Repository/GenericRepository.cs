using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Pagination;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<T>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync(); // gets the dbset of type given
        }

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters QP)
        {
            var totalSize = await _context.Set<T>().CountAsync();
            var records = await _context.Set<T>()
                .Skip(QP.NextPageNumber)
                .Take(QP.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TResult>
            {
                Records = records,
                PageNumber = QP.NextPageNumber,
                TotalCount = totalSize
            };
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
