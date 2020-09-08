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
using System.Text;
using System.Threading.Tasks;

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
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [AllowAnonymous]
        [HttpGet("check-exist")]
        public async Task<ActionResult<UserLoginDTO>> CheckUserExist()
        {
            if (await UserExist(Request.Query["username"]))
            {
                return Ok(new { result = true });
            }

            return Ok(new { result = false });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserLoginDTO>> Register(User user)
        {
            if (user.Username.Length < 6 || user.Password.Length < 6)
            {
                return Forbid();
            }

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
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            _authService.GenToken(user);

            return Ok(user);
        }

        private Task<bool> UserExist(string username)
        {
            return _context.User.AnyAsync(u => u.Username == username);
        }
    }
}
