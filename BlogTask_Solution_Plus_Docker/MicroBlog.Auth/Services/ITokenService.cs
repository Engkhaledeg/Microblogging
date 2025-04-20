namespace MicroBlog.Auth.Services
{
    public interface ITokenService
    {
        string GenerateToken(string username);
    }
}