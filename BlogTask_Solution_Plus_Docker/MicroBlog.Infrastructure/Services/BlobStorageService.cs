using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace MicroBlog.Infrastructure.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(Stream stream, string container, string filename);
    }

    public class BlobStorageService : IStorageService
    {
        private readonly BlobServiceClient _client;
        public BlobStorageService(BlobServiceClient client) => _client = client;

        public async Task<string> UploadAsync(Stream stream, string container, string filename)
        {
            var contClient = _client.GetBlobContainerClient(container);
            await contClient.CreateIfNotExistsAsync();
            var blob = contClient.GetBlobClient(filename);
            await blob.UploadAsync(stream, overwrite: true);
            return blob.Uri.ToString();
        }
    }
}
