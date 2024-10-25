using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVVenues.Api.PersistenceModels.Media;
    
public interface IMediaRepository
{
    Task Delete(string key);
    Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken cancellationToken);
    Task<string> Upload(string contentType, Stream stream, CancellationToken cancellationToken);
}
