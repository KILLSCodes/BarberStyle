using ApiBabterStyle.Data;
using ApiBabterStyle.DTOs;
using ApiBabterStyle.Model;
using ApiBabterStyle.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiBabterStyle.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(BarberShopDbContext db, JwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("cadastro")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Nome, email e senha sao obrigatorios." });
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(new { message = "A senha precisa ter pelo menos 6 caracteres." });
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await db.Users.AnyAsync(user => user.Email == email, cancellationToken);
        if (exists)
        {
            return Conflict(new { message = "Ja existe um usuario cadastrado com este email." });
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.CreateToken(user);
        return CreatedAtAction(nameof(Register), new AuthResponse(user.Id, user.Name, user.Email, user.Role, token.Token, token.ExpiresAt));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email ou senha invalidos." });
        }

        var token = jwtTokenService.CreateToken(user);
        return Ok(new AuthResponse(user.Id, user.Name, user.Email, user.Role, token.Token, token.ExpiresAt));
    }
}
