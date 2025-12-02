using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;

namespace Service.Repositories;

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
}