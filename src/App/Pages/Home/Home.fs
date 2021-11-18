module SubtleConduit.Pages.Home

open Sutil
open Tailwind
open SubtleConduit.Components
open SubtleConduit.Services.Api
open Sutil.DOM

let private articleFilter =
    Store.make<ArticleApi.ArticleFilter option> None

let HomePage dispatch =
    let view =
        Html.div [
            Html.div [
                Attr.classes [
                    tw.``bg-conduit-green``
                    tw.``w-screen``
                    tw.``p-8``
                ]
                Html.div [
                    Attr.classes [
                        tw.container
                        tw.``mx-auto``
                        tw.``text-white``
                    ]
                    Html.h1 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-6xl``
                            tw.``font-mono``
                            tw.``font-bold``
                            tw.``text-shadow-lg``
                            tw.``mb-2``
                            tw.``cursor-default``
                        ]
                        text "conduit"
                    ]
                    Html.h2 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-2xl``
                            tw.``font-light``
                            tw.``cursor-default``
                        ]
                        text "A place to share your knowledge"
                    ]
                ]
            ]
            Html.div [
                Attr.classes [
                    tw.container
                    tw.``mx-auto``
                    tw.``mt-8``
                    tw.flex
                    tw.``justify-between``
                ]
                Bind.el (
                    articleFilter,
                    (fun af ->
                        let setter = Store.set articleFilter

                        fragment [
                            Feed.Feed dispatch af setter
                            Tags.Tags af setter
                        ])
                )
            ]
        ]

    view
