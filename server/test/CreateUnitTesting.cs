using Contracts.UserDTOs;
using dataaccess;
using service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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
    
    [Fact]
    public async Task CreateUserUnvalid_UnvalidEmail()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "lars@example.dk",
            Password = "test",
            Role = "Bruger"
        };
        

        await Assert.ThrowsAsync<ValidationException>(() => userService.CreateAsync(user));
    }
}
