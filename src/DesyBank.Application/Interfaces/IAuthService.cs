using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Auth;
using DesyBank.Application.DTOs.Auth.Login;
using DesyBank.Application.Errors;
using OneOf;

namespace DesyBank.Application.Interfaces
{
    public interface IAuthService
    {
        Task<OneOf<LoginResponse, AppError>> Login(LoginRequest loginRequest, CancellationToken ct);
        Task<OneOf<RegisterResponse, AppError>> Register(RegisterRequest registerRequest, CancellationToken ct);
    }
}