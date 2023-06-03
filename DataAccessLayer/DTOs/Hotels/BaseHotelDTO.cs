using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DataAccessLayer.DTOs.Hotels
{
    public class BaseHotelDTO
    {
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public int CountryId { get; set; }
    }
}
