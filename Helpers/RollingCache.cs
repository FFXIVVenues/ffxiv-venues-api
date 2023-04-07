using System;
using System.Collections.Concurrent;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FFXIVVenues.Api.Helpers
{
    public class RollingCache<T> : IDisposable
    {
        public long TimeoutInMs { get; }
        public long MaxAgeInMs { get; }

        private readonly ConcurrentDictionary<string, DateTime> _set = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastAccess = new();
        private readonly ConcurrentDictionary<string, T> _items = new();
        private readonly Timer _timer;
        private bool _disposed;

        public RollingCache(long timeoutInMs, long maxAgeInMs)
        {
            this.TimeoutInMs = timeoutInMs;
            this.MaxAgeInMs = maxAgeInMs;
            _timer = new Timer(_ => ExpireItems(), null, 60_000, 60_000);
        }

        public T Get(string key)
        {
            var exists = _items.TryGetValue(key, out var value);
            if (exists)
            {
                _lastAccess[key] = DateTime.UtcNow;
                return value;
            }
            return default;
        }

        public void Set(string key, T value)
        {
            _lastAccess[key] = DateTime.UtcNow;
            _set[key] = DateTime.UtcNow;
            _items[key] = value;
        }

        public T GetOrSet(string key, T value)
        {
            var success = _items.TryGetValue(key, out var item);
            _lastAccess[key] = DateTime.UtcNow;
            if (success && item != null)
                return item;
            _set[key] = DateTime.UtcNow;
            _items[key] = value;
            return value;
        }

        public T Remove(string key)
        {
            _lastAccess.TryRemove(key, out _);
            _set.TryRemove(key, out _);
            _items.TryRemove(key, out var value);
            return value;
        }

        public void Clear()
        {
            this._set.Clear();
            this._items.Clear();
            this._items.Clear();
        }

        private void ExpireItems()
        {
            foreach (var keyValuePair in _lastAccess)
            {
                if (keyValuePair.Value > DateTime.UtcNow.AddMilliseconds(-this.TimeoutInMs) 
                    && _set[keyValuePair.Key] > DateTime.UtcNow.AddMilliseconds(-this.MaxAgeInMs))
                    continue;
                
                Remove(keyValuePair.Key);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
                _timer.Dispose();

            _disposed = true;
        }
    }
}
