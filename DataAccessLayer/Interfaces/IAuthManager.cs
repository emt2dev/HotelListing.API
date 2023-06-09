﻿using HotelListing.API.DataAccessLayer.DTOs.APIUsers;
using HotelListing.API.DataAccessLayer.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.DataAccessLayer.Interfaces
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> RegisterNewUser(APIUserDTO DTO);
        Task<IEnumerable<IdentityError>> RegisterNewAdmin(APIUserDTO DTO);

        Task<AuthResponseDTO> LoginUser(APIUserLoginDTO DTO);

        Task<string> CreateRefreshToken();
        Task<AuthResponseDTO> VerifyRefreshToken(AuthResponseDTO DTO);

    }
}
