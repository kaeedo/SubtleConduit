module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Types

open Fable.Core

open Tailwind
open SubtleConduit.Services
open Sutil.DOM
open System
open Fable.Core.JsInterop

let Feed () =
    let heartIcon = importDefault "../../Images/heart.svg"

    let view =
        let articles: IStore<Api.Articles> =
            Store.make
            <| Api.Articles("{\"articles\":[], \"articlesCount\":0}")

        let articleList =
            articles .> fun a -> a.articles |> List.ofArray

        promise {
            let! articlesFromApi = Api.getArticles ()
            articles <~ (articlesFromApi) // TODO Refactor to use array once Sutil supports it
        }
        |> Promise.start

        let formateDate date =
            let formatDateUS =
                Date.Format.localFormat Date.Local.englishUS "MMMM dd, yyyy"

            formatDateUS <| DateTime.Parse(date)

        Html.div [
            disposeOnUnmount [
                articles
            ]

            Attr.classes [
                tw.``flex-auto``
                tw.``mr-6``
            ]
            Html.div [
                Html.div [
                    Attr.classes [
                        tw.``text-conduit-green``
                        tw.``w-max``
                        tw.``py-2``
                        tw.``px-4``
                        tw.``border-b-2``
                        tw.``box-content``
                        tw.``border-conduit-green``
                    ]
                    text "Global Feed"
                ]
            ]
            Html.ul [
                Bind.each (
                    articleList,
                    fun a ->
                        Html.li [
                            Attr.classes [
                                tw.``border-t``
                                tw.``py-6``
                            ]
                            Html.div [
                                Html.div [
                                    Attr.classes [
                                        tw.flex
                                        tw.``justify-between``
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            tw.flex
                                            tw.``mb-4``
                                        ]
                                        Html.img [
                                            Attr.classes [
                                                tw.``w-8``
                                                tw.``h-8``
                                                tw.``rounded-3xl``
                                                tw.``self-center``
                                            ]
                                            Attr.src a.author.image
                                        ]
                                        Html.div [
                                            Attr.classes [
                                                tw.flex
                                                tw.``flex-col``
                                                tw.``ml-2``
                                            ]
                                            Html.span [
                                                Attr.classes [
                                                    tw.``text-conduit-green``
                                                    tw.``font-semibold``
                                                ]
                                                text a.author.username
                                            ]
                                            Html.span [
                                                Attr.classes [
                                                    tw.``text-xs``
                                                    tw.``text-gray-400``
                                                ]
                                                text (formateDate a.createdAt)
                                            ]
                                        ]
                                    ]
                                    Html.button [
                                        Attr.classes [
                                            tw.border
                                            tw.rounded
                                            tw.``border-conduit-green``
                                            tw.``hover:bg-conduit-green``
                                            tw.``h-8``
                                            tw.flex
                                            tw.``px-2``
                                            tw.``hover:text-white``
                                            tw.``text-conduit-green``
                                            tw.``text-xs``
                                            tw.``items-center``
                                        ]
                                        Html.img [
                                            Attr.classes [
                                                tw.``w-4``
                                                tw.``mr-1``
                                                // tw.``hover:text-white``
                                                // tw.``text-conduit-green``
                                                // tw.``fill-current``
                                                ]
                                            Attr.src heartIcon
                                        ]
                                        text (a.favoritesCount.ToString())
                                    ]
                                ]
                                Html.div [
                                    Attr.classes [
                                        tw.``mb-4``
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            tw.``text-2xl``
                                            tw.``font-semibold``
                                            tw.``mb-1``
                                        ]
                                        text a.title
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            tw.``text-sm``
                                            tw.``text-gray-400``
                                        ]
                                        text a.description
                                    ]
                                ]
                                Html.div [
                                    Attr.classes [
                                        tw.flex
                                        tw.``justify-between``
                                        tw.``items-baseline``
                                    ]
                                    Html.span [
                                        Attr.classes [
                                            tw.``text-xs``
                                            tw.``text-gray-300``
                                        ]
                                        text "Read more..."
                                    ]
                                    Html.ul [
                                        for tag in a.tagList do
                                            let tag: string = tag :?> string

                                            Html.li [
                                                Attr.classes [
                                                    tw.``inline-flex``
                                                ]
                                                Html.span [
                                                    Attr.classes [
                                                        tw.``px-2``
                                                        tw.``py-1``
                                                        tw.``rounded-xl``
                                                        tw.``cursor-pointer``
                                                        tw.``text-gray-300``
                                                        tw.``mr-1``
                                                        tw.``mb-1``
                                                        tw.``text-xs``
                                                        tw.border
                                                        tw.rounded
                                                        tw.``border-gray-300``
                                                    ]
                                                    text tag
                                                ]
                                            ]
                                    ]
                                ]
                            ]
                        ]
                )
            ]
        ]

    view
