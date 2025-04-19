using System.Threading.Tasks;

namespace MicroBlog.Worker
{
    public interface IImageProcessingJob
    {
        Task Process(int postId);
        Task ProcessPendingImages();

    }
}