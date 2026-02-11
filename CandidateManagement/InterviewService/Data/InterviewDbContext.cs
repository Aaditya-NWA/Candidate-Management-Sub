using InterviewService.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Data;

[ExcludeFromCodeCoverage]
public class InterviewDbContext : DbContext
{
   
    public InterviewDbContext(DbContextOptions<InterviewDbContext> options)
        : base(options) { }

    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Interview)
            .WithMany()
            .HasForeignKey(f => f.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
