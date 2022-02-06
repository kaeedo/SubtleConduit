module SubtleConduit.Pages.Profile

open System
open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open SubtleConduit.Elmish
open SubtleConduit.Router
open Sutil.DOM
open Sutil.Attr
open SubtleConduit.Components

type ProfileState =
    { Username: string
      Image: string
      Bio: string
      Following: bool
      Token: string }

type ProfileMsg =
    | GetProfile of string
    | Set of Profile
    | Error of string
    | Follow of string
    | Unfollow of string

let private init username token () =
    { ProfileState.Username = String.Empty
      Image = String.Empty
      Bio = String.Empty
      Following = false
      Token = token },
    Cmd.ofMsg <| GetProfile username

let private update msg (state: ProfileState) =
    match msg with
    | GetProfile username ->
        state,
        Cmd.OfPromise.either ProfileApi.getProfile (state.Token, username) (fun r -> Set r) (fun e -> Error e.Message)
    | Set result ->
        let newState =
            { state with
                Username = result.Username
                Image = result.Image
                Bio = result.Bio
                Following = result.Following }

        newState, Cmd.none
    | Error e -> state, Cmd.none // TODO actually handle this
    | Follow username ->
        state,
        Cmd.OfPromise.either
            ProfileApi.setFollow
            (state.Token, username, true)
            (fun r -> Set r)
            (fun e -> Error e.Message)
    | Unfollow username ->
        state,
        Cmd.OfPromise.either
            ProfileApi.setFollow
            (state.Token, username, false)
            (fun r -> Set r)
            (fun e -> Error e.Message)


let ProfilePage (model: State) (username: string) =
    let articleFilter = ArticleApi.Author username

    let state, dispatch =
        match model.User with
        | None -> Store.makeElmish (init username String.Empty) update ignore ()
        | Some u -> Store.makeElmish (init username u.Token) update ignore ()

    let view =
        Html.div [
            disposeOnUnmount [
                state
            ]
            Html.div [
                Attr.classes [
                    "bg-gray-100"
                    "w-full"
                    "p-8"
                ]
                Html.div [
                    Attr.classes [
                        "container"
                        "mx-auto"
                        "text-black"
                    ]
                    Html.h1 [
                        Attr.classes [
                            "mx-auto"
                            "text-center"
                            "text-6xl"
                            "font-mono"
                            "font-bold"
                            "text-shadow-lg"
                            "mb-2"
                            "cursor-default"
                            "flex"
                            "justify-center"
                        ]
                        Html.img [
                            Attr.classes [
                                "h-28"
                                "w-28"
                                "rounded-full"
                            ]

                            Bind.el (state, (fun s -> Attr.src s.Image))
                        ]
                    ]
                    Html.h2 [
                        Attr.classes [
                            "mx-auto"
                            "text-center"
                            "text-2xl"
                            "font-bold"
                            "cursor-default"
                        ]
                        Bind.el (state, (fun s -> text s.Username))
                    ]
                    Html.h4 [
                        Attr.classes [
                            "mx-auto"
                            "text-center"
                            "text-sm"
                            "text-gray-400"
                            "cursor-default"
                        ]
                        Bind.el (state, (fun s -> text s.Bio))
                    ]
                    Bind.el (
                        state,
                        (fun s ->
                            match model.User with
                            | None -> fragment []
                            | Some u ->
                                Html.div [
                                    Attr.classes [
                                        "flex"
                                        "justify-end"
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            "cursor-pointer"
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
                                        if u.Username = s.Username then
                                            onClick (fun _ -> Router.navigate "settings" None) []
                                            text "Edit profile settings"
                                        else if s.Following then
                                            onClick (fun _ -> dispatch <| Unfollow s.Username) []
                                            text $"- Unfollow {s.Username}"
                                        else
                                            onClick (fun _ -> dispatch <| Follow s.Username) []
                                            text $"+ Follow {s.Username}"

                                        ]
                                ])
                    )

                    ]
            ]
            Html.div [
                Attr.classes [
                    "container"
                    "mx-auto"
                    "mt-8"
                    "flex"
                    "justify-between"
                ]
                Feed.Feed model ignore (Some articleFilter) (ignore)
            ]
        ]

    view
