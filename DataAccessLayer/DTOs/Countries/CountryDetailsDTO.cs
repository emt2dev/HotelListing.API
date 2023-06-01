using HotelListing.API.DataAccessLayer.DTOs.Hotels;

namespace HotelListing.API.DataAccessLayer.DTOs.Countries
{
    public class CountryDetailsDTO : BaseCountryDTO
    {
        public int Id { get; set; }
        public List<HotelDTO> Hotels { get; set; }
    }
}
