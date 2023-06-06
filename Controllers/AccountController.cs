using HotelListing.API.DataAccessLayer.DTOs.APIUsers;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthManager _iAuthManager;

        public AccountController(IAuthManager iAuthManager)
        {
            this._iAuthManager = iAuthManager;
        }

        /* POST: api/Account/register */
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay

        public async Task<ActionResult> Register([FromBody] APIUserDTO DTO)
        {
            var errors = await _iAuthManager.RegisterNewUser(DTO);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
        }

        /* POST: api/Account/register */
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay

        public async Task<ActionResult> Login([FromBody] APIUserLoginDTO DTO)
        {
            var authenticatedUser = await _iAuthManager.LoginUser(DTO);

            if (authenticatedUser == null) return Unauthorized();

            return Ok(authenticatedUser);
        }
    }
}
