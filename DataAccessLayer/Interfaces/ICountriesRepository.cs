using HotelListing.API.DataAccessLayer.Models;

namespace HotelListing.API.DataAccessLayer.Interfaces
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<Country> GetDetails(int? id);
    }
}
