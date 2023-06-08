using HotelListing.API.DataAccessLayer.DTOs.APIUsers;
using HotelListing.API.DataAccessLayer.DTOs.Auth;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthManager _iAuthManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager iAuthManager, ILogger<AccountController> logger)
        {
            this._iAuthManager = iAuthManager;
            this._logger = logger;
        }

        /* POST: api/Account/register */
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay

        public async Task<ActionResult> Register([FromBody] APIUserDTO DTO)
        {
            _logger.LogInformation($"Failed Register Attempt for {DTO.Email}");

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

        /* POST: api/Account/login */
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay

        public async Task<ActionResult> Login([FromBody] APIUserLoginDTO DTO)
        {
            _logger.LogInformation($"Failed Login Attempt for {DTO.Email}");

            var authenticatedUser = await _iAuthManager.LoginUser(DTO);

            if (authenticatedUser == null) return Unauthorized();

            return Ok(authenticatedUser);
        }

        /* POST: api/Account/register */
        [HttpPost]
        [Route("registerdmin")]
        [Authorize(Roles = ("Administrator"))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay
        public async Task<ActionResult> RegisterAdmin([FromBody] APIUserDTO DTO)
        {
            _logger.LogInformation($"Failed Register Admin Attempt for {DTO.Email}");

            var errors = await _iAuthManager.RegisterNewAdmin(DTO);

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

        /* POST: api/Account/refreshToken */
        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // if validation fails, send this
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // If client issues
        [ProducesResponseType(StatusCodes.Status200OK)] // if okay

        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDTO DTO)
        {
            _logger.LogInformation($"Failed Refresh Token Attempt for {DTO.UserId}");

            var authenticatedUser = await _iAuthManager.VerifyRefreshToken(DTO);

            if (authenticatedUser == null) return Unauthorized();

            return Ok(authenticatedUser);
        }
    }
}
