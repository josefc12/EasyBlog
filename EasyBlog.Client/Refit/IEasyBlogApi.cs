using Refit;
using EasyBlog.Shared.Dtos;
using EasyBlog.Shared.Enums;

namespace EasyBlog.Client.Refit
{
    public interface IEasyBlogApi
    {

        [Post("/api/article")]
        Task<int?>? PostArticle(ArticleDto article);

        [Post("/api/article/edit")]
        Task<int?>? EditArticle(ArticleDto editedArticle);

        [Get("/api/article/{id}")]
        Task<ArticleDto> GetArticle(int Id);

        [Get("/api/home")]
        Task<string> GetHomePageContent();

        [Post("/api/home")]
        Task<IApiResponse> UpdateHomePageContent(string htmlValue);

        [Post("/api/User/register")]
        Task<IApiResponse> UserRegister(UserDto formModel);

        [Post("/api/User/login")]
        Task<string> UserLogin(UserDto formModel);

        [Post("/api/Auth/refresh")]
        Task<string> GetRefreshToken();

        [Get("/api/article/data/{type}")]
        Task<List<string>> GetArticleData(ArticleDataTypes type, int? year = null);

        [Get("/api/article/data/articles")]
        Task<List<ArticleDto>> GetArticlesList(int year, int month);
        
        [Post("/api/User/set-admin-role/{nickname}")]
        Task<IApiResponse> SetAdminRole(string nickname);

    }
}