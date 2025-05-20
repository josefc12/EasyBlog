namespace EasyBlog.Shared.Dtos;

public class ArticleDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Body { get; set; }
    public DateTime? DateCreated { get; set; }
    public UserDto? Author { get; set; }
}
