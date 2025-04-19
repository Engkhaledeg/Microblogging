using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MicroBlog.Core.Entities;
using MicroBlog.Infrastructure.Data;
using MicroBlog.Infrastructure.Services;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace MicroBlog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private static readonly string[] _allowedContentTypes = {
            MediaTypeNames.Image.Jpeg,
            "image/png",
            "image/webp"
        };

        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storage;
        private readonly IBackgroundJobClient _jobs;

        public PostsController(
            ApplicationDbContext db,
            IStorageService storage,
            IBackgroundJobClient jobs)
        {
            _db = db;
            _storage = storage;
            _jobs = jobs;
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(2 * 1024 * 1024)] // 2MB limit
        public async Task<IActionResult> Create([FromForm] PostCreateDto dto)
        {
            // 1. Validate post text
            if (string.IsNullOrWhiteSpace(dto.Text) || dto.Text.Length > 140)
                return BadRequest("Post text is required and must be <= 140 characters.");

            // 2. Validate image if provided
            string? originalUrl = null;
            string? imageFileName = null;

            if (dto.Image != null)
            {
                if (!_allowedContentTypes.Contains(dto.Image.ContentType))
                    return BadRequest("Only JPG, PNG, and WebP images are allowed.");

                if (dto.Image.Length > MaxFileSize)
                    return BadRequest("Image size must not exceed 2MB.");

                // 3. Upload original image
                var ext = Path.GetExtension(dto.Image.FileName).ToLower();
                imageFileName = $"{Guid.NewGuid()}{ext}";

                using var stream = dto.Image.OpenReadStream();
                originalUrl = await _storage.UploadAsync(stream, "posts-original", imageFileName);
            }

            // 4. Create post
            var post = new Post
            {
                UserName = User.Identity?.Name ?? "anonymous",
                Text = dto.Text,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                OriginalImageUrl = originalUrl,
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // 5. Enqueue image processing job if image was uploaded
            if (originalUrl != null)
            {
                _jobs.Enqueue<Jobs.ImageProcessingJob>(job => job.Process(post.Id));
            }

            // 6. Return the post
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, new
            {
                post.Id,
                post.UserName,
                post.Text,
                post.OriginalImageUrl,
                post.WebPImageUrl,
                post.CreatedAt,
                post.Latitude,
                post.Longitude
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            return post == null ? NotFound() : Ok(post);
        }




    }
}
