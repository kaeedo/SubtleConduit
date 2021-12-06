module SubtleConduit.Pages.NewArticle

open System
open Sutil
open SubtleConduit.Types
open SubtleConduit.Elmish
open Tailwind
open Sutil.DOM
open Sutil.Attr
open SubtleConduit.Services.Api
open SubtleConduit.Router

let NewArticlePage (model: State) =
    let view =
        let user = model.User.Value
        let token = user.Token

        let title = Store.make String.Empty
        let description = Store.make String.Empty
        let body = Store.make String.Empty
        let tag = Store.make ""
        let tags = Store.make []

        let addTag _ =
            tags <~= (fun ts -> [ tag.Value ] @ ts)
            tag <~ String.Empty

        let removeTag tag _ =
            tags
            <~= (fun ts -> ts |> List.filter (fun t -> t <> tag))

        let onSubmit (e: Browser.Types.Event) =
            e.preventDefault ()

            let article =
                { UpsertArticle.Title = title.Value
                  Description = description.Value
                  Body = body.Value
                  TagList = tags.Value
                  Token = token }

            promise {
                let! _ = ArticleApi.createArticle article
                Router.navigate "home" None
            }
            |> ignore

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
                on "submit" (onSubmit) []
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
                        tw.relative
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
                        Bind.attr ("value", tag)
                        onKeyboard
                            "keypress"
                            (fun k ->
                                if k.key = "Enter" then
                                    k.preventDefault ()
                                    addTag ())
                            []
                        Attr.placeholder "Enter tag"
                    ]
                    Html.div [
                        Attr.classes [
                            tw.absolute
                            tw.``cursor-pointer``
                            tw.``top-2``
                            tw.``right-2``
                            tw.``bg-gray-500``
                            tw.``hover:bg-gray-600``
                            tw.``text-white``
                            tw.rounded
                            tw.``px-3``
                            tw.``pt-px``
                            tw.``pb-1.5``
                            tw.``text-xl``
                        ]
                        text "+"
                        onClick (addTag) []
                    ]
                ]
                Html.ul [
                    Attr.classes [
                        tw.flex
                    ]
                    Bind.each (
                        tags,
                        (fun t ->
                            Html.li [
                                Attr.classes [
                                    tw.``px-2``
                                    tw.``py-1``
                                    tw.``rounded-xl``
                                    tw.``cursor-pointer``
                                    tw.``bg-gray-500``
                                    tw.``hover:bg-gray-600``
                                    tw.``text-white``
                                    tw.``mr-1``
                                    tw.``mb-1``
                                    tw.``text-xs``
                                ]
                                Html.span [
                                    Attr.classes [
                                        tw.``mr-1``
                                        tw.``font-bold``
                                    ]
                                    text "X"
                                ]
                                Html.span [ text t ]
                                onClick (removeTag t) []
                            ])
                    )
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
                        type' "submit"
                        text "Publish Article"
                    ]
                ]
            ]
        ]

    view
