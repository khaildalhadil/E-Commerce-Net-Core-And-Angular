using Application.Dtos.account;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AccountController(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    ILogger<AccountController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            logger.LogWarning("Registration rejected: email {Email} already in use", registerDto.Email);
            return BadRequest(new { Message = "Email is already in use" });
        }

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Registration failed for {Email}: {Errors}",
                registerDto.Email,
                string.Join("; ", result.Errors.Select(e => e.Description)));

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        logger.LogInformation("Registered new user {Email}", registerDto.Email);

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = tokenService.CreateToken(user),
        };
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            logger.LogWarning("Failed login attempt for {Email}", loginDto.Email);
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        logger.LogInformation("User {Email} logged in", loginDto.Email);

        return new UserDto
        {
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Token = tokenService.CreateToken(user),
        };
    }
}
