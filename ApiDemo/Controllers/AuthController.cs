using Application.Commands.Auth;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), 201)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await mediator.Send(new RegisterCommand(dto.Username, dto.Email, dto.Password));
        return CreatedAtAction(nameof(Login), result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCommand(dto.Email, dto.Password));
        return Ok(result);
    }
}