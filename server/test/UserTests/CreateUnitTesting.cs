using Contracts.UserDTOs;
using dataaccess;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

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
    public async Task CreateUserInvalid_InvalidEmail()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "larsexample.dk",
            Password = "Lars123!",
            Role = "Bruger"
        };
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_WeakPassword()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "Lars@example.dk",
            Password = "password",
            Role = "Bruger"
        };
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_DuplicateEmail()
    {
        
        ctx.Users.Add(new()
        {
            UserID = Guid.NewGuid().ToString(),
            Email = "lars@example.dk",
            Firstname = "Lars",
            Lastname = "Hansen",
            Role = UserRole.Bruger,
            Hash = "dummyhash",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        var duplicateDto = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "lars@example.dk",
            Password = "Lars123!",
            Role = "Bruger"
        };
        await Assert.ThrowsAsync<DuplicateResourceException>(() => userService.CreateAsync(duplicateDto));
        Assert.Equal(1, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_NullFirstName()
    {
        var user = new RegisterUserDto
        {
            Firstname = null,
            Lastname = "Hansen",
            Email = "Lars@example.dk",
            Password = "Password!123",
            Role = "Bruger"
        };
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_NullLastName()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = null,
            Email = "Lars@example.dk",
            Password = "Password!123",
            Role = "Bruger"
        };
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_NullRole()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "Lars@example.dk",
            Password = "Password!123",
            Role = null
        };
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }

    [Fact]
    public async Task CreateUserInvalid_InvalidRole()
    {
        var user = new RegisterUserDto
        {
            Firstname = "Lars",
            Lastname = "Hansen",
            Email = "Lars@example.dk",
            Password = "Password!123",
            Role = "god"
        };
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.CreateAsync(user));
        Assert.Equal(0, ctx.Users.Count());
    }
}
