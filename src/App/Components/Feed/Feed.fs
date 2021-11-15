module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

open Tailwind
open SubtleConduit.Types
open SubtleConduit.Services
open SubtleConduit.Router
open Sutil.DOM
open System
open Fable.Core.JsInterop
open SubtleConduit.Elmish

let private pageSize = 10
let private offset = Store.make 0

let private articles = ObservablePromise<Articles>()

let private getArticles pageSize newOffset filter =
    articles.Run
    <| promise {
        let! articlesFromApi = Api.getArticles pageSize newOffset filter
        // TODO handle error case
        offset <~ newOffset
        return articlesFromApi
       }

let private currentPage = offset .> fun o -> (o / pageSize) + 1

let private pageNumbers = // TODO refactor to array
    let total =
        articles
        .> (function
        | Result a ->
            Math.Ceiling(float a.ArticlesCount / float pageSize)
            |> int
        | _ -> 0)

    Store.zip currentPage total
    .> (fun (current, total) -> getPagesToDisplay current total)

let Feed (dispatch: Dispatch<Message>) (articleFilter: Api.ArticleFilter option) (setArticleFilter) =
    let heartIcon = importDefault "../../Images/heart.svg"

    let view =
        Html.div [
            onMount (fun _ -> getArticles pageSize 0 articleFilter) [ Once ]
            disposeOnUnmount [
                offset
            ]

            Attr.classes [
                tw.``flex-auto``
                tw.``mr-6``
            ]

            Html.div [
                Attr.classes [
                    tw.flex
                ]
                Html.div [
                    Attr.classes [
                        tw.``text-conduit-green``
                        tw.``w-max``
                        tw.``py-2``
                        tw.``px-4``
                        tw.``cursor-pointer``

                        if articleFilter.IsNone then
                            tw.``border-b-2``
                            tw.``border-conduit-green``
                    ]
                    onClick (fun _ -> setArticleFilter None) []
                    text "Global Feed"
                ]
                match articleFilter with
                | Some (Api.Tag t) ->
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
            Html.ul [
                Bind.el (
                    articles,
                    function
                    | Waiting -> text "Loading"
                    | Error e -> text $"Error occured: {e.Message}"
                    | Result art ->
                        fragment [
                            for a in art.Articles do
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
                                                    Attr.src a.Author.Image
                                                ]
                                                Html.div [
                                                    Attr.classes [
                                                        tw.flex
                                                        tw.``flex-col``
                                                        tw.``ml-2``
                                                    ]
                                                    Html.a [
                                                        Attr.classes [
                                                            tw.``text-conduit-green``
                                                            tw.``font-semibold``
                                                        ]
                                                        Attr.href $"javascript:void(0)"
                                                        onClick
                                                            (fun _ ->
                                                                Router.navigate
                                                                    $"profile/{a.Author.Username}"
                                                                    (Some(a.Author.Username :> obj)))
                                                            []
                                                        text a.Author.Username
                                                    ]
                                                    Html.span [
                                                        Attr.classes [
                                                            tw.``text-xs``
                                                            tw.``text-gray-400``
                                                        ]
                                                        text (a.CreatedAt |> formatDateUS "MMMM dd, yyyy")
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
                                                text (a.FavoritesCount.ToString())
                                            ]
                                        ]
                                        Html.div [
                                            Attr.classes [
                                                tw.``mb-4``
                                                tw.flex
                                                tw.``flex-col``
                                            ]
                                            Html.a [
                                                Attr.classes [
                                                    tw.``text-2xl``
                                                    tw.``font-semibold``
                                                    tw.``mb-1``
                                                ]
                                                Attr.href "#"
                                                text a.Title
                                            ]
                                            Html.a [
                                                Attr.classes [
                                                    tw.``text-sm``
                                                    tw.``text-gray-400``
                                                ]
                                                Attr.href "#"
                                                text a.Description
                                            ]
                                        ]
                                        Html.div [
                                            Attr.classes [
                                                tw.flex
                                                tw.``justify-between``
                                                tw.``items-baseline``
                                            ]
                                            Html.a [
                                                Attr.classes [
                                                    tw.``text-xs``
                                                    tw.``text-gray-300``
                                                ]
                                                Attr.href $"javascript:void(0)"
                                                onClick
                                                    (fun _ ->
                                                        Router.navigate $"article/{a.Slug}"
                                                        <| Some(a.Slug :> obj))
                                                    []
                                                text "Read more..."
                                            ]
                                            Html.ul [
                                                for tag in a.TagList do
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
                                                            onClick
                                                                (fun _ ->
                                                                    setArticleFilter
                                                                    <| Some(Api.ArticleFilter.Tag(tag.ToString())))
                                                                []
                                                            text (tag.ToString())
                                                        ]
                                                    ]
                                            ]
                                        ]
                                    ]
                                ]
                        ]
                )
            ]
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
                                            getArticles pageSize (offset.Value - pageSize) articleFilter)
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
                                    onClick (fun _ -> getArticles pageSize (int pn * pageSize - 10) articleFilter) []
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
                                            getArticles pageSize (offset.Value + pageSize) articleFilter)

                                    []

                                text ">"
                            ]
                        ]
                )
            ]
        ]

    view
