using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        /// <summary>Đăng ký tài khoản mới</summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _auth.RegisterAsync(dto);
            return CreatedAtAction(nameof(Login), result);
        }

        /// <summary>Đăng nhập, lấy JWT token</summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _auth.LoginAsync(dto);
            return Ok(result);
        }
    }
}
