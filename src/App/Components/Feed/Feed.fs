module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open Tailwind
open SubtleConduit.Services
open Sutil.DOM

let Feed () =
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

        Html.div [
            disposeOnUnmount [
                articles
            ]

            Attr.classes [
                tw.``flex-auto``
                tw.``mr-6``
            ]
            Html.div [
                Attr.classes [
                    tw.``border-b``
                    tw.``box-border``
                ]
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
                            text a.title
                        ]
                )
            ]
        ]

    view // col-9
