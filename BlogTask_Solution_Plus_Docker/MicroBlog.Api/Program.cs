using Hangfire;
using Hangfire.SqlServer;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MicroBlog.Infrastructure.Data;
using MicroBlog.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MicroBlog.Api.Jobs;

namespace MicroBlog.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services...
        builder.Services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
        builder.Services.AddSingleton(x => new BlobServiceClient(
            builder.Configuration["BlobConnectionString"]));
        builder.Services.AddScoped<IStorageService, BlobStorageService>();
        builder.Services.AddHangfire(cfg => cfg.UseSqlServerStorage(
            builder.Configuration.GetConnectionString("Default")));
        builder.Services.AddHangfireServer();

        // Auth
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "MicroBlog",
                    ValidateAudience = true,
                    ValidAudience = "MicroBlogUsers",
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YourSuperSecretKeyHere"))
                };
            });
        builder.Services.AddHttpClient(); // For downloading original image
        builder.Services.AddScoped<ImageProcessingJob>();
        builder.Services.AddHangfireServer(); // Required for background processing

        builder.Services.AddControllers();
        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseHangfireDashboard();
        app.Run();

    }
}
