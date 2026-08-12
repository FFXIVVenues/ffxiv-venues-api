using Polly;
using Serilog;
using System.Net.Http.Headers;
using System.Threading.Channels;

namespace FFXIVVenues.WebHookService;

public class WebHookDispatcher
{
    private readonly WebHook config;
    private readonly IHttpClientFactory http;
    private readonly Channel<WebHookEventPayload> channel;

    public WebHookDispatcher(WebHook config, IHttpClientFactory http)
    {
        this.config = config;
        this.http = http;
        this.channel = Channel.CreateUnbounded<WebHookEventPayload>(new UnboundedChannelOptions { SingleReader = false });
        _ = this.ProcessAsync();
    }

    public void Enqueue(string eventType, WebHookEventPayload payload)
    {
        if (!this.config.Events.Contains(eventType))
        {
            Log.Debug("Skipping dispatch of event {EventType} to WebHook {WebHookName} as it is not configured to receive this event type",
                eventType, this.config.Name);
            return;
        }

        Log.Debug("Queuing event {EventType} for WebHook {WebHookName} at {WebHookUrl}",
            eventType, this.config.Name, this.config.Url);
        this.channel.Writer.TryWrite(payload);
        Log.Information("Queued event {EventType} for WebHook {WebHookName}, {QueueLength} in queue", eventType, this.config.Name, this.channel.Reader.Count);
    }

    private async Task ProcessAsync()
    {
        await foreach (var payload in this.channel.Reader.ReadAllAsync())
            await this.SendAsync(payload);
    }

    private async Task SendAsync(WebHookEventPayload payload)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, this.config.Url)
        {
            Content = new StringContent(payload.Body, new MediaTypeHeaderValue("application/json", "utf-8")),
            Headers =
            {
                { "webhook-id", payload.Id },
                { "webhook-timestamp", payload.Timestamp.ToString() },
                { "webhook-signature", "v1a," + payload.Signature },
            }
        };
        message.SetPolicyExecutionContext(new Context(this.config.Name));

        try
        {
            var client = this.http.CreateClient($"webhook-{config.Name}");
            using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
                Log.Information("Dispatched event to WebHook {WebHookName}", this.config.Name);
            else
                Log.Warning("Failed to dispatch event to {WebHookName}. Status code: {StatusCode}", this.config.Name, response.StatusCode);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to dispatch event to {WebHookName}", this.config.Name);
        }
    }
}
