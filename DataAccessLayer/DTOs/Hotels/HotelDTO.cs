using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.API.DataAccessLayer.DTOs.Hotels
{
    public class HotelDTO : BaseHotelDTO
    {
        public int Id { get; set; }
    }
}