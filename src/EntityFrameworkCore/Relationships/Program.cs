using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Program
    /// </summary>
    public class Program {
        public static Task<int> Main(string[] args) {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });
            var logger = loggerFactory.CreateLogger<Program>();

            using (var db = new DatabaseContext(loggerFactory)) {
                // Create
                Console.WriteLine("Inserting a new blog");
                db.Add(new Blog { Url = "https://cortside.com" });
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for a blog");
                var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();

                // Update and add a post
                Console.WriteLine("Updating the blog and adding a post");
                blog.Url = "https://cortside.com/tags/EF-Core";
                blog.Posts.Add(new Post { Title = "EFCore Relationships", Content = "Examples of EFCore relationships" });
                db.SaveChanges();

                // Add another post
                Console.WriteLine("Updating the blog and adding a post");
                blog.Posts.Add(new Post { Title = "Database operation expected to affect", Content = "What Troy said....." });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete the blog");
                db.Remove(blog);
                db.SaveChanges();
            }

            return Task.FromResult<int>(0);
        }
    }
}
