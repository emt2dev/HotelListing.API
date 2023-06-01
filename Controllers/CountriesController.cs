using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.DataLayer;

using HotelListing.API.DataAccessLayer.Models;
using AutoMapper;
using HotelListing.API.DataAccessLayer.DTOs.Countries;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(HotelListingDbContext context, IMapper autoMapper)
        {
            _context = context;
            _mapper = autoMapper;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDTO>>> GetCountries()
        {
            var GetAllCountries = await _context.Countries.ToListAsync();

            var AllCountries = _mapper.Map<List<CountryDTO>>(GetAllCountries);

            return Ok(AllCountries);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDTO>> GetCountry(int id)
        {
            /* Below we only the country row */
            // var countrySearchedFor = await _context.Countries.FindAsync(id);

            /* Below we include the first hotel inside of the country row */
            var countrySearchedFor = await _context.Countries.Include(countryFound => countryFound.Hotels)
                .FirstOrDefaultAsync(hotelFound => hotelFound.Id == id);

            if (countrySearchedFor == null) return NotFound();

            var countryFoundDTO = _mapper.Map<CountryDetailsDTO>(countrySearchedFor);

            return Ok(countryFoundDTO);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO DTO)
        {
            if (id != DTO.Id) return BadRequest();

            /* Becomes tracking, we want to disable tracking */
            // _context.Entry(DTO).State = EntityState.Modified;

            var countryToBeUpdated = await _context.Countries.FindAsync(id);

            if (countryToBeUpdated == null) return NotFound();

            // take the DTO information and then update the country row
            _mapper.Map(DTO, countryToBeUpdated);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDTO CreateCountryDTO)
        {
            /*
             * Removed thanks to AutoMapper
             * 
            var newCountry = new Country
            {
                Name = CreateCountryDTO.Name,
                Abbreviation = CreateCountryDTO.Abbreviation,
            };
            */

            var newCountry = _mapper.Map<Country>(CreateCountryDTO);

            _context.Countries.Add(newCountry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = newCountry.Id }, newCountry);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null) return NotFound();

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
