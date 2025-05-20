using System.ComponentModel.DataAnnotations;
using EasyBlog.Shared.Dtos;
using EasyBlog.Shared.Enums;

namespace EasyBlog.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string Nickname { get; set; }
        public required string PasswordHash { get; set; }
        public string? Email { get; set; }
        public required DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }

        public Roles Role { get; set; } = Roles.User;
    }
    
}