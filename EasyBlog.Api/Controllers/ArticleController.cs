using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Api.Services;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController(IArticleService articleService) : ControllerBase 
    {

        [HttpPost]
        public async Task<ActionResult<int?>> PostArticle(ArticleDto article)
        {
            
            //Call the service that's gonna call the repository.
            int? result = await articleService.CreateArticleAsync(article);

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto?>> GetArticle(int id)
        {
            ArticleDto? result = await articleService.GetArticleAsync(id);

            return result;
        }
    }

}
