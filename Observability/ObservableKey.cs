﻿using System.Text.Json.Serialization;

namespace FFXIVVenues.Api.Observability
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ObservableKey
    {
        Manager,
        Id,
        DataCenter,
        World
    }
}