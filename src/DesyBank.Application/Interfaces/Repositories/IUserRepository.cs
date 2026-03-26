using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.User;
using DesyBank.Domain.Models;

namespace DesyBank.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken ct);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);

        // Validation Methods
        Task<bool> EmailAlreadyExistsAsync(string email, CancellationToken ct);
        Task<bool> UserExistsByIdAsync(Guid userId, CancellationToken ct);
    }
}