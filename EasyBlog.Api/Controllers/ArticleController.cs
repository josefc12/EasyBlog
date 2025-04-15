using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController(EasyBlogDbContext _context) : ControllerBase 
    {
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            //    return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }
    }

}
