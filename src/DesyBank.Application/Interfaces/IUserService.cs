using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.User;
using DesyBank.Application.Errors;
using OneOf;

namespace DesyBank.Application.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserResponse, AppError>> GetMyDataAsync(Guid userId, CancellationToken ct);
    }
}