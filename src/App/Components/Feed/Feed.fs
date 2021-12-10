module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

open Tailwind
open SubtleConduit.Types
open SubtleConduit.Services.Api
open Sutil.DOM
open System
open SubtleConduit.Elmish
open SubtleConduit.Components.FeedItems

let Feed
    (model: State)
    (dispatch: Dispatch<Message>)
    (articleFilter: ArticleApi.ArticleFilter option)
    (setArticleFilter)
    =
    let pageSize = 10
    let offset = Store.make 0

    let isFeedSelected = Store.make false

    let articles = ObservablePromise<Articles>()

    let getArticles user pageSize newOffset filter showFeed =
        articles.Run
        <| promise {
            let! articlesFromApi = ArticleApi.getArticles user pageSize newOffset filter showFeed
            // TODO handle error case
            offset <~ newOffset
            return articlesFromApi
           }

    let currentPage = offset .> fun o -> (o / pageSize) + 1

    let pageNumbers = // TODO refactor to array
        let total =
            articles
            .> (function
            | Result a ->
                Math.Ceiling(float a.ArticlesCount / float pageSize)
                |> int
            | _ -> 0)

        Store.zip currentPage total
        .> (fun (current, total) -> getPagesToDisplay current total)


    let favoriteArticle slug isFavorited _ =
        match model.User with
        | None -> ()
        | Some u ->
            promise {
                let! _ = ArticleApi.favoriteArticle slug isFavorited u.Token
                // TODO Ask how to update single value in observable promise
                getArticles model.User pageSize offset.Value articleFilter isFeedSelected.Value
                return ()
            }
            |> Promise.start

    let tabs () =
        Html.div [
            Attr.classes [
                tw.flex
            ]
            match model.User with
            | None -> ()
            | Some u ->
                Html.div [
                    Bind.toggleClass (isFeedSelected, $"{tw.``border-b-2``} {tw.``border-conduit-green``}")
                    Attr.classes [
                        tw.``text-conduit-green``
                        tw.``w-max``
                        tw.``py-2``
                        tw.``px-4``
                        tw.``box-content``
                        tw.``cursor-pointer``
                    ]
                    onClick
                        (fun _ ->
                            getArticles model.User pageSize offset.Value articleFilter true
                            isFeedSelected <~ true)
                        []
                    text "Your Feed"
                ]
            Html.div [
                Bind.toggleClass (
                    (isFeedSelected
                     .> fun f -> not f && articleFilter.IsNone),
                    $"{tw.``border-b-2``} {tw.``border-conduit-green``}"
                )
                Attr.classes [
                    tw.``text-conduit-green``
                    tw.``w-max``
                    tw.``py-2``
                    tw.``px-4``
                    tw.``cursor-pointer``
                ]
                onClick
                    (fun _ ->
                        isFeedSelected <~ false
                        setArticleFilter None)
                    []
                text "Global Feed"
            ]
            match articleFilter with
            | Some (ArticleApi.Tag t) ->
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
                    text t
                ]
            | _ -> ()
        ]

    let view =
        Html.div [
            onMount (fun _ -> getArticles model.User pageSize 0 articleFilter false) [ Once ]
            disposeOnUnmount [
                offset
                isFeedSelected
            ]

            Attr.classes [
                tw.``flex-auto``
                tw.``mr-6``
            ]
            tabs ()
            FeedItems articles favoriteArticle setArticleFilter
            Html.ul [
                Attr.classes [
                    tw.flex
                    tw.``justify-center``
                ]
                Bind.el (
                    pageNumbers,
                    fun pns ->
                        let lastPage = pns.[pns.Length - 1]

                        fragment [
                            Html.li [
                                Attr.classes [
                                    tw.``cursor-pointer``
                                    tw.``px-3``
                                    tw.``py-2``
                                    tw.border
                                    tw.``border-gray-300``
                                    tw.``border-r-0``
                                    tw.``rounded-l-sm``
                                ]
                                onClick
                                    (fun _ ->
                                        if offset.Value = 0 then
                                            ()
                                        else
                                            getArticles
                                                model.User
                                                pageSize
                                                (offset.Value - pageSize)
                                                articleFilter
                                                isFeedSelected.Value)
                                    []
                                text "<"
                            ]

                            for pn in pns do
                                Html.li [
                                    Bind.toggleClass (
                                        (currentPage .> fun cp -> (cp.ToString()) = pn),
                                        tw.``bg-conduit-green`` + " " + tw.``text-white``,
                                        tw.``hover:bg-gray-100``
                                    )
                                    Attr.classes [
                                        tw.``cursor-pointer``
                                        tw.``px-3``
                                        tw.``py-2``
                                        tw.border
                                        tw.``border-gray-300``
                                        tw.``border-r-0``
                                    ]
                                    onClick
                                        (fun _ ->
                                            getArticles
                                                model.User
                                                pageSize
                                                (int pn * pageSize - 10)
                                                articleFilter
                                                isFeedSelected.Value)
                                        []
                                    text pn
                                ]

                            Html.li [
                                Attr.classes [
                                    tw.``cursor-pointer``
                                    tw.``px-3``
                                    tw.``py-2``
                                    tw.border
                                    tw.``border-gray-300``
                                    tw.``rounded-r-sm``
                                ]
                                onClick
                                    (fun _ ->
                                        if (offset.Value / pageSize + 1) = int lastPage then
                                            ()
                                        else
                                            getArticles
                                                model.User
                                                pageSize
                                                (offset.Value + pageSize)
                                                articleFilter
                                                isFeedSelected.Value)

                                    []

                                text ">"
                            ]
                        ]
                )
            ]
        ]

    view
