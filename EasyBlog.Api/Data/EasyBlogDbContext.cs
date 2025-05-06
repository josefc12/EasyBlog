using EasyBlog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Data 
{
    public class EasyBlogDbContext(DbContextOptions<EasyBlogDbContext> options) : DbContext(options)
    {
        public required DbSet<Article> Articles { get; set; }
        public required DbSet<User> Users { get; set; }
    }
}
