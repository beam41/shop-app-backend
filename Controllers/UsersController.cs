using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;
using ShopAppBackend.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly AuthService _authService;

        public UsersController(DatabaseContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: api/Users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserFormDTO>> GetUser(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1) return BadRequest();

            var user = await _context.User.Select(u => new UserFormDTO
            {
                Id = u.Id,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                FullName = u.FullName,
                Address = u.Address,
                Province = u.Province,
                District = u.District,
                SubDistrict = u.SubDistrict,
                PostalCode = u.PostalCode,
            }).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserListDTO>>> GetUserList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1) return BadRequest();

            return await _context.User.Select(u => new UserListDTO
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
        public async Task<ActionResult<UserLoginDTO>> CheckUserExist()
        {
            if (await UserExist(Request.Query["username"]))
            {
                return Ok(new {result = true});
            }

            return Ok(new {result = false});
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserLoginDTO>> Register(User user)
        {
            user.Password = _authService.HashPassword(user.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            UserLoginDTO newUser = user;

            _authService.GenToken(newUser);

            return CreatedAtAction("Login", newUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginDTO>> Login(UserLoginFormDTO userBody)
        {
            var passwordHash = _authService.HashPassword(userBody.Password);

            UserLoginDTO user = await _context.User
                .FirstOrDefaultAsync(u =>
                    u.Username == userBody.Username &&
                    u.Password == passwordHash
                );

            // return null if user not found
            if (user == null)
            {
                return Forbid();
            }

            _authService.GenToken(user);

            return user;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, UserEditDTO userInfo)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

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

                if (user == null)
                {
                    return Forbid();
                }

                user.Password = _authService.HashPassword(userInfo.NewPassword);
            }
            else
            {
                user = await _context.User
                    .FirstOrDefaultAsync(u =>
                        u.Id == id
                    );

                if (user == null)
                {
                    return Forbid();
                }
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
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

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