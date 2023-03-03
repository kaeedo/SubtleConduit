namespace App.Test

open Microsoft.Playwright
open Scrutiny

type GlobalState(page: IPage, logger: string -> unit) =
    member val Logger = logger
    member val Page = page

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
        }
