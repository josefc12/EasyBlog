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
        
        [Post("/api/User/register")]
        Task<IApiResponse> UserRegister(UserDto formModel);
    
        [Post("/api/User/login")]
        Task<string> UserLogin(UserDto formModel);

        [Post("/api/Auth/refresh")]
        Task<string> GetRefreshToken();

    }
}