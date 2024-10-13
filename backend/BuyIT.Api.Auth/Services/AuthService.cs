using BuyIT.Api.Auth.Models;
using BuyIT.Api.Auth.Models.Dto;
using BuyIT.Api.Auth.Services.IServices;
using BuyIT.API.Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuyIT.Api.Auth.Services
{
    public class AuthService : IAuthService
    {

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }


        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()){

                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            if (await _userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserDto userDTO = new()
                {
                    Email = user.Email,
                    ID = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                };
                return new LoginResponseDto
                {
                    User = userDTO,
                    Token = _jwtTokenGenerator.GenerateToken(user, roles)
                };
            }

            return new LoginResponseDto() { User = null, Token = "" };
        }

        //create a method to register a user and use a try catch block to handle exceptions
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = registrationRequestDto.Email,
                    UserName = registrationRequestDto.Email,
                    Name = registrationRequestDto.Name,
                    PhoneNumber = registrationRequestDto.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (result.Succeeded)
                {
                    return "";
                }
                else
                {
                    return "User creation failed!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
