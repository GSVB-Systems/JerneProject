
using Contracts.WinningNumberDTOs;
using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Rules;
using service.Services;
using test;

namespace service.Tests
{
    public class WinningNumberServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _ctx;
        private readonly IWinningNumberRepository _repo;
        private readonly ISieveProcessor _sieveProcessor = new TestSieveProcessor();
        private readonly WinningNumberRules _rules = new();
        private readonly WinningNumberService _service;

        public WinningNumberServiceIntegrationTests()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _ctx = new AppDbContext(opts);

            _repo = new WinningNumberRepository(_ctx);

            
            _service = new WinningNumberService(_repo, _sieveProcessor, _rules);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSeededWinningNumbers()
        {
            
            var boardId = await TestDataFactory.CreateWinningBoardAsync(_ctx);
            await TestDataFactory.CreateWinningNumberAsync(_ctx, boardId, 1);
            await TestDataFactory.CreateWinningNumberAsync(_ctx, boardId, 2);

           
            var result = await _service.GetAllAsync(new SieveModel { Page = 1, PageSize = 10 });

           
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
            Assert.Contains(result.Items, i => i.Number == 1);
            Assert.Contains(result.Items, i => i.Number == 2);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectItem()
        {
           
            var boardId = await TestDataFactory.CreateWinningBoardAsync(_ctx);
            var wnId = await TestDataFactory.CreateWinningNumberAsync(_ctx, boardId, 5);

           
            var dto = await _service.GetByIdAsync(wnId);

            
            Assert.NotNull(dto);
            Assert.Equal<string>(wnId, dto!.WinningNumberID);
            Assert.Equal(5, dto.Number);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesNumber()
        {
           
            var boardId = await TestDataFactory.CreateWinningBoardAsync(_ctx);
            var wnId = await TestDataFactory.CreateWinningNumberAsync(_ctx, boardId, 7);

            var update = new UpdateWinningNumberDto { Number = 9 };

            
            var updated = await _service.UpdateAsync(wnId, update);

            
            Assert.NotNull(updated);
            Assert.Equal(9, updated!.Number);

           
            var fromDb = await _ctx.Set<WinningNumber>().FindAsync(wnId);
            Assert.NotNull(fromDb);
            Assert.Equal(9, fromDb!.Number);
        }

        [Fact]
        public async Task DeleteAsync_RemovesItem()
        {
           
            var boardId = await TestDataFactory.CreateWinningBoardAsync(_ctx);
            var wnId = await TestDataFactory.CreateWinningNumberAsync(_ctx, boardId, 3);

            
            var deleted = await _service.DeleteAsync(wnId);

            
            Assert.True(deleted);
            var fromDb = await _ctx.Set<WinningNumber>().FindAsync(wnId);
            Assert.Null(fromDb);
        }

        public void Dispose()
        {
            _ctx?.Dispose();
        }
    }
}