using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;
using System.Net;

namespace FFXIVVenues.WebHookService;

public static class WebHookRetryPolicy
{
    private static readonly TimeSpan[] Schedule =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(3),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(20),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(8),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(18),
        TimeSpan.FromHours(24),
    ];

    public static IAsyncPolicy<HttpResponseMessage> Create() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => (int) response.StatusCode >= 300)
            
            .WaitAndRetryAsync(
                Schedule.Length,
                (attempt, outcome, _) => NextDelay(attempt, outcome),
                (outcome, delay, attempt, context) =>
                {
                    var name = context.OperationKey;
                    if (outcome.Exception is not null)
                        Log.Warning(outcome.Exception, "Dispatch attempt {Attempt} for WebHook {WebHookName} failed; retrying in {Delay}", attempt, name, delay);
                    else
                        Log.Warning("Dispatch attempt {Attempt} for WebHook {WebHookName} returned {StatusCode}; retrying in {Delay}", attempt, name, outcome.Result.StatusCode, delay);
                    return Task.CompletedTask;
                });

    private static TimeSpan NextDelay(int attempt, DelegateResult<HttpResponseMessage> outcome)
    {
        var scheduled = Schedule[attempt - 1];

        var retryAfter = outcome.Result?.Headers.RetryAfter;
        var requested = retryAfter?.Delta
            ?? (retryAfter?.Date is { } date ? date - DateTimeOffset.UtcNow : (TimeSpan?)null);

        return requested is not null && requested > scheduled ? requested.Value : scheduled;
    }
}
