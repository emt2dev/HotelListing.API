using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DataAccessLayer.DTOs.APIUsers
{
    public class APIUserLoginDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
