using DesyBank.Application.DTOs.User;
using DesyBank.Application.Errors;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using OneOf;

namespace DesyBank.Application.Services
{
    public class UserService : IUserService
    {
        // Repository
        private readonly IUserRepository _repository;
        private readonly IAccountRepository _accountRepository;

        public UserService(IUserRepository repository, IAccountRepository accountRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
        }

        // Methods
        public async Task<OneOf<UserResponse, AppError>> GetMyDataAsync(Guid userId, CancellationToken ct)
        {
            // Busca usuario
            var user = await _repository.GetUserByIdAsync(userId, ct);
            var account = await _accountRepository.GetAccountByUserReadAsync(userId, ct);

            if (user == null)
                return new UserNotFoundError();

            if (account == null)
                return new AccountNotFoundError();
            
            return new UserResponse(
                user.Id,
                user.FullName,
                account.AccountNumber,
                user.JoinedAt
            );
        }
    }
}