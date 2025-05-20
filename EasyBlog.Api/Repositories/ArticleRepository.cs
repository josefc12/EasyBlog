using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Repositories
{
    public interface IArticleRepository
    {
        Task<int?>? AddArticle(ArticleDto article, User user);
        Task<int?>? EditArticle(ArticleDto editedArticle);
        Task<ArticleDto?> GetArticle(int id);
        Task<List<string>>? GetArticleYears();
        Task<List<string>>? GetArticleMonths(int year);
        Task<List<ArticleDto>?> GetArticlesList(int year, int month);
    }

    public class ArticleRepository(EasyBlogDbContext _context) : IArticleRepository
    {
        public async Task<int?>? AddArticle(ArticleDto article, User user)
        {
            Article newArticle = new Article()
            {
                Title = article.Title,
                Body = article.Body,
                Author = user,
                AuthorId = user.Id
            };
            var result = await _context.Articles.AddAsync(newArticle);
            await _context.SaveChangesAsync();

            return newArticle.Id;

        }

        public async Task<int?>? EditArticle(ArticleDto editedArticle)
        {
            var originalArticle = await _context.Articles.Include(a => a.Author).Where(a => a.Id == editedArticle.Id).FirstOrDefaultAsync();

            if (originalArticle != null)
            {
                originalArticle.Title = editedArticle.Title;
                originalArticle.Body = editedArticle.Body;

                await _context.SaveChangesAsync();
                return originalArticle.Id;
            }

            return null;
        }

        public async Task<ArticleDto?> GetArticle(int id)
        {
            var result = await _context.Articles.Include(a => a.Author).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (result != null)
            {
                ArticleDto retreivedArticle = new ArticleDto()
                {
                    Id = result.Id,
                    Title = result.Title,
                    Body = result.Body,
                    DateCreated = result.DateCreated,
                    Author = new UserDto()
                    {
                        Nickname = result.Author.Nickname
                    }
                };
                return retreivedArticle;
            }
            return null;
        }

        public async Task<List<string>?> GetArticleYears()
        {
            return await _context.Articles
                .Select(a => a.DateCreated.Year.ToString())
                .Distinct()
                .OrderBy(year => year)
                .ToListAsync();
        }
        public async Task<List<string>?> GetArticleMonths(int year)
        {
            return await _context.Articles
                .Where(a => a.DateCreated.Year == year)
                .Select(a => a.DateCreated.Month.ToString())
                .Distinct()
                .OrderBy(year => year)
                .ToListAsync();
        }
        public async Task<List<ArticleDto>?> GetArticlesList(int year, int month)
        {
            return await _context.Articles
                .Where(a => a.DateCreated.Year == year && a.DateCreated.Month == month)
                .Select(a => new ArticleDto(){ Id = a.Id, Title = a.Title})
                .ToListAsync();
        }
    }
}