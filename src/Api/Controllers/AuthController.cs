using Microsoft.AspNetCore.Mvc;
using ProductApi.Security;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwt;

    public AuthController(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        // Credenciales simuladas
        const string demoUser = "admin";
        const string demoPass = "Password123!";

        if (req is null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return Unauthorized();

        if (req.Username == demoUser && req.Password == demoPass)
        {
            var token = _jwt.GenerateToken(req.Username);
            return Ok(new { token });
        }
        return Unauthorized();
    }
}
