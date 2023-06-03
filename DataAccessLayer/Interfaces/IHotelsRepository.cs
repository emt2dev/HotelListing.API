using HotelListing.API.DataAccessLayer.Models;

namespace HotelListing.API.DataAccessLayer.Interfaces
{
    public interface IHotelsRepository : IGenericRepository<Hotel>
    {
        // Task<Hotel> GetDetails(int? id);
    }
}
