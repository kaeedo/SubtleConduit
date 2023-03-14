namespace App.Test

open System
open Microsoft.Playwright
open Scrutiny

type GlobalState(page: IPage, logger: string -> unit) =
    do printfn "constructed"
    member val Logger = logger
    member val Page = page

    member val SelectedAuthor = String.Empty with get, set

    member val ActiveArticleName = String.Empty with get, set

    member val Username = String.Empty with get, set
    member val Email = String.Empty with get, set
    member val Password = String.Empty with get, set

module rec ScrutinyStateMachine =
    let home =
        fun (gs: GlobalState) -> page {
            name "Home"

            onEnter (fun _ -> task {
                let header = gs.Page.GetByRole(AriaRole.Heading)
                let! text = header.First.InnerTextAsync()

                test <@ text = "conduit" @>

                do!
                    gs
                        .Page
                        .GetByTestId("feedItems")
                        .Locator("li")
                        .Nth(0)
                        .WaitForAsync()

                let! postCount =
                    gs
                        .Page
                        .GetByTestId("feedItems")
                        .Locator("li")
                        .CountAsync()

                test <@ postCount > 1 @>

                do!
                    gs
                        .Page
                        .GetByTestId("tags")
                        .Locator("li")
                        .Nth(0)
                        .WaitForAsync()

                let! tagCount =
                    gs
                        .Page
                        .GetByTestId("tags")
                        .Locator("li")
                        .CountAsync()

                test <@ tagCount > 1 @>
            })

            action {
                name "Filter by tag"

                fn (fun _ -> task {
                    let tags = gs.Page.GetByTestId("tags").Locator("li")
                    do! tags.Nth(1).WaitForAsync()
                    let! tagCount = tags.CountAsync()
                    let rdn = Random()
                    let tag = tags.Nth(rdn.Next(tagCount - 1))

                    do! tag.ClickAsync()
                    let! tagText = tag.TextContentAsync()

                    let filteredFeedLabel =
                        gs
                            .Page
                            .GetByText("Global Feed")
                            .Locator("xpath=following-sibling::*")

                    do! filteredFeedLabel.WaitForAsync()

                    let! filteredFeedLabelText = filteredFeedLabel.TextContentAsync()

                    test <@ filteredFeedLabelText = tagText @>
                })
            }

            action {
                dependantActions [ "Filter by tag" ]

                fn (fun _ -> task {
                    let globalFeedLabel = gs.Page.GetByText("Global Feed")

                    do! globalFeedLabel.ClickAsync()
                })
            }

            transition {
                destination authorProfile

                via (fun _ -> task {
                    let author = gs.Page.GetByTestId("authorName").Nth(0)

                    do! author.WaitForAsync()

                    let! authorName = author.TextContentAsync()

                    gs.SelectedAuthor <- authorName

                    do! author.ClickAsync()
                })
            }

            transition {
                destination article

                via (fun _ -> task {
                    let readMoreLink =
                        gs.Page.GetByRole(AriaRole.Link).Filter(
                            LocatorFilterOptions(HasText = "Read more...")
                        )
                            .First

                    do! readMoreLink.WaitForAsync()

                    let! articleName =
                        readMoreLink
                            .Locator("xpath=../../div[2]/a[1]")
                            .First.TextContentAsync()

                    gs.ActiveArticleName <- articleName

                    do! readMoreLink.ClickAsync()
                })
            }

            transition {
                destination signUp

                via (fun _ -> task {
                    let signUpLink = gs.Page.GetByText("Sign up")

                    do! signUpLink.First.WaitForAsync()

                    do! signUpLink.ClickAsync()
                })
            }

            transition {
                destination signIn

                via (fun _ -> task {
                    let signInLink = gs.Page.GetByText("Sign In")

                    do! signInLink.First.WaitForAsync()

                    do! signInLink.ClickAsync()
                })
            }
        }

    let loggedInHome =
        fun (gs: GlobalState) -> page {
            name "Logged in Home"

            onEnter (fun _ -> task {
                let header = gs.Page.GetByRole(AriaRole.Heading)
                let! text = header.First.InnerTextAsync()

                test <@ text = "conduit" @>

                let globalFeed = gs.Page.GetByText("Global Feed")

                do! globalFeed.WaitForAsync()
                let! isGlobalFeedVisible = globalFeed.IsVisibleAsync()

                test <@ isGlobalFeedVisible @>

                let yourFeed = gs.Page.GetByText("Your Feed")

                do! yourFeed.WaitForAsync()
                let! isYourFeedVisible = yourFeed.IsVisibleAsync()

                test <@ isYourFeedVisible @>
            })

            action {
                name "Filter by tag"

                fn (fun _ -> task {
                    let tags = gs.Page.GetByTestId("tags").Locator("li")
                    do! tags.Nth(1).WaitForAsync()
                    let! tagCount = tags.CountAsync()
                    let rdn = Random()
                    let tag = tags.Nth(rdn.Next(tagCount - 1))

                    do! tag.ClickAsync()
                    let! tagText = tag.TextContentAsync()

                    let filteredFeedLabel =
                        gs
                            .Page
                            .GetByText("Global Feed")
                            .Locator("xpath=following-sibling::*")

                    do! filteredFeedLabel.WaitForAsync()

                    let! filteredFeedLabelText = filteredFeedLabel.TextContentAsync()

                    test <@ filteredFeedLabelText = tagText @>
                })
            }

            action {
                dependantActions [ "Filter by tag" ]

                fn (fun _ -> task {
                    let globalFeedLabel = gs.Page.GetByText("Global Feed")

                    do! globalFeedLabel.ClickAsync()
                })
            }

            action {
                fn (fun _ -> task {
                    let youFeedLabel = gs.Page.GetByText("Your Feed")

                    do! youFeedLabel.ClickAsync()
                })
            }

            transition {
                destination loggedInAuthorProfile

                via (fun _ -> task {
                    let author = gs.Page.GetByTestId("authorName").Nth(0)

                    do! author.WaitForAsync()

                    let! authorName = author.TextContentAsync()

                    gs.SelectedAuthor <- authorName
                })
            }

            transition {
                destination loggedInArticle

                via (fun _ -> task {
                    let readMoreLink =
                        gs.Page.GetByRole(AriaRole.Link).Filter(
                            LocatorFilterOptions(HasText = "Read more...")
                        )
                            .First

                    do! readMoreLink.WaitForAsync()

                    let! articleName =
                        readMoreLink
                            .Locator("xpath=../../div[2]/a[1]")
                            .First.TextContentAsync()

                    gs.ActiveArticleName <- articleName

                    do! readMoreLink.ClickAsync()
                })
            }

            transition {
                destination newArticle

                via (fun _ -> task {
                    let newArticleLink = gs.Page.GetByText("New Article")

                    do! newArticleLink.First.WaitForAsync()

                    do! newArticleLink.ClickAsync()
                })
            }

            transition {
                destination settings

                via (fun _ -> task {
                    let settingsLink = gs.Page.GetByText("Settings")

                    do! settingsLink.First.WaitForAsync()

                    do! settingsLink.ClickAsync()
                })
            }
        }

    let authorProfile =
        fun (gs: GlobalState) -> page {
            name "Author Profile"

            onEnter (fun _ -> task {
                let bannerName = gs.Page.GetByRole(AriaRole.Heading).Nth(1)

                do! bannerName.WaitForAsync()
                let! name = bannerName.TextContentAsync()

                test <@ name = gs.SelectedAuthor @>
            })

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("Favorited Posts")
                    do! favoritedPostsTab.ClickAsync()
                })
            }

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("Posts").First

                    do! favoritedPostsTab.ClickAsync()
                })
            }

            transition {
                destination article

                via (fun _ -> task {
                    let readMoreLink =
                        gs
                            .Page
                            .GetByRole(AriaRole.Link)
                            .Filter(LocatorFilterOptions(HasText = "Read more..."))

                    do! readMoreLink.First.WaitForAsync()

                    let! articleName =
                        readMoreLink
                            .Locator("xpath=../../div[2]/a[1]")
                            .First.TextContentAsync()

                    gs.ActiveArticleName <- articleName

                    do! readMoreLink.ClickAsync()
                })
            }

            transition {
                destination home

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }

            transition {
                destination signUp

                via (fun _ -> task {
                    let signUpLink = gs.Page.GetByText("Sign up")

                    do! signUpLink.First.WaitForAsync()

                    do! signUpLink.ClickAsync()
                })
            }

            transition {
                destination signIn

                via (fun _ -> task {
                    let signInLink = gs.Page.GetByText("Sign In")

                    do! signInLink.First.WaitForAsync()

                    do! signInLink.ClickAsync()
                })
            }
        }

    let loggedInAuthorProfile =
        fun (gs: GlobalState) -> page {
            name "Logged in author Profile"

            onEnter (fun _ -> task {
                let bannerName = gs.Page.GetByRole(AriaRole.Heading).Nth(1)

                do! bannerName.WaitForAsync()
                let! name = bannerName.TextContentAsync()

                gs.SelectedAuthor <- name

                test <@ name = gs.Username @>
            })

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("Favorited Posts")
                    do! favoritedPostsTab.ClickAsync()
                })
            }

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("My Posts")
                    do! favoritedPostsTab.ClickAsync()
                })
            }

            // TODO revisit with conditional steps
            action {
                fn (fun _ -> task {
                    if gs.SelectedAuthor = gs.Username then
                        let editProfileSettings = gs.Page.GetByText($"Edit profile settings")
                        let! isEditProfileSettingsButtonVisible = editProfileSettings.IsVisibleAsync()

                        test <@ isEditProfileSettingsButtonVisible @>
                    else
                        let followButton = gs.Page.GetByText($"+ Follow {gs.SelectedAuthor}")
                        do! followButton.ClickAsync()

                        let unfollowButton = gs.Page.GetByText($"- Unfollow {gs.SelectedAuthor}")
                        let! isUnfollowButtonVisible = unfollowButton.IsVisibleAsync()

                        test <@ isUnfollowButtonVisible @>
                })
            }

            // TODO revisit with conditional steps
            // transition {
            //     destination loggedInArticle
            //
            //     via (fun _ -> task {
            //         let readMoreLink =
            //             gs.Page.GetByRole(AriaRole.Link).Filter(
            //                 LocatorFilterOptions(HasText = "Read more...")
            //             )
            //                 .First
            //
            //         do! readMoreLink.WaitForAsync()
            //
            //         let! articleName =
            //             readMoreLink
            //                 .Locator("xpath=../../div[2]/a[1]")
            //                 .First.TextContentAsync()
            //
            //         gs.ActiveArticleName <- articleName
            //
            //         do! readMoreLink.ClickAsync()
            //     })
            // }

            transition {
                destination loggedInHome

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }

            transition {
                destination newArticle

                via (fun _ -> task {
                    let newArticleLink = gs.Page.GetByText("New Article")

                    do! newArticleLink.First.WaitForAsync()

                    do! newArticleLink.ClickAsync()
                })
            }

            transition {
                destination settings

                via (fun _ -> task {
                    let settingsLink = gs.Page.GetByText("Settings")

                    do! settingsLink.First.WaitForAsync()

                    do! settingsLink.ClickAsync()
                })
            }

            transition {
                destination loggedInAuthorProfile

                via (fun _ -> task {
                    let profileLink = gs.Page.GetByText(gs.Username)

                    do! profileLink.First.WaitForAsync()

                    do! profileLink.ClickAsync()
                })
            }
        }

    let article =
        fun (gs: GlobalState) -> page {
            name "Article"

            onEnter (fun _ -> task {
                let articleTitle = gs.Page.GetByRole(AriaRole.Heading).Nth(0)

                do! articleTitle.WaitForAsync()
                let! articleTitleText = articleTitle.TextContentAsync()

                test <@ articleTitleText = gs.ActiveArticleName @>
            })

            transition {
                destination home

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }

            transition {
                destination signUp

                via (fun _ -> task {
                    let signUpLink = gs.Page.GetByText("Sign up")

                    do! signUpLink.First.WaitForAsync()

                    do! signUpLink.ClickAsync()
                })
            }

            transition {
                destination signIn

                via (fun _ -> task {
                    let signInLink = gs.Page.GetByText("Sign In")

                    do! signInLink.First.WaitForAsync()

                    do! signInLink.ClickAsync()
                })
            }
        }

    let loggedInArticle =
        fun (gs: GlobalState) -> page {
            name "Logged in Article"

            onEnter (fun _ -> task {
                let articleTitle = gs.Page.GetByRole(AriaRole.Heading).Nth(0)

                do! articleTitle.WaitForAsync()
                let! articleTitleText = articleTitle.TextContentAsync()

                test <@ articleTitleText = gs.ActiveArticleName @>
            })

            transition {
                destination newArticle

                via (fun _ -> task {
                    let newArticleLink = gs.Page.GetByText("New Article")

                    do! newArticleLink.First.WaitForAsync()

                    do! newArticleLink.ClickAsync()
                })
            }

            transition {
                destination settings

                via (fun _ -> task {
                    let settingsLink = gs.Page.GetByText("Settings").First

                    do! settingsLink.WaitForAsync()

                    do! settingsLink.ClickAsync()
                })
            }

            transition {
                destination loggedInAuthorProfile

                via (fun _ -> task {
                    let profileLink = gs.Page.GetByText(gs.Username).First

                    do! profileLink.WaitForAsync()

                    do! profileLink.ClickAsync()
                })
            }

            transition {
                destination loggedInHome

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }
        }

    let newArticle =
        fun (gs: GlobalState) -> page {
            name "New Article"

            onEnter (fun _ -> task {
                let publishButton = gs.Page.GetByRole(AriaRole.Button).First

                do! publishButton.WaitForAsync()

                let! isActive = publishButton.IsEnabledAsync()

                test <@ isActive @>
            })

            action {
                name "Article title"

                fn (fun _ -> task {
                    let title = "Herp Derp Nerp Sherp"
                    gs.ActiveArticleName <- title

                    let titleInput = gs.Page.GetByPlaceholder("Article Title")

                    do! titleInput.WaitForAsync()

                    do! titleInput.ClearAsync()

                    do! titleInput.TypeAsync(title)

                    let! inputText = titleInput.InputValueAsync()

                    test <@ title = inputText @>
                })
            }

            action {
                name "Article description"

                fn (fun _ -> task {
                    let description = "Description description description"

                    let descriptionInput = gs.Page.GetByPlaceholder("What's this article about?")

                    do! descriptionInput.WaitForAsync()

                    do! descriptionInput.ClearAsync()

                    do! descriptionInput.TypeAsync(description)

                    let! inputText = descriptionInput.InputValueAsync()

                    test <@ description = inputText @>
                })
            }

            action {
                name "Article body"

                fn (fun _ -> task {
                    let body =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Vitae purus faucibus ornare suspendisse sed. Eget magna fermentum iaculis eu. Etiam non quam lacus suspendisse faucibus interdum posuere lorem. Pellentesque massa placerat duis ultricies lacus sed turpis tincidunt. Sed enim ut sem viverra aliquet eget sit. Integer vitae justo eget magna fermentum iaculis. Porta nibh venenatis cras sed felis eget velit aliquet. Molestie a iaculis at erat pellentesque adipiscing commodo elit at. Nunc aliquet bibendum enim facilisis gravida neque. Sed felis eget velit aliquet sagittis id. Lectus quam id leo in vitae turpis massa. Quisque egestas diam in arcu cursus. Nec feugiat in fermentum posuere urna. In nisl nisi scelerisque eu ultrices vitae. Mattis molestie a iaculis at erat pellentesque adipiscing commodo elit. Elit eget gravida cum sociis natoque penatibus et magnis dis. Massa enim nec dui nunc mattis enim ut tellus elementum. Habitasse platea dictumst vestibulum rhoncus est pellentesque elit ullamcorper."

                    let bodyInput = gs.Page.GetByPlaceholder("Write your article (in markdown)")

                    do! bodyInput.WaitForAsync()

                    do! bodyInput.ClearAsync()

                    do! bodyInput.TypeAsync(body)

                    let! inputText = bodyInput.InputValueAsync()

                    test <@ body = inputText @>
                })
            }

            transition {
                destination loggedInArticle

                dependantActions [
                    "Article title"
                    "Article description"
                    "Article body"
                ]

                via (fun _ -> task {
                    let publishButton = gs.Page.GetByRole(AriaRole.Button).First

                    do! publishButton.ClickAsync()
                })
            }

            transition {
                destination settings

                via (fun _ -> task {
                    let settingsLink = gs.Page.GetByText("Settings")

                    do! settingsLink.First.WaitForAsync()

                    do! settingsLink.ClickAsync()
                })
            }

            transition {
                destination loggedInAuthorProfile

                via (fun _ -> task {
                    let profileLink = gs.Page.GetByText(gs.Username)

                    do! profileLink.First.WaitForAsync()

                    do! profileLink.ClickAsync()
                })
            }

            transition {
                destination loggedInHome

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }
        }

    let settings =
        fun (gs: GlobalState) -> page {
            name "Settings"

            onEnter (fun _ -> task {
                let usernameInput = gs.Page.GetByPlaceholder("Username")
                do! usernameInput.WaitForAsync()

                let! username = usernameInput.InputValueAsync()

                test <@ username = gs.Username @>

                let emailInput = gs.Page.GetByPlaceholder("E-mail")
                do! emailInput.WaitForAsync()

                let! email = emailInput.InputValueAsync()

                test <@ email = gs.Email @>
            })

            action {
                isExit

                fn (fun _ -> task {
                    let logoutButton = gs.Page.GetByText("Or click here to logout")

                    do! logoutButton.First.WaitForAsync()

                    do! logoutButton.ClickAsync()
                })
            }

            transition {
                destination loggedInHome

                via (fun _ -> task {
                    let emailInput = gs.Page.GetByPlaceholder("E-mail")

                    let newEmail = $"{Guid.NewGuid()}@example.com"

                    do! emailInput.TypeAsync(newEmail)

                    gs.Email <- newEmail

                    let updateSettings = gs.Page.GetByText("Update Settings")

                    do! updateSettings.ClickAsync()
                })
            }

            transition {
                destination home

                via (fun _ -> task {
                    let logoutButton = gs.Page.GetByText("Or click here to logout")

                    do! logoutButton.First.WaitForAsync()

                    do! logoutButton.ClickAsync()
                })
            }

            transition {
                destination loggedInAuthorProfile

                via (fun _ -> task {
                    let profileLink = gs.Page.GetByText(gs.Username)

                    do! profileLink.First.WaitForAsync()

                    do! profileLink.ClickAsync()
                })
            }

            transition {
                destination loggedInHome

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }
        }

    let signIn =
        fun (gs: GlobalState) -> page {
            name "Sign In"

            onEnter (fun _ -> task {
                let header = gs.Page.GetByRole(AriaRole.Heading).First

                let! isVisible = header.IsVisibleAsync()

                test <@ isVisible @>
            })

            transition {
                destination home

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }

            transition {
                destination signUp

                via (fun _ -> task {
                    let signUpLink = gs.Page.GetByText("Sign up")

                    do! signUpLink.First.WaitForAsync()

                    do! signUpLink.ClickAsync()
                })
            }
        }

    let signUp =
        fun (gs: GlobalState) -> page {
            name "Sign Up"

            onEnter (fun _ -> task {
                let header = gs.Page.GetByRole(AriaRole.Heading).First

                let! isVisible = header.IsVisibleAsync()

                test <@ isVisible @>
            })

            action {
                name "Write username"

                fn (fun _ -> task {
                    let username = Guid.NewGuid().ToString()
                    let usernameInput = gs.Page.GetByPlaceholder("Username")
                    do! usernameInput.ClearAsync()
                    do! usernameInput.TypeAsync(username)

                    gs.Username <- username
                })
            }

            action {
                name "Write email"

                fn (fun _ -> task {
                    let email = $"{Guid.NewGuid()}@example.com"
                    let emailInput = gs.Page.GetByPlaceholder("Email")
                    do! emailInput.ClearAsync()
                    do! emailInput.TypeAsync(email)

                    gs.Email <- email
                })
            }

            action {
                name "Write password"

                fn (fun _ -> task {
                    let password = Guid.NewGuid().ToString()
                    let passwordInput = gs.Page.GetByPlaceholder("Password")
                    do! passwordInput.ClearAsync()
                    do! passwordInput.TypeAsync(password)

                    gs.Password <- password
                })
            }

            transition {
                dependantActions [
                    "Write username"
                    "Write email"
                    "Write password"
                ]

                destination loggedInHome

                via (fun _ -> task {
                    let signUpButton =
                        gs
                            .Page
                            .GetByRole(AriaRole.Button)
                            .Filter(LocatorFilterOptions(HasText = "Sign up"))

                    do! signUpButton.ClickAsync()

                    let subHeader = gs.Page.GetByText("A place to share your knowledge")
                    do! subHeader.WaitForAsync()
                })
            }

            transition {
                destination home

                via (fun _ -> task {
                    let homeLink = gs.Page.GetByText("Home")

                    do! homeLink.First.WaitForAsync()

                    do! homeLink.ClickAsync()
                })
            }

            transition {
                destination signIn

                via (fun _ -> task {
                    let signInLink = gs.Page.GetByText("Sign In")

                    do! signInLink.First.WaitForAsync()

                    do! signInLink.ClickAsync()
                })
            }
        }
