using HotelListing.API.DataAccessLayer.Interfaces;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class CountriesRepository<Country> : ICountriesRepository
    {
        public Task<Models.Country> AddAsync(Models.Country entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Models.Country>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Models.Country> GetAsync(Models.Country entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Models.Country entity)
        {
            throw new NotImplementedException();
        }
    }
}
