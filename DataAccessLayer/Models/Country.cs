namespace HotelListing.API.DataAccessLayer.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public virtual IList<Hotel> Hotels { get; set; }
    }
}