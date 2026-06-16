using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;

        public UserController(IUserService users) => _users = users;

        /// <summary>Lấy thông tin user hiện tại (từ token)</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _users.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        /// <summary>Lấy danh sách tất cả users (Admin only)</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _users.GetAllAsync();
            return Ok(users);
        }

        /// <summary>Lấy user theo ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        /// <summary>Cập nhật thông tin user</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _users.UpdateAsync(id, dto);
            return Ok(updated);
        }

        /// <summary>Xóa user (Admin only)</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _users.DeleteAsync(id);
            return NoContent();
        }
    }
}
