using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DatabaseContext : DbContext {
    private readonly ILoggerFactory loggerFactory;

    public DatabaseContext(ILoggerFactory loggerFactory) {
        this.loggerFactory = loggerFactory;
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var connectionString = "Data Source=.;Initial Catalog=Blog;Integrated Security=True;TrustServerCertificate=true;";
        optionsBuilder.UseSqlServer(connectionString, sqlOptions => {
            // instruct ef to use multiple queries instead of large joined queries
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
        optionsBuilder.UseLoggerFactory(loggerFactory);


        // NOT safe for production
        optionsBuilder.EnableSensitiveDataLogging(true);
    }
}
