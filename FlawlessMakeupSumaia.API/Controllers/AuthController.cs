using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match"
                });
            }

            var registerModel = new RegisterModel
            {
                Email = dto.Email,
                Password = dto.Password,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _authService.RegisterAsync(registerModel);

            UserDto? userDto = null;
            if (result.User != null)
            {
                var roles = await _userManager.GetRolesAsync(result.User);
                userDto = result.User.ToDto(roles.ToList());
            }

            var response = new AuthResponseDto
            {
                Success = result.Success,
                Token = result.Token,
                Message = result.Message,
                User = userDto
            };

            if (result.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var loginModel = new LoginModel
            {
                Email = dto.Email,
                Password = dto.Password
            };

            var result = await _authService.LoginAsync(loginModel);

            UserDto? userDto = null;
            if (result.User != null)
            {
                var roles = await _userManager.GetRolesAsync(result.User);
                userDto = result.User.ToDto(roles.ToList());
            }

            var response = new AuthResponseDto
            {
                Success = result.Success,
                Token = result.Token,
                Message = result.Message,
                User = userDto
            };

            if (result.Success)
                return Ok(response);
            else
                return Unauthorized(response);
        }
    }
}
