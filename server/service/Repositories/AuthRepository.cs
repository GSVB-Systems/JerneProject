using System.Security.Claims;
using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;

namespace service.Repositories;

public class AuthRepository : Repository<User>, IAuthRepository
{
    public AuthRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User> getUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<User> updateUserPasswordAsync(string userId, string newHashedPassword)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
        if (user != null)
        {
            user.Hash = newHashedPassword;
            _context.SaveChanges();
        }
        return Task.FromResult(user);
        
    }
}