module SubtleConduit.Pages.Profile

open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open Tailwind
open Sutil.DOM
open SubtleConduit.Components

let ProfilePage (profile: Profile) =
    let articleFilter =
        User(profile.Username |> Option.defaultValue "")

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
                            Attr.src (profile.Image |> Option.defaultValue "")
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
                        text (profile.Username |> Option.defaultValue "")
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
                Feed.Feed ignore (Some articleFilter) (ignore)
            ]
        ]

    view
