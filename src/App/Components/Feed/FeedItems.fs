module SubtleConduit.Components.FeedItems

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

open Tailwind
open SubtleConduit.Types
open SubtleConduit.Router
open SubtleConduit.Services.Api
open Sutil.DOM
open Fable.Core.JsInterop

let FeedItems articles favoriteArticle setArticleFilter =
    let heartIcon = importDefault "../../Images/heart.svg"

    let view =
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
                                                if a.Favorited then
                                                    tw.``bg-conduit-green``
                                                    tw.``text-white``
                                            ]
                                            Html.img [
                                                Attr.classes [
                                                    tw.``w-4``
                                                    tw.``mr-1``
                                                ]
                                                Attr.src heartIcon
                                            ]
                                            onClick (favoriteArticle a.Slug a.Favorited) []
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
                                                                <| Some(ArticleApi.ArticleFilter.Tag(tag.ToString())))
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

    view
