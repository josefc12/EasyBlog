namespace EasyBlog.Shared.Dtos;

public class ArticleDto
{
    public int Id {get; set;}
    public required string Title {get; set;}
    public required string Body {get; set;}
}
