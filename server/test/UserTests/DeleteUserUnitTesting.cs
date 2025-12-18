using dataaccess;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

public class DeleteUserUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task DeleteUserValid()
    {
      
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "todelete@example.dk",
            Firstname = "To",
            Lastname = "Delete",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        
        await userService.DeleteAsync(id);

        
        var u = ctx.Users.SingleOrDefault(u => u.UserID == id);
        Assert.Null(u);
    }

    [Fact]
    public async Task DeleteUserInvalid_NullOrEmptyId()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.DeleteAsync(null));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.DeleteAsync(string.Empty));
    }

    [Fact]
    public async Task DeleteUserInvalid_NonExistingUser()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.DeleteAsync(randomId));
    }
}