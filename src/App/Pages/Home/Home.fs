module SubtleConduit.Pages.Home

open Sutil
open SubtleConduit.Components
open Sutil.DOM

let private articleFilter = Store.make None

let HomePage model dispatch =
    let view =
        Html.div [
            Html.div [
                Attr.classes [
                    "bg-conduit-green"
                    "w-full"
                    "p-8"
                ]
                Html.div [
                    Attr.classes [
                        "container"
                        "mx-auto"
                        "text-white"
                    ]
                    Html.h1 [
                        Attr.classes [
                            "mx-auto"
                            "text-center"
                            "text-6xl"
                            "font-mono"
                            "font-bold"
                            "text-shadow-lg"
                            "mb-2"
                            "cursor-default"
                        ]
                        text "conduit"
                    ]
                    Html.h2 [
                        Attr.classes [
                            "mx-auto"
                            "text-center"
                            "text-2xl"
                            "font-light"
                            "cursor-default"
                        ]
                        text "A place to share your knowledge"
                    ]
                ]
            ]
            Html.div [
                Attr.classes [
                    "container"
                    "mx-auto"
                    "mt-8"
                    "flex"
                    "justify-between"
                ]
                Bind.el (
                    articleFilter,
                    (fun af ->
                        let setter = Store.set articleFilter

                        fragment [
                            Feed.Feed model dispatch af setter
                            Tags.Tags af setter
                        ])
                )
            ]
        ]

    view
