using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.DataLayer;

using HotelListing.API.DataAccessLayer.Models;
using AutoMapper;
using HotelListing.API.DataAccessLayer.DTOs.Countries;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Exceptions;
using HotelListing.API.DataAccessLayer.Pagination;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.API.Controllers
{

    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository; // the controller will never touch the context/sql
        private readonly IMapper _mapper;

        public CountriesV2Controller(ICountriesRepository countriesRepository, IMapper autoMapper)
        {
            this._countriesRepository = countriesRepository;
            this._mapper = autoMapper;
        }

        // GET: api/Countries
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CountryDTO>>> GetCountries()
        {

            // var GetAllCountries = await _context.Countries.ToListAsync();
            var GetAllCountries = await _countriesRepository.GetAllAsync<CountryDTO>();

            var AllCountries = _mapper.Map<List<CountryDTO>>(GetAllCountries);

            return Ok(AllCountries);
        }
        /*
        // GET: api/Countries/?StartIndex=0&pagesize=5&pagenumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<CountryDTO>>> GetPagedCountries([FromQuery] QueryParameters QP)
        {

            // var GetAllCountries = await _context.Countries.ToListAsync();
            var pagedResultCountries = await _countriesRepository.GetAllAsync<CountryDTO>(QP);

            return Ok(pagedResultCountries);
        }
        */

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDTO>> GetCountry(int id)
        {
            /* Below we only the country row */
            // var countrySearchedFor = await _context.Countries.FindAsync(id);

            /* Below we include the first hotel inside of the country row */
            // var countrySearchedFor = await _context.Countries.Include(countryFound => countryFound.Hotels).FirstOrDefaultAsync(hotelFound => hotelFound.Id == id);

            var countrySearchedFor = await _countriesRepository.GetDetails(id);

            var countryFoundDTO = _mapper.Map<CountryDetailsDTO>(countrySearchedFor);

            return Ok(countryFoundDTO);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO DTO)
        {
            if (id != DTO.Id) return BadRequest("Invalid Record Id");

            var countrySearchedFor = await _countriesRepository.GetAsync(id);

            if (countrySearchedFor == null) throw new NotFoundException(nameof(PutCountry), id);

            _mapper.Map(DTO, countrySearchedFor); // takes left object and puts in right object

            try
            {
                await _countriesRepository.UpdateAsync(countrySearchedFor);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();

            /*
             * Below is before repository pattern
             * 
            if (id != DTO.Id) return BadRequest();

            /* Becomes tracking, we want to disable tracking 
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
            */
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
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

            /*
             * Removed thanks to repository pattern
             * 
            var newCountry = _mapper.Map<Country>(CreateCountryDTO);

            _context.Countries.Add(newCountry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = newCountry.Id }, newCountry);
            */

            var newCountry = _mapper.Map<Country>(CreateCountryDTO);

            await _countriesRepository.AddAsync(newCountry);

            var _DTO = _mapper.Map<CreateCountryDTO>(newCountry);

            return Ok(_DTO);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCountry(int id)
        {

            var countrySearchedFor = await _countriesRepository.GetAsync(id);

            if (countrySearchedFor == null) throw new NotFoundException(nameof(DeleteCountry), id);

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private Task<bool> CountryExists(int id)
        {
            return _countriesRepository.Exists(id);
        }
    }
}
