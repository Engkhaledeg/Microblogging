using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using MicroBlog.Application.Interfaces;
//using MicroBlog.Application.Services;
//using MicroBlog.Worker.Jobs;
using Hangfire;
using Microsoft.Extensions.Configuration;
using System;
using MicroBlog.Api.Jobs;
using System.Threading.Tasks;

namespace MicroBlog.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Create and configure the host for the worker service
            var host = CreateHostBuilder(args).Build();

            // Schedule recurring jobs here
            ScheduleJobs(host.Services);

            // Run the host, which will start processing jobs
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Optionally, add any custom configuration files like appsettings.json
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Add Hangfire with SQL Server storage for background job processing
                    services.AddHangfire(x => x.UseSqlServerStorage(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                    services.AddHangfireServer();

                    // Register your services for the worker
                    //services.AddScoped<IPostService, PostService>();

                    // Configure logging
                    services.AddLogging();

                    // Register the background job processing service
                    services.AddSingleton<IImageProcessingJob, ImageProcessingJob>();
                });

        // Method to schedule jobs
        private static void ScheduleJobs(IServiceProvider services)
        {
            // Create a scope to resolve services required by the job
            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // Create a job scheduler instance
            var recurringJobs = serviceProvider.GetRequiredService<IRecurringJobManager>();

            // Example: Scheduling an image processing job to run every 10 minutes
            recurringJobs.AddOrUpdate(
                "ImageProcessingJob",
                () => ProcessImageJob(serviceProvider),  // Method to run
                Cron.Hourly); // Adjust the Cron expression to schedule the job
        }

        // The method that will be called by Hangfire to process the job
        public static void ProcessImageJob(IServiceProvider serviceProvider)
        {
            var imageProcessingService = serviceProvider.GetRequiredService<IImageProcessingJob>();

            // Example: Fetch and process posts that need image resizing or conversion
            imageProcessingService.ProcessPendingImages(); // Make sure this method is implemented in PostService
        }
    }
}
