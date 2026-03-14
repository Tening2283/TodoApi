using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // POST: api/auth/register
    // Crée un nouveau compte — body: { "username": "...", "password": "...", "role": "Admin"|"User" }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Vérifier que le username n'existe pas déjà
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existing is not null)
            return Conflict(new { message = "Ce nom d'utilisateur existe déjà." });

        if (request.Role != "Admin" && request.Role != "User")
            return BadRequest(new { message = "Le rôle doit être 'Admin' ou 'User'." });

        var user = new AppUser
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Utilisateur '{user.Username}' créé avec le rôle '{user.Role}'." });
    }

    // POST: api/auth/login
    // Authentifie et retourne un token JWT — body: { "username": "...", "password": "..." }
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });

        var token = GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        });
    }

    private string GenerateJwtToken(AppUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
