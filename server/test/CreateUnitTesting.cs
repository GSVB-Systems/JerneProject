using Contracts.UserDTOs;
using dataaccess;
using service.Services.Interfaces;

namespace test;

public class CreateUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task CreateUserValid()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "lars@example.dk",
            Password = "JegKanHuskeDenneKode123!",
            Role = "Bruger"
        };

        await userService.CreateAsync(user);

        Assert.Equal(1, ctx.Users.Count());
    }
}
