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
    member this.``Profile page shows article title``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173/#/profile/Anah%20Bene%C5%A1ov%C3%A1")

        let readMoreLink =
            page
                .GetByRole(AriaRole.Link)
                .Filter(LocatorFilterOptions(HasText = "Read more..."))

        do! readMoreLink.First.WaitForAsync()

        let! articleName =
            readMoreLink
                .Locator("xpath=../../div[2]/a[1]")
                .First.TextContentAsync()

        test <@ articleName.Length > 0 @>
    }

    [<Fact>]
    member this.``Home link in nav is clickable``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173/#/profile/Anah%20Bene%C5%A1ov%C3%A1")

        let homeLink = page.GetByText("Home")

        do! homeLink.First.WaitForAsync()

        do! homeLink.ClickAsync()

        let globalFeedTab = page.GetByText("Global Feed")

        do! globalFeedTab.First.WaitForAsync()

        let! isVisible = globalFeedTab.First.IsVisibleAsync()

        test <@ isVisible @>
    }

    [<Fact>]
    member this.``Can sign up``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let signUpLink = page.GetByText("Sign up")

        do! signUpLink.First.WaitForAsync()

        do! signUpLink.ClickAsync()

        let username = Guid.NewGuid().ToString()
        let usernameInput = page.GetByPlaceholder("Username")
        do! usernameInput.ClearAsync()
        do! usernameInput.TypeAsync(username)

        let email = $"{Guid.NewGuid()}@example.com"
        let emailInput = page.GetByPlaceholder("Email")
        do! emailInput.ClearAsync()
        do! emailInput.TypeAsync(email)

        let password = Guid.NewGuid().ToString()
        let passwordInput = page.GetByPlaceholder("Password")
        do! passwordInput.ClearAsync()
        do! passwordInput.TypeAsync(password)

        let signUpButton =
            page
                .GetByRole(AriaRole.Button)
                .Filter(LocatorFilterOptions(HasText = "Sign up"))

        do! signUpButton.ClickAsync()

        let subHeader = page.GetByText("A place to share your knowledge")
        do! subHeader.WaitForAsync()

        let! isSubHeaderVisible = subHeader.IsVisibleAsync()

        test <@ isSubHeaderVisible @>
    }

    [<Fact>]
    member this.``Can publish new article``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let signUpLink = page.GetByText("Sign up")

        do! signUpLink.First.WaitForAsync()

        do! signUpLink.ClickAsync()

        let username = Guid.NewGuid().ToString()
        let usernameInput = page.GetByPlaceholder("Username")
        do! usernameInput.ClearAsync()
        do! usernameInput.TypeAsync(username)

        let email = $"{Guid.NewGuid()}@example.com"
        let emailInput = page.GetByPlaceholder("Email")
        do! emailInput.ClearAsync()
        do! emailInput.TypeAsync(email)

        let password = Guid.NewGuid().ToString()
        let passwordInput = page.GetByPlaceholder("Password")
        do! passwordInput.ClearAsync()
        do! passwordInput.TypeAsync(password)

        let signUpButton =
            page
                .GetByRole(AriaRole.Button)
                .Filter(LocatorFilterOptions(HasText = "Sign up"))

        do! signUpButton.ClickAsync()

        let subHeader = page.GetByText("A place to share your knowledge")
        do! subHeader.WaitForAsync()

        let newArticleLink = page.GetByText("New Article")
        do! newArticleLink.First.WaitForAsync()
        do! newArticleLink.ClickAsync()


        let title = "Herp Derp Nerp Sherp"
        let titleInput = page.GetByPlaceholder("Article Title")
        do! titleInput.WaitForAsync()
        do! titleInput.TypeAsync(title)

        let description = "Description description description"
        let descriptionInput = page.GetByPlaceholder("What's this article about?")
        do! descriptionInput.WaitForAsync()
        do! descriptionInput.TypeAsync(description)

        let body =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Vitae purus faucibus ornare suspendisse sed. Eget magna fermentum iaculis eu. Etiam non quam lacus suspendisse faucibus interdum posuere lorem. Pellentesque massa placerat duis ultricies lacus sed turpis tincidunt. Sed enim ut sem viverra aliquet eget sit. Integer vitae justo eget magna fermentum iaculis. Porta nibh venenatis cras sed felis eget velit aliquet. Molestie a iaculis at erat pellentesque adipiscing commodo elit at. Nunc aliquet bibendum enim facilisis gravida neque. Sed felis eget velit aliquet sagittis id. Lectus quam id leo in vitae turpis massa. Quisque egestas diam in arcu cursus. Nec feugiat in fermentum posuere urna. In nisl nisi scelerisque eu ultrices vitae. Mattis molestie a iaculis at erat pellentesque adipiscing commodo elit. Elit eget gravida cum sociis natoque penatibus et magnis dis. Massa enim nec dui nunc mattis enim ut tellus elementum. Habitasse platea dictumst vestibulum rhoncus est pellentesque elit ullamcorper."

        let bodyInput = page.GetByPlaceholder("Write your article (in markdown)")
        do! bodyInput.WaitForAsync()
        do! bodyInput.TypeAsync(body)

        let publishButton = page.GetByRole(AriaRole.Button).First
        do! publishButton.ClickAsync()

        let articleTitle = page.GetByRole(AriaRole.Heading).Nth(0)

        do! articleTitle.WaitForAsync()
        let! articleTitleText = articleTitle.TextContentAsync()

        test <@ articleTitleText = title @>
    }

    [<Fact>]
    member this.``Can favorite an article``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let signUpLink = page.GetByText("Sign up")

        do! signUpLink.First.WaitForAsync()

        do! signUpLink.ClickAsync()

        let username = Guid.NewGuid().ToString()
        let usernameInput = page.GetByPlaceholder("Username")
        do! usernameInput.ClearAsync()
        do! usernameInput.TypeAsync(username)

        let email = $"{Guid.NewGuid()}@example.com"
        let emailInput = page.GetByPlaceholder("Email")
        do! emailInput.ClearAsync()
        do! emailInput.TypeAsync(email)

        let password = Guid.NewGuid().ToString()
        let passwordInput = page.GetByPlaceholder("Password")
        do! passwordInput.ClearAsync()
        do! passwordInput.TypeAsync(password)

        let signUpButton =
            page
                .GetByRole(AriaRole.Button)
                .Filter(LocatorFilterOptions(HasText = "Sign up"))

        do! signUpButton.ClickAsync()

        let subHeader = page.GetByText("A place to share your knowledge")
        do! subHeader.WaitForAsync()

        let favoriteButton = page.GetByRole(AriaRole.Button).First
        do! favoriteButton.ClickAsync()

        let profileLink = page.GetByText(username).First
        do! profileLink.ClickAsync()

        let bannerName = page.GetByRole(AriaRole.Heading).Nth(1)
        do! bannerName.WaitForAsync()

        let! name = bannerName.TextContentAsync()
        test <@ name = username @>

        let favoritePostsTab = page.GetByText("Favorited Posts")
        do! favoritePostsTab.ClickAsync()

        let favoriteButtonInFeedItem =
            page
                .GetByTestId("feedItems")
                .First.GetByRole(AriaRole.Button)

        do! favoriteButtonInFeedItem.First.WaitForAsync()

        let! favoritedItemsCount = favoriteButtonInFeedItem.CountAsync()

        test <@ favoritedItemsCount = 1 @>
    }

    [<Fact>]
    member this.``Run Scrutiny Test``() = task {
        let launchOptions = BrowserTypeLaunchOptions()
        launchOptions.Headless <- isHeadless
        //launchOptions.SlowMo <- 500f

        let! browser = playwright.Firefox.LaunchAsync(launchOptions)
        let! context = browser.NewContextAsync(BrowserNewContextOptions(IgnoreHTTPSErrors = true))

        let! page = context.NewPageAsync()

        let! _ = page.GotoAsync("http://localhost:5173")

        let seed = 553931187

        let config =
            { ScrutinyConfig.Default with
                Seed = seed
                MapOnly = false
                ComprehensiveActions = true
                ComprehensiveStates = true
            }

        let! result = scrutinize config (GlobalState(page, seed, logger)) ScrutinyStateMachine.home

        test <@ result.Graph.Length = 10 @>
        test <@ result.Steps.Length = 40 @>
    }

    interface IDisposable with
        member this.Dispose() = playwright.Dispose()
