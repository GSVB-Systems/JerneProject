using System.ComponentModel.DataAnnotations.Schema;

namespace dataaccess.Entities;

public class Board
{
    public String BoardID { get; set; }
    public int BoardSize  { get; set; }
    public bool IsActive { get; set; }
    public bool IsRepeating { get; set; }
    public DateTime CreatedAt { get; set; }
    public String UserID { get; set; }
    
    public ICollection<BoardNumber> Numbers { get; set; }
    
    
    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}