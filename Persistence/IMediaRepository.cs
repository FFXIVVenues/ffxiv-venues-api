using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVVenues.Api.Persistence
{
    public interface IMediaRepository
    {
        Task Delete(string key);
        Task<BlobDownloadStreamingResult> Download(string key, CancellationToken cancellationToken);
        Task Upload(string key, string contentType, Stream stream, CancellationToken cancellationToken);
    }
}