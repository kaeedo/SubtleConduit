module SubtleConduit.Components.FeedTab

open Sutil

open Sutil.CoreElements


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
// 8D211B52-2900-48B1-8906-60994AFB09D7
