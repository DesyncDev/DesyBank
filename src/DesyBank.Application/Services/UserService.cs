using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.User;
using DesyBank.Application.Errors;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Repositories;
using OneOf;

namespace DesyBank.Application.Services
{
    public class UserService : IUserService
    {
        // Repository
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        // Methods
        public async Task<OneOf<UserResponse, AppError>> GetMyDataAsync(Guid userId, CancellationToken ct)
        {
            // Busca usuario
            var user = await _repository.GetUserByIdAsync(userId, ct);

            if (user == null)
                return new UserNotFoundError();
            
            return user;
        }
    }
}