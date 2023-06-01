using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DataAccessLayer.DTOs.Countries
{
    public class UpdateCountryDTO : BaseCountryDTO
    {
        public int Id { get; set; }
    }
}
