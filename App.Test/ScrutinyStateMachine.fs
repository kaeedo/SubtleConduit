namespace App.Test

open System
open Microsoft.Playwright
open Scrutiny

type GlobalState(page: IPage, logger: string -> unit) =
    member val Logger = logger
    member val Page = page

    member val SelectedAuthor = String.Empty with get, set

    member val ActiveArticle = String.Empty with get, set

    member x.GetInputValueAsync(selector: string) = task {
        let! element = x.Page.QuerySelectorAsync(selector)
        let! value = element.EvaluateAsync("e => e.value")
        return value.ToString()
    }


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
                destination profile

                via (fun _ -> task {
                    let author = gs.Page.GetByTestId("authorName").Nth(0)

                    do! author.WaitForAsync()

                    let! authorName = author.TextContentAsync()

                    gs.SelectedAuthor <- authorName
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

                    gs.ActiveArticle <- articleName

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

    let profile =
        fun (gs: GlobalState) -> page {
            name "Profile"

            onEnter (fun _ -> task {
                let bannerName = gs.Page.GetByRole(AriaRole.Heading).Nth(1)

                do! bannerName.WaitForAsync()
                let! name = bannerName.TextContentAsync()

                test <@ name = gs.SelectedAuthor @>
            })

            onExit (fun _ -> gs.SelectedAuthor <- String.Empty)

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("Favorited Posts")
                    do! favoritedPostsTab.ClickAsync()
                })
            }

            action {
                fn (fun _ -> task {
                    let favoritedPostsTab = gs.Page.GetByText("Posts")
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

                    gs.ActiveArticle <- articleName

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

    let article =
        fun (gs: GlobalState) -> page {
            name "Article"

            onEnter (fun _ -> task {
                let articleTitle = gs.Page.GetByRole(AriaRole.Heading).Nth(0)

                do! articleTitle.WaitForAsync()
                let! articleTitleText = articleTitle.TextContentAsync()

                test <@ articleTitleText = gs.ActiveArticle @>
            })

            onExit (fun _ -> task { gs.ActiveArticle <- String.Empty })

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
