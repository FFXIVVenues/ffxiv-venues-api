namespace FFXIVVenues.WebHookService;

public record WebHookEventPayload(
    string Id,
    long Timestamp,
    string Body,
    string Signature);
