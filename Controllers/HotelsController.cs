using AutoMapper;
using HotelListing.API.DataAccessLayer.DTOs.Hotels;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IMapper _mapper;

        public HotelsController(IHotelsRepository hotelsRepository, IMapper mapper)
        {
            this._hotelsRepository = hotelsRepository;
            this._mapper = mapper;
        }

        // GET: api/<HotelsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            var GetAllHotels = await _hotelsRepository.GetAllAsync();

            var AllHotels = _mapper.Map<List<HotelDTO>>(GetAllHotels);

            return Ok(AllHotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDTO>> GetHotel(int id)
        {
            var hotelSearchedFor = await _hotelsRepository.GetAsync(id);

            var hotelFound = _mapper.Map<HotelDTO>(hotelSearchedFor);

            return Ok(hotelFound);
        }

        // POST api/<HotelsController>
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDTO DTO)
        {
            var newHotel = _mapper.Map<Hotel>(DTO);

            await _hotelsRepository.AddAsync(newHotel);

            var _DTO = _mapper.Map<CreateHotelDTO>(DTO);

            return Ok(_DTO);
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDTO DTO)
        {
            if (id != DTO.Id) return BadRequest("Invalid Record ID");

            var hotelSearchedFor = await _hotelsRepository.GetAsync(id);

            if (hotelSearchedFor == null) return NotFound();

            _mapper.Map(DTO, hotelSearchedFor);

            try
            {
                await _hotelsRepository.UpdateAsync(hotelSearchedFor);
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!await HotelExists(id))
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

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private Task<bool> HotelExists(int id)
        {
            return _hotelsRepository.Exists(id);
        }
    }
}
