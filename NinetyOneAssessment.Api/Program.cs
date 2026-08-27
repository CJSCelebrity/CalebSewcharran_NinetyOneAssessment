using Microsoft.EntityFrameworkCore;
using NinetyOneAssessment.Infrastructure;
using NinetyOneAssessment.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScoresDbContext>();
    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();