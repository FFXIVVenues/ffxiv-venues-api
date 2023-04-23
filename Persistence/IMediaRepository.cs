using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVVenues.Api.Persistence
{
    public interface IMediaRepository
    {
        Task Delete(string key);
        Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken cancellationToken);
        Uri GetUri(string key);
        Task<string> Upload(string contentType, Stream stream, CancellationToken cancellationToken);
    }
}