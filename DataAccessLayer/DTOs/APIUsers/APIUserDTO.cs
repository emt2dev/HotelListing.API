using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DataAccessLayer.DTOs.APIUsers
{
    public class APIUserDTO : APIUserLoginDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}