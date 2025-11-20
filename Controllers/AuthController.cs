using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UpskillingApi.Data;

namespace UpskillingApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Cadastros.FirstOrDefault(u => u.Email == request.Email && u.Senha == request.Senha);
        if (user == null)
        {
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        var jwtSection = _configuration.GetSection("Jwt");
        var secretKey = jwtSection.GetValue<string>("Key") ?? "SUA_CHAVE_SECRETA_DEV_AQUI_ALTERE_PARA_PROJETO_REAL";
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "upskilling-api";
        var audience = jwtSection.GetValue<string>("Audience") ?? "upskilling-api-clients";
        var expiresMinutes = jwtSection.GetValue<int>("ExpiresInMinutes", 60);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("cpf", user.Cpf),
            new Claim(ClaimTypes.Name, user.Nome)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = expiresMinutes * 60
        });
    }
}