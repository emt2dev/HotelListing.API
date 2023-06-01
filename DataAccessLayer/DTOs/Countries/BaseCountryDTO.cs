using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DataAccessLayer.DTOs.Countries
{
    public abstract class BaseCountryDTO
    {
        [Required]
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
