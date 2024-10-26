using Microsoft.Playwright;

namespace FFXIVVenues.PlayTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class WhenUsingAddYourVenueModal : PageTest
{

    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("https://ffxivvenues.com");
        await Page.GetByRole(AriaRole.Button, new () { NameRegex = new Regex("Add your venue") }).ClickAsync();
    }

    [Test]
    public async Task ThenModalShows()
    {
        await Page.GetByText("Join via Veni!").IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Join the discord!" }).Nth(0).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Meet Veni Ki!" }).Nth(0).IsVisibleAsync();
    }

    [Test]
    public async Task ThenJoinDiscordButtonTakesYouToDiscordServer()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Join the discord!" }).Nth(0).ClickAsync();

        var page = await Page.WaitForPopupAsync();
        await page.WaitForURLAsync(new Regex("discord\\.com"));
        await page.GetByText("Discord App Launched").IsVisibleAsync();
    }

    [Test]
    public async Task ThenMeetVeniButtonTakesYouToVeniOnDiscord()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Meet Veni Ki!" }).Nth(0).ClickAsync();

        var page = await Page.WaitForPopupAsync();
        await page.WaitForURLAsync(new Regex("discord\\.com"));
    }

}