using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Repositories
{
    public interface IArticleRepository
    {
        Task<int?>? AddArticle(ArticleDto article);
        Task<ArticleDto?> GetArticle(int id);
    }

    public class ArticleRepository(EasyBlogDbContext _context) : IArticleRepository
    {
        public async Task<int?>? AddArticle(ArticleDto article)
        {
            Article newArticle = new Article() {
                Title = article.Title,
                Body = article.Body,
            };
            var result = await _context.Articles.AddAsync(newArticle);
            await _context.SaveChangesAsync();

            return newArticle.Id;

        }

        public async Task<ArticleDto?> GetArticle(int id)
        {
            var result = await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (result != null)
            {
                ArticleDto retreivedArticle = new ArticleDto() {
                    Id = result.Id,
                    Title = result.Title,
                    Body = result.Body
                };
                return retreivedArticle;
            }
            return null;
        }
    }
}