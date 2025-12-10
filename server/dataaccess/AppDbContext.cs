using dataaccess.Entities;
 using Microsoft.EntityFrameworkCore;
namespace dataaccess;

public class AppDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WinningBoard> WinningBoards { get; set; }
    public DbSet<BoardNumber> Numbers { get; set; }
    public DbSet<WinningNumber> WinningNumbers { get; set; }
    public DbSet<BoardNumber> BoardNumbers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}