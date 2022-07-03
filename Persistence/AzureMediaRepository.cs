using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVVenues.Api.Persistence
{
    public class AzureMediaRepository : IMediaRepository
    {
        private IConfiguration _config;

        public AzureMediaRepository(IConfiguration config) =>
            _config = config;

        public async Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken cancellationToken)
        {
            var connectionString = _config.GetValue<string>("MediaStorage:ConnectionString");
            if (connectionString == null)
                throw new Exception("No connection string configured for media storage.");

            var containerName = _config.GetValue<string>("MediaStorage:ContainerName");
            if (containerName == null)
                throw new Exception("No connection string configured for media storage.");

            var container = new BlobContainerClient(connectionString, containerName);
            var blob = container.GetBlobClient(key);
            var response = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);

            return (response.Value.Content, response.Value.Details.ContentType);
        }

        public async Task Upload(string key, string contentType, Stream stream, CancellationToken cancellationToken)
        {
            var connectionString = _config.GetValue<string>("MediaStorage:ConnectionString");
            if (connectionString == null)
                throw new Exception("No connection string configured for media storage.");

            var containerName = _config.GetValue<string>("MediaStorage:ContainerName");
            if (containerName == null)
                throw new Exception("No connection string configured for media storage.");

            var container = new BlobContainerClient(connectionString, containerName);
            var blob = container.GetBlobClient(key);
            await blob.DeleteIfExistsAsync();
            _ = await blob.UploadAsync(stream,
                                 httpHeaders: new BlobHttpHeaders() { ContentType = contentType },
                                 transferOptions: new StorageTransferOptions { MaximumTransferSize = 1_048_576 },
                                 cancellationToken: cancellationToken);
        }

        public async Task Delete(string key)
        {
            var connectionString = _config.GetValue<string>("MediaStorage:ConnectionString");
            if (connectionString == null)
                throw new Exception("No connection string configured for media storage.");

            var containerName = _config.GetValue<string>("MediaStorage:ContainerName");
            if (containerName == null)
                throw new Exception("No connection string configured for media storage.");

            var container = new BlobContainerClient(connectionString, containerName);
            var blob = container.GetBlobClient(key);
            _ = await blob.DeleteIfExistsAsync();
        }

    }
}
