using System.Security.Claims;
using DesyBank.Application.Interfaces;
using DesyBank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesyBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Service
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        // Rotas
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var result = await _service.GetMyDataAsync(Guid.Parse(userId), ct);

            return result.Match(
                sucess => Ok(sucess),
                error => error.type switch
                {
                    EErrorType.NotFoundError => NotFound(error),
                    _ => StatusCode(500, error)
                }
            );
        }
    }
}