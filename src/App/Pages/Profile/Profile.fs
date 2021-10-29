module SubtleConduit.Pages.Profile

open Sutil
open SubtleConduit.Components
open Tailwind

let ProfilePage username =
    let view =
        Html.div [
            text username
        ]

    view
