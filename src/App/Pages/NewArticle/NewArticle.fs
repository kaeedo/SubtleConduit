module SubtleConduit.Pages.NewArticle

open System
open Sutil
open SubtleConduit.Elmish
open Tailwind
open Sutil.DOM
open Sutil.Attr


let NewArticlePage (model: State) =
    let view =
        let user = model.User.Value
        let token = user.Token

        let title = Store.make String.Empty
        let description = Store.make String.Empty
        let body = Store.make String.Empty
        let tags = Store.make []

        Html.div [
            disposeOnUnmount [
                title
                description
                body
                tags
            ]

            Attr.classes [
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-col``
                tw.``items-center``
            ]
            Html.form [
                Attr.classes [
                    tw.``w-9/12``
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                            tw.``h-16``
                            tw.``text-xl``
                        ]
                        type' "text"
                        Bind.attr ("value", title)
                        Attr.placeholder "Article Title"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
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
                        type' "text"
                        Bind.attr ("value", description)
                        Attr.placeholder "What's this article about?"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.textarea [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        Attr.rows 8
                        Bind.attr ("value", body)
                        Attr.placeholder "Write your article (in markdown)"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
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
                        type' "text"
                        Bind.attr ("value", tags)
                        Attr.placeholder "Enter tags"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.flex
                        tw.``justify-end``
                    ]
                    Html.button [
                        Attr.classes [
                            tw.flex
                            tw.``bg-conduit-green``
                            tw.``hover:bg-conduit-green-500``
                            tw.``text-white``
                            tw.rounded
                            tw.``px-6``
                            tw.``py-3``
                            tw.``text-xl``
                        ]
                        //type' "submit"
                        text "Publish Article"
                    ]
                ]
            ]
        ]

    view
