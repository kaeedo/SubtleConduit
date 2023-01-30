module SubtleConduit.Components.FeedItems

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

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
                                Attr.classes [ "border-t"; "py-6" ]
                                Html.div [
                                    Html.div [
                                        Attr.classes [ "flex"; "justify-between" ]
                                        Html.div [
                                            Attr.classes [ "flex"; "mb-4" ]
                                            Html.img [
                                                Attr.classes [
                                                    "w-8"
                                                    "h-8"
                                                    "rounded-3xl"
                                                    "self-center"
                                                ]
                                                Attr.src a.Author.Image
                                            ]
                                            Html.div [
                                                Attr.classes [
                                                    "flex"
                                                    "flex-col"
                                                    "ml-2"
                                                ]
                                                Html.a [
                                                    Attr.classes [
                                                        "text-conduit-green"
                                                        "font-semibold"
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
                                                    Attr.classes [ "text-xs"; "text-gray-400" ]
                                                    text (a.CreatedAt |> formatDateUS "MMMM dd, yyyy")
                                                ]
                                            ]
                                        ]
                                        Html.button [
                                            Attr.classes [
                                                "border"
                                                "rounded"
                                                "border-conduit-green"
                                                "hover:bg-conduit-green"
                                                "h-8"
                                                "flex"
                                                "px-2"
                                                "hover:text-white"
                                                "text-conduit-green"
                                                "text-xs"
                                                "items-center"
                                                if a.Favorited then
                                                    "bg-conduit-green"
                                                    "text-white"
                                            ]
                                            Html.img [
                                                Attr.classes [ "w-4"; "mr-1" ]
                                                Attr.src heartIcon
                                            ]
                                            onClick (favoriteArticle a.Slug a.Favorited) []
                                            text (a.FavoritesCount.ToString())
                                        ]
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            "mb-4"
                                            "flex"
                                            "flex-col"
                                        ]
                                        Html.a [
                                            Attr.classes [
                                                "text-2xl"
                                                "font-semibold"
                                                "mb-1"
                                            ]
                                            Attr.href "#"
                                            text a.Title
                                        ]
                                        Html.a [
                                            Attr.classes [ "text-sm"; "text-gray-400" ]
                                            Attr.href "#"
                                            text a.Description
                                        ]
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            "flex"
                                            "justify-between"
                                            "items-baseline"
                                        ]
                                        Html.a [
                                            Attr.classes [ "text-xs"; "text-gray-300" ]
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
                                                    Attr.classes [ "inline-flex" ]
                                                    Html.span [
                                                        Attr.classes [
                                                            "px-2"
                                                            "py-1"
                                                            "rounded-xl"
                                                            "cursor-pointer"
                                                            "text-gray-300"
                                                            "mr-1"
                                                            "mb-1"
                                                            "text-xs"
                                                            "border"
                                                            "rounded"
                                                            "border-gray-300"
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
