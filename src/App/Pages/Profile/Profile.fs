module SubtleConduit.Pages.Profile

open System
open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open SubtleConduit.Elmish
open SubtleConduit.Router
open Tailwind
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
                    tw.``bg-gray-100``
                    tw.``w-full``
                    tw.``p-8``
                ]
                Html.div [
                    Attr.classes [
                        tw.container
                        tw.``mx-auto``
                        tw.``text-black``
                    ]
                    Html.h1 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-6xl``
                            tw.``font-mono``
                            tw.``font-bold``
                            tw.``text-shadow-lg``
                            tw.``mb-2``
                            tw.``cursor-default``
                            tw.flex
                            tw.``justify-center``
                        ]
                        Html.img [
                            Attr.classes [
                                tw.``h-28``
                                tw.``w-28``
                                tw.``rounded-full``
                            ]

                            Bind.el (state, (fun s -> Attr.src s.Image))
                        ]
                    ]
                    Html.h2 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-2xl``
                            tw.``font-bold``
                            tw.``cursor-default``
                        ]
                        Bind.el (state, (fun s -> text s.Username))
                    ]
                    Html.h4 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-sm``
                            tw.``text-gray-400``
                            tw.``cursor-default``
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
                                        tw.flex
                                        tw.``justify-end``
                                    ]
                                    Html.div [
                                        Attr.classes [
                                            tw.``cursor-pointer``
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
                    tw.container
                    tw.``mx-auto``
                    tw.``mt-8``
                    tw.flex
                    tw.``justify-between``
                ]
                Feed.Feed model ignore (Some articleFilter) (ignore)
            ]
        ]

    view
