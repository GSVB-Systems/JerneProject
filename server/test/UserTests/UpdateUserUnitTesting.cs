using Contracts.UserDTOs;
using dataaccess;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

public class UpdateUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task UpdateUserValid()
    {
        // arrange
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "old@example.dk",
            Firstname = "Old",
            Lastname = "Name",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        var dto = new UpdateUserDto
        {
            Firstname = "New",
            Lastname = "Name",
            Email = "new@example.dk"
            
        };

        // act
        await userService.UpdateAsync(id, dto);

        // assert
        var u = ctx.Users.Single(u => u.UserID == id);
        Assert.Equal("New", u.Firstname);
        Assert.Equal("new@example.dk", u.Email);
    }

    [Fact]
    public async Task UpdateUserInvalid_NullOrEmptyId()
    {
        var dto = new UpdateUserDto { Email = "a@b.com" };
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.UpdateAsync(null, dto));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.UpdateAsync(string.Empty, dto));
    }

    [Fact]
    public async Task UpdateUserInvalid_NonExistingUser()
    {
        var dto = new UpdateUserDto { Email = "a@b.com" };
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.UpdateAsync(randomId, dto));
    }

    [Fact]
    public async Task UpdateUserInvalid_InvalidEmail()
    {
        // arrange
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "keep@example.dk",
            Firstname = "Lars",
            Lastname = "Hansen",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        var dto = new UpdateUserDto { Email = "invalid-email" };

        // act / assert
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.UpdateAsync(id, dto));
    }

    [Fact]
    public async Task UpdateUserInvalid_DuplicateEmail()
    {
        // arrange: existing user with the target email
        ctx.Users.Add(new()
        {
            UserID = Guid.NewGuid().ToString(),
            Email = "existing@example.dk",
            Firstname = "Ex",
            Lastname = "Ist",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });

        var targetId = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = targetId,
            Email = "original@example.dk",
            Firstname = "Target",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });

        await ctx.SaveChangesAsync();

        var dto = new UpdateUserDto { Email = "existing@example.dk" };

        // act / assert
        await Assert.ThrowsAsync<DuplicateResourceException>(() => userService.UpdateAsync(targetId, dto));
    }

    [Fact]
    public async Task UpdateUserInvalid_NegativeBalance()
    {
        // arrange
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "bal@test.dk",
            Firstname = "Bal",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            Balance = 10m
        });
        await ctx.SaveChangesAsync();

        var dto = new UpdateUserDto { Balance = -5m };

        // act / assert
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.UpdateAsync(id, dto));
    }

    [Fact]
    public async Task UpdateUserInvalid_InvalidRole()
    {
        // arrange
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "role@test.dk",
            Firstname = "Role",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        // assign an undefined enum value to trigger validation
        var dto = new UpdateUserDto { Role = (UserRole)999 };

        // act / assert
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.UpdateAsync(id, dto));
    }

    [Fact]
    public async Task UpdateUser_NoOp_WhenDtoNull()
    {
        // arrange
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new()
        {
            UserID = id,
            Email = "noop@example.dk",
            Firstname = "No",
            Lastname = "Op",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        // act - should not throw and should leave entity unchanged
        await userService.UpdateAsync(id, null);

        var u = ctx.Users.Single(u => u.UserID == id);
        Assert.Equal("No", u.Firstname);
    }
}