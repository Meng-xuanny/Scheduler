using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(120);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null
            );
        })
);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls(
    "http://0.0.0.0:" + (Environment.GetEnvironmentVariable("PORT") ?? "10000")
);

var app = builder.Build();

// Enable Swagger in development and production
app.UseSwagger(); // 🟢 Generates Swagger JSON
app.UseSwaggerUI(c => // 🟢 Adds interactive UI
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Roots API v1");
});

// app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // for local frontend

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();

// Serve static files
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles(); // Enables serving static assets

app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
