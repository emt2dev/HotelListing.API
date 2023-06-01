using AutoMapper;
using HotelListing.API.DataAccessLayer.DTOs.Countries;
using HotelListing.API.DataAccessLayer.DTOs.Hotels;
using HotelListing.API.DataAccessLayer.Models;

namespace HotelListing.API.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            /* Country DTOs */
            // R
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Country, CountryDetailsDTO>().ReverseMap();

            // C and U
            CreateMap<Country, CreateCountryDTO>().ReverseMap();
            CreateMap<Country, UpdateCountryDTO>().ReverseMap();

            /* Hotel DTOs */
            CreateMap<Hotel, HotelDTO>().ReverseMap();
        }
    }
}
