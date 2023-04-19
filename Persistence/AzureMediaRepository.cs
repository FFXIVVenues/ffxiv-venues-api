﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FFXIVVenues.Api.Helpers;

namespace FFXIVVenues.Api.Persistence
{
    public class AzureMediaRepository : IMediaRepository
    {
        private readonly IConfiguration _config;

        public AzureMediaRepository(IConfiguration config) =>
            _config = config;

        public async Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken cancellationToken)
        {
            var container = GetBlobContainerClient();
            var blob = container.GetBlobClient(key);
            var response = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);
            
            return (response.Value.Content, response.Value.Details.ContentType);
        }

        public Uri GetUri(string key)
        {
            var template = this._config.GetValue<string>("MediaStorage:BlobUriTemplate");
            return template == null ? null : new Uri(template.Replace("{key}", key));
        }

        public async Task<string> Upload(string contentType, Stream stream, CancellationToken cancellationToken)
        {
            var key = IdHelper.GenerateId();
            var container = GetBlobContainerClient();
            var blob = container.GetBlobClient(key);
            await blob.DeleteIfExistsAsync(cancellationToken:cancellationToken);
            _ = await blob.UploadAsync(stream,
                                 httpHeaders: new BlobHttpHeaders { ContentType = contentType },
                                 transferOptions: new StorageTransferOptions { MaximumTransferSize = 1_048_576 },
                                 cancellationToken: cancellationToken);
            return key;
        }

        public async Task Delete(string key)
        {
            var container = GetBlobContainerClient();
            var blob = container.GetBlobClient(key);
            _ = await blob.DeleteIfExistsAsync();
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            var connectionString = _config.GetValue<string>("MediaStorage:ConnectionString");
            if (connectionString == null)
                throw new Exception("No connection string configured for media storage.");

            var containerName = _config.GetValue<string>("MediaStorage:ContainerName");
            if (containerName == null)
                throw new Exception("No connection string configured for media storage.");

            var container = new BlobContainerClient(connectionString, containerName);
            return container;
        }

    }
}
