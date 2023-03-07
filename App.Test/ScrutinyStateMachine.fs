namespace App.Test

open System
open Microsoft.Playwright
open Scrutiny

type GlobalState(page: IPage, logger: string -> unit) =
    member val Logger = logger
    member val Page = page

    member val SelectedAuthor = String.Empty with get, set

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
                destination profilePage

                via (fun _ -> task {
                    let author = gs.Page.GetByTestId("authorName").Nth(0)

                    do! author.WaitForAsync()

                    let! authorName = author.TextContentAsync()

                    gs.SelectedAuthor <- authorName
                })
            }
        }

    let profilePage =
        fun (gs: GlobalState) -> page {
            name "Profile"

            onEnter (fun _ -> task {
                let bannerName = gs.Page.GetByRole(AriaRole.Heading).Nth(1)

                do! bannerName.WaitForAsync()
                let! name = bannerName.TextContentAsync()

                test <@ name = gs.SelectedAuthor @>
            })
        }
