using DesyBank.Application.DTOs.Auth;
using DesyBank.Application.DTOs.Auth.Login;
using DesyBank.Application.Interfaces;
using DesyBank.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DesyBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Service
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        // Rotas
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest, CancellationToken ct)
        {
            var result = await _service.Login(loginRequest, ct);

            return result.Match(
                sucess => Ok(sucess),
                error => error.type switch
                {
                    EErrorType.UnauthorizedError => Unauthorized(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest, CancellationToken ct)
        {
            var result = await _service.Register(registerRequest, ct);

            return result.Match(
                sucess => Created("/me", sucess),
                error => error.type switch
                {
                    EErrorType.ValidationError => BadRequest(error),
                    EErrorType.ConflictError => Conflict(error),
                    _ => StatusCode(500, error)
                }
            );
        }
    }
}