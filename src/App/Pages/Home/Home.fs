module SubtleConduit.Pages.Home

open Sutil
open Sutil.Attr

let HomePage () =
    let view =
        Html.div [
            class' "bg-conduit-green w-screen p-8"
            Html.div [
                class' "container mx-auto text-white"
                Html.h1 [
                    class' "mx-auto text-center text-6xl font-mono font-bold text-shadow-lg mb-2"
                    text "conduit"
                ]
                Html.h2 [
                    class' "mx-auto text-center text-2xl font-light"
                    text "A place to share your knowledge"
                ]
            ]
        ]

    view