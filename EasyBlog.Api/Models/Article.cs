using System.ComponentModel.DataAnnotations;

namespace EasyBlog.Api.Models
{
    public class Article
    {
        [Key]
        public int Id {get; set;}
        [MaxLength(200)]
        public required string Title {get; set;}
        public required string Body {get; set;}
        public DateTime DateCreated {get; set;}
        public User Author { get; set; }
        public int AuthorId { get; set; }

        public Article()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}