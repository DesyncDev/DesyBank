using System.Security.Claims;
using DesyBank.Application.DTOs.Account;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.DTOs.Transfer;
using DesyBank.Application.Interfaces;
using DesyBank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesyBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        // Services
        private readonly IAccountService _service;
        private readonly ITransactionService _transactionService;
        private readonly ITransferService _transferService;

        public AccountsController(IAccountService service, ITransactionService transactionService,
            ITransferService transferService
        )
        {
            _service = service;
            _transactionService = transactionService;
            _transferService = transferService;
        }

        // Rotas
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateAccountAsync(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            AccountRequest request = new AccountRequest(Guid.Parse(userId));
            var result = await _service.CreateAccountAsync(request, ct);

            return result.Match(
                sucess => Created("", sucess),
                error => error.type switch
                {
                    EErrorType.UnauthorizedError => Unauthorized(error),
                    EErrorType.BusinessRuleError => BadRequest(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("withdraw")]
        [Authorize]
        public async Task<IActionResult> WithDraw(OperationRequest request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            // Cria o transaction Request
            var transaction = new TransactionRequest(
                Guid.Parse(userId),
                request.Amount,
                ETransactionType.WithDraw
            );

            var result = await _transactionService.CreateTransactionAsync(transaction, ct);

            return result.Match(
                sucess => Ok(sucess),
                error => error.type switch
                {
                    EErrorType.NotFoundError => NotFound(error),
                    EErrorType.BusinessRuleError => BadRequest(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("deposit")]
        [Authorize]
        public async Task<IActionResult> Deposit(OperationRequest request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            // Cria o transaction Request
            var transaction = new TransactionRequest(
                Guid.Parse(userId),
                request.Amount,
                ETransactionType.Deposit
            );

            var result = await _transactionService.CreateTransactionAsync(transaction, ct);

            return result.Match(
                sucess => Ok(sucess),
                error => error.type switch
                {
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("transfer")]
        [Authorize]
        public async Task<IActionResult> Transfer(TransferOperationRequest request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var transferRequest = new TransferRequest(
                Guid.Parse(userId),
                request.ToAccountNumber,
                request.Amount
            );

            var result = await _transferService.CreateTransferAsync(transferRequest, ct);

            return result.Match(
                sucess => Created("", sucess),
                error => error.type switch
                {
                    EErrorType.NotFoundError => NotFound(error),
                    EErrorType.BusinessRuleError => BadRequest(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> Balance(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var result = await _service.GetBalance(Guid.Parse(userId), ct);

            return result.Match(
                sucess => Ok(sucess),
                error => error.type switch
                {
                    EErrorType.NotFoundError => NotFound(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpGet("transactions")]
        [Authorize]
        public async Task<IActionResult> Transactions(CancellationToken ct, [FromQuery] int p = 1, [FromQuery] int t = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _transactionService.GetPagedTransactionsAsync(Guid.Parse(userId), p, t, ct);

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