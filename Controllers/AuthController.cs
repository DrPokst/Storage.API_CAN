using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Storage.API_CAN.DTOs;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInmanager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInmanager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInmanager = signInmanager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDTO);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDTO.Password);
            
            if (result.Succeeded)
            {
                return StatusCode(201);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {


            var user = await _userManager.FindByNameAsync(userForLoginDTO.Username);
            var result = await _signInmanager.CheckPasswordSignInAsync(user, userForLoginDTO.Password, false);

            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);
                return Ok(new
                {
                    token = GenerateJwtToken(user),
                    user = appUser
                });
            }

            return Unauthorized();

        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

}
