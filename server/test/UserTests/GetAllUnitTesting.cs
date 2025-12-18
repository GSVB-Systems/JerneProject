using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;
using Sieve.Models;

namespace xunittests.UserTests;

public class GetAllUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task GetAll_NoParams_ReturnsAll()
    {
        for (var i = 0; i < 3; i++)
        {
            ctx.Users.Add(new User
            {
                UserID = Guid.NewGuid().ToString(),
                Email = $"user{i}@example.dk",
                Firstname = "User",
                Lastname = i.ToString(),
                Role = UserRole.Bruger,
                Hash = "h",
                IsActive = true
            });
        }
        await ctx.SaveChangesAsync();

        var result = await userService.GetAllAsync(null);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task GetAll_WithPaging_ReturnsPage()
    {
        for (var i = 0; i < 3; i++)
        {
            ctx.Users.Add(new User
            {
                UserID = Guid.NewGuid().ToString(),
                Email = $"pageuser{i}@example.dk",
                Firstname = "Page",
                Lastname = i.ToString(),
                Role = UserRole.Bruger,
                Hash = "h",
                IsActive = true
            });
        }
        await ctx.SaveChangesAsync();

        var model = new SieveModel { Page = 1, PageSize = 2 };
        var result = await userService.GetAllAsync(model);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAll_PageBeyondResults_ReturnsEmptyItems()
    {
        for (var i = 0; i < 3; i++)
        {
            ctx.Users.Add(new User
            {
                UserID = Guid.NewGuid().ToString(),
                Email = $"edge{i}@example.dk",
                Firstname = "Edge",
                Lastname = i.ToString(),
                Role = UserRole.Bruger,
                Hash = "h",
                IsActive = true
            });
        }
        await ctx.SaveChangesAsync();

        var model = new SieveModel { Page = 10, PageSize = 2 };
        var result = await userService.GetAllAsync(model);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Empty(result.Items);
        Assert.Equal(10, result.Page);
    }

    [Fact]
    public async Task GetAll_InvalidPage_ThrowsRangeValidationException()
    {
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.GetAllAsync(new SieveModel { Page = 0 }));
    }

    [Fact]
    public async Task GetAll_InvalidPageSize_ThrowsRangeValidationException()
    {
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.GetAllAsync(new SieveModel { PageSize = 0 }));
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.GetAllAsync(new SieveModel { PageSize = 1001 }));
    }
}