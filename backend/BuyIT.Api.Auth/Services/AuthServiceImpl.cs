using BuyIT.Api.Auth;
using BuyIT.Api.Auth.Models;
using BuyIT.Api.Auth.Models.Dto;
using BuyIT.Api.Auth.Services.IServices;
using BuyIT.API.Auth.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class AuthServiceImpl : AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;

    public AuthServiceImpl(IAuthService authService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
    {
        _authService = authService;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public override async Task<RegistrationResponse> Register(RegistrationRequest request, ServerCallContext context)
    {
        var errorMessage = await _authService.Register(new RegistrationRequestDto
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
        });

        return new RegistrationResponse
        {
            Success = string.IsNullOrEmpty(errorMessage),
            Message = errorMessage ?? "User created successfully!"
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var loginResponse = await _authService.Login(new LoginRequestDto
        {
            UserName = request.Email,
            Password = request.Password
        });

        return new LoginResponse
        {
            Success = loginResponse.User != null,
            Token = loginResponse.Token,
            Message = loginResponse.User != null ? "Login successful" : "Username or password is incorrect"
        };
    }

    public override async Task<AssignRoleResponse> AssignRole(AssignRoleRequest request, ServerCallContext context)
    {
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(request.RoleName));
            }
            await _userManager.AddToRoleAsync(user, request.RoleName);
            return new AssignRoleResponse
            {
                Success = true,
                Message = "Role assigned successfully"
            };
        }
        return new AssignRoleResponse
        {
            Success = false,
            Message = "User not found"
        };
    }
}