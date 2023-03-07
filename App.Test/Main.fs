module App.Test.Main

open System
open Microsoft.Playwright
open Scrutiny
open Xunit
open Xunit.Abstractions

// https://blog.ploeh.dk/2014/03/21/composed-assertions-with-unquote/

type PlaywrightTests(outputHelper: ITestOutputHelper) =
    do
        Microsoft.Playwright.Program.Main([| "install" |])
        |> ignore

    let logger msg = outputHelper.WriteLine(msg)
    let playwright = Playwright.CreateAsync().GetAwaiter().GetResult()
    let isHeadless = false

    [<Fact>]
    member this.``Loads home page``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let header = page.GetByRole(AriaRole.Heading)
        let! text = header.First.InnerTextAsync()

        test <@ text = "conduit" @>

        do!
            page
                .GetByTestId("feedItems")
                .Locator("li")
                .Nth(0)
                .WaitForAsync()

        let! postCount =
            page
                .GetByTestId("feedItems")
                .Locator("li")
                .CountAsync()

        test <@ postCount > 1 @>

        do!
            page
                .GetByTestId("tags")
                .Locator("li")
                .Nth(0)
                .WaitForAsync()

        let! tagCount =
            page
                .GetByTestId("tags")
                .Locator("li")
                .CountAsync()

        test <@ tagCount > 1 @>
    }

    [<Fact>]
    member this.``Filters by tag``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let tags = page.GetByTestId("tags").Locator("li")
        do! tags.Nth(1).WaitForAsync()
        let! tagCount = tags.CountAsync()
        let rdn = Random()
        let tag = tags.Nth(rdn.Next(tagCount - 1))

        do! tag.ClickAsync()
        let! tagText = tag.TextContentAsync()

        let filteredFeedLabel =
            page
                .GetByText("Global Feed")
                .Locator("xpath=following-sibling::*")

        do! filteredFeedLabel.WaitForAsync()

        let! filteredFeedLabelText = filteredFeedLabel.TextContentAsync()

        test <@ filteredFeedLabelText = tagText @>
    }

    [<Fact>]
    member this.``Profile page shows author name in banner``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173/#/profile/Anah%20Bene%C5%A1ov%C3%A1")

        let bannerName = page.GetByRole(AriaRole.Heading).Nth(1)

        do! bannerName.WaitForAsync()

        let! name = bannerName.TextContentAsync()

        test <@ name = "Anah Benešová" @>
    }

    [<Fact>]
    member this.``Run Scrutiny Test``() = task {
        //let isHeadless = Environment.GetEnvironmentVariable("CI") = "true"

        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless
        //launchOptions.SlowMo <- 500f

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let config =
            { ScrutinyConfig.Default with
                Seed = 553931187
                MapOnly = false
                ComprehensiveActions = true
                ComprehensiveStates = true
            }

        let! result = scrutinize config (GlobalState(page, logger)) ScrutinyStateMachine.home
        // Assert.True(result.Steps |> Seq.length >= 5)
        // Assert.Equal(5, result.Graph.Length)
        test <@ 1 = 2 @>
    }

    interface IDisposable with
        member this.Dispose() = playwright.Dispose()
