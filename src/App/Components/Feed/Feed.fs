module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open Tailwind

let Feed () =
    let view =
        Html.div [
            Attr.classes [
                tw.``flex-auto``
            ]
            text "feed"
        ]

    view // col-9
