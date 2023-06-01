using HotelListing.API.DataAccessLayer.Interfaces;

namespace HotelListing.API.DataAccessLayer.Repository
{
    public class HotelsRepository<Hotel> : IHotelsRepository
    {
        public Task<Models.Hotel> AddAsync(Models.Hotel entity)
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

        public Task<List<Models.Hotel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Models.Hotel> GetAsync(Models.Hotel entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Models.Hotel entity)
        {
            throw new NotImplementedException();
        }
    }
}
