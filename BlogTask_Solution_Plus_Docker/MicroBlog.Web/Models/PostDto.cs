using System;

namespace MicroBlog.Web.Models
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
