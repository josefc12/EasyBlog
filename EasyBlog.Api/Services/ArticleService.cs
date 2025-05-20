using EasyBlog.Api.Models;
using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Services
{
    public interface IArticleService
    {

        Task<int?>? CreateArticleAsync(ArticleDto article, int userId);
        Task<int?>? EditArticleAsync(ArticleDto editedArticle);
        Task<ArticleDto?> GetArticleAsync(int id);
        Task<List<string>>? GetArticleYears();
        Task<List<string>>? GetArticleMonths(int year);
        Task<List<ArticleDto>?> GetArticlesList(int year, int month);
    }

    public class ArticleService(IArticleRepository articleRepository, IUserService userService) : IArticleService
    {
        public async Task<int?>? CreateArticleAsync(ArticleDto article, int userId)
        {
            //Check if there's actually no nulls
            if (article == null || article.Title == null || article.Body == null)
            {
                return null;
            }

            var user = await userService.GetUserById(userId);
            
            if (user is OkObjectResult userResult)
            {
                var userRecord = userResult.Value as User;

                if (userRecord != null)
                {
                    var result = await articleRepository.AddArticle(article, userRecord);
                    return result;
                }
            }
            return null;            
        }

        public async Task<int?>? EditArticleAsync(ArticleDto editedArticle)
        {
            //Check if there's actually no nulls
            if (editedArticle == null || editedArticle.Title == null || editedArticle.Body == null)
            {
                return null;
            }
            
            var result = await articleRepository.EditArticle(editedArticle);
            return result;            
        }

        public async Task<ArticleDto?> GetArticleAsync(int id)
        {
            var result = await articleRepository.GetArticle(id);

            return result;
        }

        public async Task<List<string>?> GetArticleYears()
        {
            var result = await articleRepository.GetArticleYears();

            return result;
        }

        public async Task<List<string>?> GetArticleMonths(int year)
        {
            var result = await articleRepository.GetArticleMonths(year);

            return result;
        }
        
        public async Task<List<ArticleDto>?> GetArticlesList(int year, int month)
        {
            var result = await articleRepository.GetArticlesList(year, month);

            return result;
        }
    }
}