using Application.Shared.Interfaces;
using Domain.IdentityEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)); //get key from appsettings.json
        }

        public string CreateToken(ApplicationUser user, int roleId, string roleName) //IList<string> roles
        {
            var authClaims = new List<Claim>
        {    //ClaimTypes.Name, ClaimTypes.NameIdentifier = long xml schema
            new Claim("username", user.UserName!), //username
            new Claim("userId", user.Id.ToString()), //id
            new Claim("email", user.Email!),
            new Claim("roleId", roleId.ToString()), 
            new Claim("roleName", roleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token id: "jti": "c8524302-378b-4a68-970b-1c7a0b990680",
        };

            //foreach (var role in roles)
            //{   //"role" insted of ClaimTypes.Role
            //    authClaims.Add(new Claim("role", role)); //add roles in token and allow [Authorize(Roles = "Admin")] in controller.
            //}

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(120), //10 min
                claims: authClaims,
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); //as string (Header.Payload.Signature).
        }
    }
}