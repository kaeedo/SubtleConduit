module SubtleConduit.Components.TextInput

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open Tailwind

type Props = { Placeholder: string }

let TextInput (props: Props) =
    let view =
        Html.input [
            Attr.classes [
                tw.``border-2``
                tw.``border-solid``
                tw.rounded
                tw.``border-gray-200``
                tw.``px-6``
                tw.``py-3``
                tw.``w-full``
            ]
            Attr.placeholder props.Placeholder
            type' "text"
        ]

    view
