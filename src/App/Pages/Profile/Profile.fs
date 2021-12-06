module SubtleConduit.Pages.Profile

open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open Tailwind
open Sutil.DOM
open SubtleConduit.Components

type ProfileState =
    { Username: string
      Image: string
      Bio: string
      Following: bool }

type ProfileMsg =
    | GetProfile of string
    | Set of Profile
    | Error of string

let private init username () =
    { ProfileState.Username = ""
      Image = ""
      Bio = ""
      Following = false },
    Cmd.ofMsg <| GetProfile username

let private mapResultToProfileState (profile: Profile) =
    { ProfileState.Username = profile.Username
      Image = profile.Image
      Bio = profile.Bio
      Following = profile.Following }

let private update msg state =
    match msg with
    | GetProfile username ->
        state, Cmd.OfPromise.either ProfileApi.getProfile username (fun r -> Set r) (fun e -> Error e.Message)
    | Set result ->
        let newState = mapResultToProfileState result
        newState, Cmd.none
    | Error e -> state, Cmd.none // TODO actually handle this


let ProfilePage model (username: string) =
    let articleFilter = ArticleApi.User username

    let state, dispatch =
        Store.makeElmish (init username) update ignore ()

    let view =
        Html.div [
            Html.div [
                Attr.classes [
                    tw.``bg-gray-100``
                    tw.``w-screen``
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
