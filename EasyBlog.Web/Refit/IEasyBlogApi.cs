using Refit;
using EasyBlog.Shared.Dtos;

namespace EasyBlog.Web.Refit
{
    public interface IEasyBlogApi
    {
        [Get("/api/article/{id}")]
        Task<ArticleDto> GetArticle(int Id);
        
    }
}