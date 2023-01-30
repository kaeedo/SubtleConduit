module SubtleConduit.Components.FeedTab

open Sutil
open Sutil.Attr

open Sutil.DOM


let FeedTab label clickHandler isSelected =
    let view =
        Html.div [
            Bind.toggleClass (isSelected, $"{tw.``border-b-2``} {tw.``border-conduit-green``}")
            Attr.classes [
                tw.``text-conduit-green``
                tw.``w-max``
                tw.``py-2``
                tw.``px-4``
                tw.``box-content``
                tw.``cursor-pointer``
            ]
            onClick (clickHandler) []
            text label
        ]

    view
