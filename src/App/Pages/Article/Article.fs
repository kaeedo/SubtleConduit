module SubtleConduit.Pages.Article

// open Fable.Core
// open Fable.Core.JsInterop
// open Feliz
// open Feliz.UseElmish
// open Elmish

open System
open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open Tailwind
open Sutil.DOM
open SubtleConduit.Router
open SubtleConduit.Utilities
open Sutil.Attr
open Fable.Core.JsInterop

type ArticleState =
    { Title: string
      Description: string
      Body: string
      CreatedAt: DateTime
      Tags: string list
      Favorited: bool
      FavoritesCount: int
      Author: Profile }

type ArticleMsg =
    | GetArticle of string
    | Set of Article
    | Error of string

let init slug () =
    { ArticleState.Title = ""
      Description = ""
      Body = ""
      CreatedAt = new DateTime()
      Tags = []
      Favorited = false
      FavoritesCount = 0
      Author =
        { Profile.Username = ""
          Bio = ""
          Image = ""
          Following = false } },
    Cmd.ofMsg <| GetArticle slug

let mapResultToArticleState (result: Article) =
    { ArticleState.Title = result.Title
      Description = result.Description
      Body = result.Body
      CreatedAt = result.CreatedAt
      Tags = result.TagList
      Favorited = result.Favorited
      FavoritesCount = result.FavoritesCount
      Author = result.Author }

let update msg state =
    match msg with
    | GetArticle slug -> state, Cmd.OfPromise.either getArticle slug (fun r -> Set r) (fun _ -> Error "error")
    | Set result ->
        let newState = mapResultToArticleState result
        newState, Cmd.none
    | Error e -> state, Cmd.none // TODO actually handle this


let ArticlePage (slug: string) =
    let state, dispatch =
        Store.makeElmish (init slug) update ignore ()

    let tags = state .> (fun s -> s.Tags)
    let heartIcon = importDefault "../../Images/heart.svg"

    let articleInfo =
        Html.div [
            Attr.classes [
                tw.flex
                tw.``w-max``
            ]
            Html.img [
                Attr.classes [
                    tw.``w-8``
                    tw.``h-8``
                    tw.``rounded-3xl``
                    tw.``self-center``
                    tw.``mr-1``
                ]
                Bind.el (state, (fun s -> Attr.src s.Author.Image))
            ]
            Html.div [
                Attr.classes [
                    tw.flex
                    tw.``flex-col``
                ]
                Html.a [
                    Attr.classes [
                        tw.``text-sm``
                        tw.``leading-none``
                        tw.``font-bold``
                        tw.``hover:underline``
                    ]
                    Attr.href $"javascript:void(0)"

                    Bind.el (
                        state,
                        (fun s ->
                            fragment [
                                onClick
                                    (fun _ ->
                                        Router.navigate $"profile/{s.Author.Username}" (Some(s.Author.Username :> obj)))
                                    []
                                text s.Author.Username
                            ])
                    )
                ]
                Html.span [
                    Attr.classes [
                        tw.``text-xs``
                        tw.``text-gray-400``
                    ]
                    Bind.el (state, (fun s -> text (s.CreatedAt |> formatDateUS "MMMM dd, yyyy")))
                ]
            ]
            Html.div [
                Attr.classes [
                    tw.flex
                ]
                Html.div [
                    Attr.classes [
                        tw.``self-center``
                        tw.``pl-1``
                        tw.``pr-2``
                        tw.``ml-6``
                        tw.``rounded-sm``
                        tw.``text-base``
                        tw.``leading-6``
                        tw.``h-7``
                        tw.border
                        tw.rounded
                        tw.``text-gray-400``
                        tw.``border-gray-300``
                        tw.``hover:bg-gray-300``
                        tw.``hover:text-white``
                    ]
                    Bind.el (state, (fun s -> text $"+ Follow {s.Author.Username}"))
                ]
                Html.div [
                    Attr.classes [
                        tw.flex
                        tw.``self-center``
                        tw.``pl-1``
                        tw.``pr-2``
                        tw.``ml-2``
                        tw.``rounded-sm``
                        tw.``text-base``
                        tw.``leading-6``
                        tw.``h-7``
                        tw.``w-max``
                        tw.border
                        tw.rounded
                        tw.``text-conduit-green``
                        tw.``border-conduit-green``
                        tw.``hover:bg-conduit-green``
                        tw.``hover:text-white``
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
                    Bind.el (state, (fun s -> text $"Favorite Article ({s.FavoritesCount})"))
                ]
            ]
        ]

    let view =
        Html.div [
            disposeOnUnmount [
                state
            ]
            Html.div [
                Attr.classes [
                    tw.``bg-gray-800``
                    tw.``w-screen``
                    tw.``p-8``
                    tw.``text-white``
                ]
                Html.div [
                    Attr.classes [
                        tw.``mx-auto``
                        tw.container
                        tw.``mb-8``
                    ]
                    Html.h1 [
                        Attr.classes [
                            tw.``text-4xl``
                        ]
                        Bind.el (state, (fun s -> text s.Title))
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mx-auto``
                        tw.container
                    ]
                    articleInfo
                ]
            ]
            Html.div [
                Attr.classes [
                    tw.``mx-auto``
                    tw.container
                ]
                Html.div [
                    Attr.classes [
                        tw.``my-8``
                    ]
                    Bind.el (state, (fun s -> text s.Body))
                ]
                Html.ul [
                    Bind.each (
                        tags,
                        fun t ->
                            Html.li [
                                Attr.classes [
                                    tw.``inline-flex``
                                ]
                                Html.span [
                                    Attr.classes [
                                        tw.``px-2``
                                        tw.``py-1``
                                        tw.``rounded-xl``
                                        tw.``text-gray-300``
                                        tw.``mr-1``
                                        tw.``mb-1``
                                        tw.``text-xs``
                                        tw.border
                                        tw.rounded
                                        tw.``border-gray-300``
                                    ]

                                    text t
                                ]
                            ]
                    )
                ]
                Html.div [
                    Attr.classes [
                        tw.``my-8``
                        tw.``border-b-2``
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.container
                        tw.flex
                        tw.``justify-center``
                    ]
                    articleInfo
                ]
            ]
        ]

    view
