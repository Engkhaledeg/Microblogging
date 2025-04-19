using Microsoft.AspNetCore.Http;

public class PostCreateDto
{
    public string Text { get; set; }
    public IFormFile? Image { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
