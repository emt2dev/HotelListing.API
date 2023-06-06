using AutoMapper;
using HotelListing.API.DataAccessLayer.DTOs.APIUsers;
using HotelListing.API.DataAccessLayer.DTOs.Auth;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.DataAccessLayer.Services
{
    public class AuthManager : IAuthManager
    {
        private IMapper _mapper;
        private UserManager<APIUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthManager(IMapper mapper, UserManager<APIUser> userManager, IConfiguration configuration)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<AuthResponseDTO> LoginUser(APIUserLoginDTO DTO)
        {
            var findUser = await _userManager.FindByEmailAsync(DTO.Email);

            bool isValidPassword = await _userManager.CheckPasswordAsync(findUser, DTO.Password);

            if (findUser == null || !isValidPassword) return default;

            var issueNewToken = await GenerateToken(findUser);

            return new AuthResponseDTO
            {
                Token = issueNewToken,
                UserId = findUser.Id
            };
        }

        public async Task<IEnumerable<IdentityError>> RegisterNewUser(APIUserDTO DTO)
        {
            var newUser = _mapper.Map<APIUser>(DTO);

            newUser.UserName = DTO.Email;

            var registerResult = await _userManager.CreateAsync(newUser, DTO.Password);

            if (registerResult.Succeeded) await _userManager.AddToRoleAsync(newUser, "User");

            return registerResult.Errors;
        }

        private async Task<string> GenerateToken(APIUser currentUser)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["JwtSettings:Key"]
                    )
                );

            var userCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var userRoleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            /* Below is the method to retrieve claims that are stored in database */
            var userClaims = await _userManager.GetClaimsAsync(currentUser);

            var userClaimsList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, currentUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, currentUser.Email),
            }
            .Union(userClaims)
            .Union(userRoleClaims);

            var newToken = new JwtSecurityToken(
                    issuer: _configuration["JwtSetting:Issuer"],
                    audience: _configuration["JwtSetting:Audience"],
                    claims: userClaimsList,
                    expires: DateTime.Now
                                .AddMinutes(Convert.ToInt32(
                                    _configuration["JwtSetting:DurationInMinutes"])),
                    signingCredentials: userCredentials

                );

            return new JwtSecurityTokenHandler()
                .WriteToken(newToken);
        }
    }
}
