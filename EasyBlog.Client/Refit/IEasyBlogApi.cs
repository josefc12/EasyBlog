using Refit;
using EasyBlog.Shared.Dtos;

namespace EasyBlog.Client.Refit
{
    public interface IEasyBlogApi
    {

        [Post("/api/article")]
        Task<int?>? PostArticle(ArticleDto article);

        [Get("/api/article/{id}")]
        Task<ArticleDto> GetArticle(int Id);
        
    }
}