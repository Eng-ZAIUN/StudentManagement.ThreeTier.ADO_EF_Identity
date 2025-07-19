using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAPIBusinessLayer.DTO.Auth;
using StudentAPIBusinessLayer.Interfaces;
using StudentAPIBusinessLayer.Services;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new {token = result .Token, expireson = result .Expiration});
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetRegisterAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokentAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.RoleName))
                return BadRequest("User ID and Role Name are required.");

            var result = await _authService.AddRoleAsync(model);

            if (string.IsNullOrEmpty(result))
                return BadRequest("Something went wrong");

            return Ok(new
            {
                model.UserId,
                model.RoleName
            });
        }

    }
}
