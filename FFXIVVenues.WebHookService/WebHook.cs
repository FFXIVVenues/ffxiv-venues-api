using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIVVenues.WebHookService;

public record WebHook(string Name, string Url, string[] Events);
    