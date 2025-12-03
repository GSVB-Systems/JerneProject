using dataaccess.Entities.Enums;

namespace dataaccess.Entities;

public class User
{
    public String UserID { get; set; }
    public String Firstname { get; set; }
    public String Lastname { get; set; }
    public String Email { get; set; }
    public String Hash { get; set; }
    public UserRole Role { get; set; }
    public bool Firstlogin { get; set; }
    public bool IsActive { get; set; }
    public decimal Balance { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<Board> Boards { get; set; }
}