using InterviewService.Data;
using InterviewService.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;


var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<InterviewService.Services.InterviewService>();
builder.Services.AddScoped<InterviewValidationService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddDbContext<InterviewDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapDefaultEndpoints();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();
[ExcludeFromCodeCoverage]
public partial class Program { }