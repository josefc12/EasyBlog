using System.ComponentModel.DataAnnotations;

namespace EasyBlog.Api.Models
{
    public class User 
    {
        [Key]
        public int Id {get;set;}
        public required string Nickname {get;set;}
        public required string PasswordHash {get;set;}
        public string? Email {get;set;}
        public required DateTime DateCreated {get;set;}
        public DateTime? DateDeleted {get;set;}

    }
}