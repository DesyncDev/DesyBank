using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.User;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using DesyBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DesyBank.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        // DataBase
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methods
        public async Task AddUserAsync(User user, CancellationToken ct)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct)
        => await _context.Users
        .AsNoTracking()
        .Where(e => e.Email == email)
        .FirstOrDefaultAsync(ct);

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        => await _context.Users
        .AsNoTracking()
        .Where(pk => pk.Id == userId)
        .Select(x => new UserResponse(
            x.Id,
            x.FullName,
            x.JoinedAt
        ))
        .FirstOrDefaultAsync(ct);

        // Verification Methods
        public async Task<bool> EmailAlreadyExistsAsync(string email, CancellationToken ct)
        => await _context.Users
        .AsNoTracking()
        .AnyAsync(e => e.Email == email, ct);

        public async Task<bool> UserExistsByIdAsync(Guid userId, CancellationToken ct)
        => await _context.Users
        .AsNoTracking()
        .AnyAsync(u => u.Id == userId, ct);
    }
}