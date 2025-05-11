using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;

namespace EasyBlog.Api.Services
{
    public interface IArticleService
    {

        Task<int?>? CreateArticleAsync(ArticleDto article);
        Task<ArticleDto?> GetArticleAsync(int id);
    }

    public class ArticleService (IArticleRepository articleRepository) : IArticleService
    {
        public async Task<int?>? CreateArticleAsync(ArticleDto article)
        {
            //Check if there's actually no nulls
            if (article == null || article.Title == null || article.Body == null)
            {
                return null;
            }

            var result = await articleRepository.AddArticle(article);

            return result;
        }

        public async Task<ArticleDto?> GetArticleAsync(int id)
        {
            var result = await articleRepository.GetArticle(id);

            return result;
        }
    }

}