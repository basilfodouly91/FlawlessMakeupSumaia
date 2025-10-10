using Microsoft.AspNetCore.Mvc;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

            var response = new AuthResponseDto
            {
                Success = result.Success,
                Token = result.Token,
                Message = result.Message,
                User = result.User?.ToDto()
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

            var response = new AuthResponseDto
            {
                Success = result.Success,
                Token = result.Token,
                Message = result.Message,
                User = result.User?.ToDto()
            };

            if (result.Success)
                return Ok(response);
            else
                return Unauthorized(response);
        }
    }
}
