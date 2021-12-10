module SubtleConduit.Components.FeedTab

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

open Tailwind
open SubtleConduit.Types
open SubtleConduit.Services.Api
open Sutil.DOM
open System
open SubtleConduit.Elmish


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
