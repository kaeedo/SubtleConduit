module SubtleConduit.Components.FeedTab

open Sutil

open Sutil.Core
open Sutil.CoreElements
open Sutil.DomHelpers


let FeedTab label clickHandler isSelected =
    let view =
        Html.div [
            Bind.toggleClass (isSelected, "border-b-2 border-conduit-green")
            Attr.classes [
                "text-conduit-green"
                "w-max"
                "py-2"
                "px-4"
                "box-content"
                "cursor-pointer"
            ]
            onClick (clickHandler) []
            text label
        ]

    view
