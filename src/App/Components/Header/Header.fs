module SubtleConduit.Components.Header

open System
open Sutil
open Sutil.Attr
open SubtleConduit.Types
open SubtleConduit.Elmish
open Tailwind

let Header (model, dispatch) =
    let isHome = model .> fun m -> m.Page = Page.Home
    let isSignIn = model .> fun m -> m.Page = Page.SignIn
    let isSignUp = model .> fun m -> m.Page = Page.SignUp

    let isSettings = model .> fun m -> m.Page = Page.Settings

    let isMyProfile (u: User) =
        model .> fun m -> m.Page = Page.Profile u.Username

    let isNewArticle =
        model
        .> fun m -> m.Page = Page.NewArticle String.Empty

    let loggedOutMenuItems =
        Html.ul [
            Attr.classes [
                tw.flex
                tw.``flex-row``
            ]
            Html.li [
                Bind.toggleClass (isHome, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/home"
                    text "Home"
                ]
            ]
            Html.li [
                Bind.toggleClass (isSignIn, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``ml-4``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/signin"
                    text "Sign In"
                ]
            ]
            Html.li [
                Bind.toggleClass (isSignUp, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``ml-4``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/signup"
                    text "Sign Up"
                ]
            ]
        ]

    let loggedInMenuItems (u: User) =
        Html.ul [
            Attr.classes [
                tw.flex
                tw.``flex-row``
            ]
            Html.li [
                Bind.toggleClass (isHome, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/home"
                    text "Home"
                ]
            ]
            Html.li [
                Bind.toggleClass (isNewArticle, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``ml-4``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/editor"
                    text "New Article"
                ]
            ]
            Html.li [
                Bind.toggleClass (isSettings, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``ml-4``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href "#/settings"
                    text "Settings"
                ]
            ]
            Html.li [
                Bind.toggleClass (isMyProfile u, tw.``text-gray-700``, tw.``text-gray-400``)

                Attr.classes [
                    tw.``h-auto``
                    tw.``ml-4``
                    tw.``hover:text-gray-700``
                ]
                Html.a [
                    Attr.href $"#/profile/{u.Username}"
                    text u.Username
                ]
            ]
        ]


    let view =
        Html.nav [
            Attr.classes [
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-row``
                tw.``justify-between``
                tw.``py-2``
            ]
            Html.span [
                Attr.classes [
                    tw.``text-2xl``
                    tw.``font-mono``
                    tw.``text-conduit-green``
                    tw.``cursor-default``
                ]
                text "conduit"
            ]
            Html.div [
                Attr.classes [
                    tw.flex
                    tw.``justify-end``
                    tw.``content-center``
                    tw.``h-auto``
                ]
                Bind.el (
                    model,
                    fun m ->
                        match m.User with
                        | Some u -> loggedInMenuItems u
                        | None -> loggedOutMenuItems
                )
            ]
        ]

    view
