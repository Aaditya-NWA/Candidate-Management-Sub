using InterviewService.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Tests.Helpers;

[ExcludeFromCodeCoverage]
public static class TestDbContextFactory
{
    public static InterviewDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new InterviewDbContext(options);
    }
}
