module SubtleConduit.Pages.Article

open System
open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open SubtleConduit.Elmish
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
    | UpdateFavorite of bool * int
    | UpdateFollowing of bool
    | Set of Article
    | Error of string

let private init slug () =
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

let private mapResultToArticleState (result: Article) =
    { ArticleState.Title = result.Title
      Description = result.Description
      Body = result.Body
      CreatedAt = result.CreatedAt
      Tags = result.TagList
      Favorited = result.Favorited
      FavoritesCount = result.FavoritesCount
      Author = result.Author }

let private update (user: User option) msg state =
    let token = user |> Option.map (fun u -> u.Token)

    match msg with
    | GetArticle slug ->
        state,
        Cmd.OfPromise.either
            ArticleApi.getArticle
            (slug, (token |> Option.defaultValue ""))
            (fun r -> Set r)
            (fun _ -> Error "error")
    | Set result ->
        let newState = mapResultToArticleState result
        newState, Cmd.none
    | UpdateFavorite (isFavorited, favoriteCount) ->
        let newState =
            { state with
                Favorited = isFavorited
                FavoritesCount = favoriteCount }

        newState, Cmd.none
    | UpdateFollowing isFollowing ->
        let newState =
            { state with Author = { state.Author with Following = isFollowing } }

        newState, Cmd.none
    | Error e -> state, Cmd.none // TODO actually handle this


let ArticlePage (model: State) (slug: string) =
    let state, dispatch =
        Store.makeElmish (init slug) (update model.User) ignore ()

    let tags = state .> (fun s -> s.Tags)
    //let heartIcon = importDefault "../../Images/heart.svg"

    let favoriteArticle slug isFavorited _ =
        match model.User with
        | None -> ()
        | Some u ->
            promise {
                let! article = ArticleApi.favoriteArticle slug isFavorited u.Token

                dispatch
                <| UpdateFavorite(article.Favorited, article.FavoritesCount)

                return ()
            }
            |> Promise.start

    let followAuthor author isFollowing _ =
        match model.User with
        | None -> ()
        | Some u ->
            promise {
                let! profile = ProfileApi.setFollow (u.Token, author, not isFollowing)

                dispatch <| UpdateFollowing profile.Following

                return ()
            }
            |> Promise.start

    let otherArticleActions =
        Html.div [
            Attr.classes [
                "flex"
            ]
            Html.div [
                Bind.toggleClass (
                    (state .> fun s -> s.Author.Following),
                    $"bg-gray-300 text-white",
                    "text-gray-400"
                )
                Attr.classes [
                    "cursor-pointer"
                    "self-center"
                    "pl-1"
                    "pr-2"
                    "ml-6"
                    "rounded-sm"
                    "text-base"
                    "leading-6"
                    "h-7"
                    "border"
                    "rounded"
                    "border-gray-300"
                    "hover:bg-gray-300"
                    "hover:text-white"
                ]
                Bind.el (
                    state,
                    (fun s ->
                        fragment [
                            if s.Author.Following then
                                text $"- Unfollow {s.Author.Username}"
                            else
                                text $"+ Follow {s.Author.Username}"
                            onClick (followAuthor s.Author.Username s.Author.Following) []
                        ])
                )
                onClick (ignore) []
            ]
            Html.div [
                Bind.toggleClass (
                    (state .> fun s -> s.Favorited),
                    "bg-conduit-green text-white",
                    "hover:bg-conduit-green hover:text-white"
                )
                Attr.classes [
                    "cursor-pointer"
                    "flex"
                    "self-center"
                    "pl-1"
                    "pr-2"
                    "ml-2"
                    "rounded-sm"
                    "text-base"
                    "leading-6"
                    "h-7"
                    "w-max"
                    "border"
                    "rounded"
                    "text-conduit-green"
                    "border-conduit-green"
                ]
                Html.img [
                    Attr.classes [
                        "w-4"
                        "mr-1"
                    ]
                    //Attr.src heartIcon
                ]
                Bind.el (
                    state,
                    (fun s ->
                        fragment [
                            if s.Favorited then
                                text $"Unfavorite Article ({s.FavoritesCount})"
                            else
                                text $"Favorite Article ({s.FavoritesCount})"
                            onClick (favoriteArticle slug s.Favorited) []
                        ])
                )
            ]
        ]

    let myArticleActions token =
        Html.div [
            Attr.classes [
                "flex"
            ]
            Html.div [
                Attr.classes [
                    "cursor-pointer"
                    "self-center"
                    "pl-1"
                    "pr-2"
                    "ml-6"
                    "rounded-sm"
                    "text-base"
                    "leading-6"
                    "h-7"
                    "border"
                    "rounded"
                    "text-gray-400"
                    "border-gray-300"
                    "hover:bg-gray-300"
                    "hover:text-white"
                ]
                text "Edit article"
                onClick (fun _ -> Router.navigate $"editor/{slug}" (Some(slug :> obj))) []
            ]
            Html.div [
                Attr.classes [
                    "cursor-pointer"
                    "flex"
                    "self-center"
                    "pl-1"
                    "pr-2"
                    "ml-2"
                    "rounded-sm"
                    "text-base"
                    "leading-6"
                    "h-7"
                    "w-max"
                    "border"
                    "rounded"
                    "text-red-500"
                    "border-red-500"
                    "hover:bg-red-500"
                    "hover:text-white"
                ]
                Html.img [
                    Attr.classes [
                        "w-4"
                        "mr-1"
                    ]
                    //Attr.src heartIcon
                ]
                text "Delete article"
                onClick
                    (fun _ ->
                        promise {
                            let! _ = ArticleApi.deleteArticle slug token
                            Router.navigate "home" None
                        }
                        |> ignore)
                    []
            ]
        ]

    let articleInfo =
        Html.div [
            Attr.classes [
                "flex"
                "w-max"
            ]
            Html.img [
                Attr.classes [
                    "w-8"
                    "h-8"
                    "rounded-3xl"
                    "self-center"
                    "mr-1"
                ]
                Bind.el (state, (fun s -> Attr.src s.Author.Image))
            ]
            Html.div [
                Attr.classes [
                    "flex"
                    "flex-col"
                ]
                Html.a [
                    Attr.classes [
                        "text-sm"
                        "leading-none"
                        "font-bold"
                        "hover:underline"
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
                        "text-xs"
                        "text-gray-400"
                    ]
                    Bind.el (state, (fun s -> text (s.CreatedAt |> formatDateUS "MMMM dd, yyyy")))
                ]
            ]
            Bind.el (
                state,
                fun s ->
                    match model.User with
                    | Some u when u.Username = s.Author.Username -> myArticleActions u.Token
                    | _ -> otherArticleActions
            )

            ]

    let view =
        Html.div [
            disposeOnUnmount [
                state
            ]
            Html.div [
                Attr.classes [
                    "bg-gray-800"
                    "w-full"
                    "p-8"
                    "text-white"
                ]
                Html.div [
                    Attr.classes [
                        "mx-auto"
                        "container"
                        "mb-8"
                    ]
                    Html.h1 [
                        Attr.classes [
                            "text-4xl"
                        ]
                        Bind.el (state, (fun s -> text s.Title))
                    ]
                ]
                Html.div [
                    Attr.classes [
                        "mx-auto"
                        "container"
                    ]
                    articleInfo
                ]
            ]
            Html.div [
                Attr.classes [
                    "mx-auto"
                    "container"
                ]
                Html.div [
                    Attr.classes [
                        "my-8"
                    ]
                    Bind.el (state, (fun s -> text s.Body))
                ]
                Html.ul [
                    Bind.each (
                        tags,
                        fun t ->
                            Html.li [
                                Attr.classes [
                                    "inline-flex"
                                ]
                                Html.span [
                                    Attr.classes [
                                        "px-2"
                                        "py-1"
                                        "rounded-xl"
                                        "text-gray-300"
                                        "mr-1"
                                        "mb-1"
                                        "text-xs"
                                        "border"
                                        "rounded"
                                        "border-gray-300"
                                    ]

                                    text t
                                ]
                            ]
                    )
                ]
                Html.div [
                    Attr.classes [
                        "my-8"
                        "border-b-2"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        "container"
                        "flex"
                        "justify-center"
                    ]
                    articleInfo
                ]
            ]
        ]

    view
