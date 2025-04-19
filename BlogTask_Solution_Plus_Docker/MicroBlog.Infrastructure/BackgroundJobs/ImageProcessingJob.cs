using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MicroBlog.Core.Entities;
using MicroBlog.Infrastructure.Data;
using MicroBlog.Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using MicroBlog.Worker;
using System.Linq;

namespace MicroBlog.Api.Jobs
{
    public class ImageProcessingJob : IImageProcessingJob
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storage;
        private readonly IHttpClientFactory _httpFactory;

        // Desired responsive widths
        private readonly int[] _targetWidths = { 320, 640, 1024 };

        public ImageProcessingJob(
            ApplicationDbContext db,
            IStorageService storage,
            IHttpClientFactory httpFactory)
        {
            _db = db;
            _storage = storage;
            _httpFactory = httpFactory;
        }

        public async Task Process(int postId)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null || string.IsNullOrEmpty(post.OriginalImageUrl) || post.IsImageProcessed)
                return;

            try
            {
                // Download original image from Azure Blob using HTTP
                using var client = _httpFactory.CreateClient();
                await using var originalStream = await client.GetStreamAsync(post.OriginalImageUrl);
                using var image = await Image.LoadAsync(originalStream);

                string? primaryWebpUrl = null;

                foreach (var width in _targetWidths)
                {
                    // Clone & resize
                    var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(width, 0) // Maintain aspect ratio
                    }));

                    await using var outputStream = new MemoryStream();
                    await resizedImage.SaveAsWebpAsync(outputStream, new WebpEncoder
                    {
                        Quality = 80
                    });
                    outputStream.Position = 0;

                    // Upload to Blob Storage
                    string fileName = $"{post.Id}_{width}.webp";
                    string uploadedUrl = await _storage.UploadAsync(outputStream, "posts-webp", fileName);

                    if (width == 640)
                        primaryWebpUrl = uploadedUrl; // Pick mid-size as default
                }

                post.WebPImageUrl = primaryWebpUrl;
                //post.IsImageProcessed = true;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // You can add retry logic or logging here
                Console.WriteLine($"Image processing failed for Post {postId}: {ex.Message}");
            }
        }


        public async Task ProcessPendingImages()
        {

            var posts = await _db.Posts
    .Where(p => p.IsImageProcessed == false)
    .ToListAsync();

            foreach (var post in posts)
            {
                await Process(post.Id);
            }

        }
    }
}
