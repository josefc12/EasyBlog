using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Api.Services;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyBlog.Shared.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EasyBlog.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController(IArticleService articleService) : ControllerBase
    {

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<int?>> PostArticle(ArticleDto article)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Call the service that's gonna call the repository.
            int? result = await articleService.CreateArticleAsync(article, int.Parse(userIdClaim));

            return result;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("edit")]
        public async Task<ActionResult<int?>> EditArticle(ArticleDto editedArticle)
        {

            var userNicknameClaim = User.FindFirst(JwtRegisteredClaimNames.Nickname)?.Value;
            if (userNicknameClaim == null || userNicknameClaim == "" || userNicknameClaim != editedArticle.Author.Nickname)
            {
                return null;
            }
            //Call the service that's gonna call the repository.
            int? result = await articleService.EditArticleAsync(editedArticle);

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto?>> GetArticle(int id)
        {
            ArticleDto? result = await articleService.GetArticleAsync(id);

            return result;
        }

        [HttpGet("data/{type}")]
        public async Task<ActionResult<List<string>>> GetArticleData(ArticleDataTypes type, int? year = null)
        {
            switch (type)
            {
                case ArticleDataTypes.Years:
                    return await articleService.GetArticleYears();
                case ArticleDataTypes.Months:
                    return await articleService.GetArticleMonths(year ?? 0);
                default:
                    return BadRequest("Invalid type");
            }
        }
        
        [HttpGet("data/articles")]
        public async Task<ActionResult<List<ArticleDto>>> GetArticlesList(int year, int month)
        { 
            return await articleService.GetArticlesList(year, month);
        }
    }
}
