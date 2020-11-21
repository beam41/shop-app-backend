using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Enums;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Models.DTOs;
using ShopAppBackend.Services;

namespace ShopAppBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _authService;

        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserFormDto>> GetUser(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return BadRequest();

            var user = await _context.User.Select(u => new UserFormDto
            {
                Id = u.Id,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                FullName = u.FullName,
                Address = u.Address,
                Province = u.Province,
                District = u.District,
                SubDistrict = u.SubDistrict,
                PostalCode = u.PostalCode
            }).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return user;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserListDto>>> GetUserList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return BadRequest();

            return await _context.User.Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                ActiveOrders = u.Orders.Count(o => o.OrderStates.All(os => os.State != OrderStateEnum.Received))
            }).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("check-exist")]
        public async Task<ActionResult> CheckUserExist()
        {
            if (await UserExist(Request.Query["username"])) return Ok(new { result = true });

            return Ok(new { result = false });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserLoginDto>> Register(User user)
        {
            user.Password = _authService.HashPassword(user.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            UserLoginDto newUser = user;

            _authService.GenToken(newUser);

            return CreatedAtAction("Login", newUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginDto>> Login(UserLoginFormDto userBody)
        {
            var passwordHash = _authService.HashPassword(userBody.Password);

            UserLoginDto user = await _context.User
                .FirstOrDefaultAsync(u =>
                    u.Username == userBody.Username &&
                    u.Password == passwordHash
                );

            // return null if user not found
            if (user == null) return Forbid();

            _authService.GenToken(user);

            return user;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, UserEditDto userInfo)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1 && id != tokenId) return BadRequest();

            User user;
            if (userInfo.NewPassword?.Length > 0)
            {
                if (tokenId != 1)
                {
                    var passwordHash = _authService.HashPassword(userInfo.Password);

                    user = await _context.User
                        .FirstOrDefaultAsync(u =>
                            u.Id == id &&
                            u.Password == passwordHash
                        );
                }
                else
                {
                    user = await _context.User
                        .FirstOrDefaultAsync(u =>
                            u.Id == id
                        );
                }

                if (user == null) return Forbid();

                user.Password = _authService.HashPassword(userInfo.NewPassword);
            }
            else
            {
                user = await _context.User
                    .FirstOrDefaultAsync(u =>
                        u.Id == id
                    );

                if (user == null) return Forbid();
            }

            user.PhoneNumber = userInfo.PhoneNumber;
            user.FullName = userInfo.FullName;
            user.Address = userInfo.Address;
            user.Province = userInfo.Province;
            user.District = userInfo.District;
            user.SubDistrict = userInfo.SubDistrict;
            user.PostalCode = userInfo.PostalCode;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var user = new User { Id = id };
            _context.Entry(user).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            return user;
        }

        private Task<bool> UserExist(string username)
        {
            return _context.User.AnyAsync(u => u.Username == username);
        }
    }
}
