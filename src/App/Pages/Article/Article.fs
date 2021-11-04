module SubtleConduit.Pages.Article

open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open Tailwind
open Sutil.DOM
open SubtleConduit.Components

let ArticlePage (slug: string) =
    let view =
        Html.div [
            text slug
        ]

    view
