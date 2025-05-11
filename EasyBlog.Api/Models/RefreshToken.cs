using System.ComponentModel.DataAnnotations;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; } 
    public string UserId { get; set; } 
    public DateTime DateCreated { get; set; }
    public DateTime DateExpires { get; set; }
    public bool IsRevoked { get; set; }
}