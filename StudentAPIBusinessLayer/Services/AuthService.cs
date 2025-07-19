using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAPIBusinessLayer.DTO.Auth;
using StudentAPIBusinessLayer.Helpers;
using StudentAPIBusinessLayer.Interfaces;
using StudentDataAccessLayer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAPIBusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService( UserManager<ApplicationUser> userManager,
            IOptions< JwtSettings> jwtSettings , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || !await _roleManager.RoleExistsAsync(model.RoleName))
                return "User or Role not found.";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return ("User already has this role.");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return errors;
            }

            return($"Role '{model.RoleName}' added to user '{user.UserName}'.");
        }

        public async Task<AuthModel> GetTokentAsync(TokenRequestModel model)
        {
            var authNodel = new AuthModel();

            if( string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                authNodel.Message = "Email and Password are required";
                authNodel.IsAuthenticated = false;
                return authNodel;
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authNodel.Message = "Invalid Email or Password";
                authNodel.IsAuthenticated = false;
                return authNodel;
            }

            var token = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            authNodel.IsAuthenticated = true;
            authNodel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authNodel.UserName = user.UserName;
            authNodel.Email = user.Email;
            authNodel.Expiration = token.ValidTo;
            authNodel.Roles = [.. roles];

            return authNodel;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            //  تحقق من البريد
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthModel
                {
                    Message = "Email is already in use",
                    IsAuthenticated = false
                };

            //  تحقق من اسم المستخدم
            if (await _userManager.FindByNameAsync(model.UserName) != null)
                return new AuthModel
                {
                    Message = "Username is already taken",
                    IsAuthenticated = false
                };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new AuthModel
                {
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)),
                    IsAuthenticated = false
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            //  أنشئ التوكن
            var token = await CreateJwtToken(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                UserName = user.UserName,
                Email = user.Email,
                Roles = ["User"],
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName!),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("uid", user.Id)
            };

            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: creds
            );
        }

    }
}