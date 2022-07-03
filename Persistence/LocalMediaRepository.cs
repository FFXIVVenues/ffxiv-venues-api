using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace FFXIVVenues.Api.Persistence
{
    public class LocalMediaRepository : IMediaRepository, IDisposable
    {

        private const string MEDIA_LOCATION = "media/";
        private const string TYPES_FILE = MEDIA_LOCATION + "contentTypes";
        private readonly Dictionary<string, string> _contentTypes;
        private readonly Timer _timer;
        private bool disposedValue;

        public LocalMediaRepository()
        {
            Directory.CreateDirectory(MEDIA_LOCATION);
            if (File.Exists(TYPES_FILE))
                this._contentTypes = File.ReadAllLines(TYPES_FILE).ToDictionary(l => l.Split("::")[0], l => l.Split("::")[1]);
            else
                this._contentTypes = new ();
            this._timer = new Timer(this.WriteFile, null, 300_000, 300_000);
        }

        public Task Delete(string key)
        {
            this._contentTypes.Remove(key);
            File.Delete(MEDIA_LOCATION + key);
            return Task.CompletedTask;
        }

        public Task<(Stream Stream, string ContentType)> Download(string key, CancellationToken _)
        {
            if (!this._contentTypes.ContainsKey(key))
                return Task.FromResult((Stream.Null, ""));
            return Task.FromResult((File.OpenRead(MEDIA_LOCATION + key) as Stream, this._contentTypes[key]));
        }

        public async Task Upload(string key, string contentType, Stream stream, CancellationToken _)
        {
            this._contentTypes[key] = contentType;
            using (var fileStream = File.OpenWrite(MEDIA_LOCATION + key))
                await stream.CopyToAsync(fileStream);
        }

        private void WriteFile(object _)
        {
            var serialization = this._contentTypes.Select(kv => $"{kv.Key}::{kv.Value}");
            File.WriteAllLines(TYPES_FILE, serialization);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    this._timer.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
