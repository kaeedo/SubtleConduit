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

    [<Fact>]
    member this.``Loads home page``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- false

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173/SubtleConduit/")

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
    member this.``Run Scrutiny Test``() = task {
        let isHeadless = Environment.GetEnvironmentVariable("CI") = "true"

        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless
        //launchOptions.SlowMo <- 500f

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173/SubtleConduit/")

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
