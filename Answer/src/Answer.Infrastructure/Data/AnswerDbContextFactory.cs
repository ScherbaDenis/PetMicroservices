using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Answer.Infrastructure.Data;

public class AnswerDbContextFactory : IDesignTimeDbContextFactory<AnswerDbContext>
{
    public AnswerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AnswerDbContext>();
        
        // Use a default connection string for migrations
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AnswerDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new AnswerDbContext(optionsBuilder.Options);
    }
}
