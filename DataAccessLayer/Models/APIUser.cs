using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.DataAccessLayer.Models
{
    public class APIUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
