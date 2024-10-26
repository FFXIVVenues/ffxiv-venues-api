using Microsoft.Playwright;

namespace FFXIVVenues.PlayTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class WhenVisitingVenuesSite : PageTest
{

    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("https://ffxivvenues.com");
    }

    [Test]
    public async Task BasePageLoads()
    {
        await Expect(Page).ToHaveTitleAsync(new Regex("FFXIV Venues"));
    }

    [Test]
    public async Task VenueCardsAreShown()
    {
        await Expect(Page.GetByText("Today")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ClickingAVenueCardOpensTheModal()
    {
        await Page.GetByPlaceholder("Search venues").TypeAsync("Red moon\n");
        await Page.WaitForTimeoutAsync(1000);
        await Page.GetByText("Red moon").Nth(0).ClickAsync();
        await Page.GetByText("All times are in your timezone").IsVisibleAsync();
        await Page.GetByText("Join Discord").IsVisibleAsync();
        await Page.GetByText("July 25th, 2020").IsVisibleAsync();
    }

    [Test]
    public async Task SearchBoxFiltersVenues()
    {
        await Page.GetByPlaceholder("Search venues").TypeAsync("Red moon\n");
        await Page.WaitForTimeoutAsync(1000);
        var redMoonCards = await Page.GetByText("Red moon").ElementHandlesAsync();
        Assert.That(redMoonCards, Is.Not.Empty);
        foreach (var card in redMoonCards)
        {
            Assert.That(await card.IsVisibleAsync(), Is.True);
        }
    }

    [Test]
    public async Task TagButtonsFiltersVenues()
    {
        await Page.GetByRole(AriaRole.Button, new () { NameString = "Jenova" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new () { NameString = "Bath house" }).ClickAsync();
        await Page.WaitForTimeoutAsync(1000);
        var onsenCards = await Page.GetByText("The Blooming Flowers Onsen").ElementHandlesAsync();
        Assert.That(onsenCards, Is.Not.Empty);
        foreach (var card in onsenCards)
        {
            Assert.That(await card.IsVisibleAsync(), Is.True);
        }
    }

}