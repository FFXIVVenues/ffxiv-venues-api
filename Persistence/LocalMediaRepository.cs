using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using FFXIVVenues.Api.Helpers;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.Persistence
{
    public class LocalMediaRepository : IMediaRepository
    {
        private readonly IConfiguration _config;

        private const string DEFAULT_MEDIA_LOCATION = "media/";
        private const string TYPES_FILE = DEFAULT_MEDIA_LOCATION + "contentTypes";

        private readonly string _mediaLocation;
        private readonly Dictionary<string, string> _contentTypes;
        
        public LocalMediaRepository(IConfiguration config)
        {
            this._config = config;
            this._mediaLocation = _config.GetValue("ConnectionString", DEFAULT_MEDIA_LOCATION);
            if (!this._mediaLocation.EndsWith('/'))
                this._mediaLocation += '/';
            Directory.CreateDirectory(this._mediaLocation);
            if (File.Exists(TYPES_FILE))
                this._contentTypes = File.ReadAllLines(TYPES_FILE).ToDictionary(l => l.Split("::")[0], l => l.Split("::")[1]);
            else
                this._contentTypes = new ();
        }

        public Task Delete(string key)
        {
            this._contentTypes.Remove(key);
            File.Delete(this._mediaLocation + key);
            return this.WriteTypesFileAsync();
        }

        public Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken _)
        {
            if (!this._contentTypes.ContainsKey(key))
                return Task.FromResult((Stream.Null, ""));
            return Task.FromResult((File.OpenRead(DEFAULT_MEDIA_LOCATION + key) as Stream, this._contentTypes[key]));
        }

        public Uri GetUri(string key)
        {
            var template = this._config.GetValue<string>("MediaStorage:BlobUriTemplate");
            return template == null ? null : new Uri(template.Replace("{key}", key));
        }

        public async Task<string> Upload(string contentType, Stream stream, CancellationToken cancellationToken)
        {
            var key = IdHelper.GenerateId();
            this._contentTypes[key] = contentType;
            await using (var fileStream = File.OpenWrite(DEFAULT_MEDIA_LOCATION + key))
                await stream.CopyToAsync(fileStream, cancellationToken);
            _ = this.WriteTypesFileAsync();
            return key;
        }

        private Task WriteTypesFileAsync()
        {
            var serialization = this._contentTypes.Select(kv => $"{kv.Key}::{kv.Value}");
            return File.WriteAllLinesAsync(TYPES_FILE, serialization);
        }

    }
}
