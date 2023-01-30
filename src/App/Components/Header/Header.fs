module SubtleConduit.Components.Header

open System
open Sutil
open SubtleConduit.Types
open SubtleConduit.Elmish

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
            Attr.classes [ "flex"; "flex-row" ]
            Html.li [
                Bind.toggleClass (isHome, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "hover:text-gray-700"
                ]
                Html.a [ Attr.href "#/home"; text "Home" ]
            ]
            Html.li [
                Bind.toggleClass (isSignIn, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "ml-4"
                    "hover:text-gray-700"
                ]
                Html.a [ Attr.href "#/signin"; text "Sign In" ]
            ]
            Html.li [
                Bind.toggleClass (isSignUp, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "ml-4"
                    "hover:text-gray-700"
                ]
                Html.a [ Attr.href "#/signup"; text "Sign Up" ]
            ]
        ]

    let loggedInMenuItems (u: User) =
        Html.ul [
            Attr.classes [ "flex"; "flex-row" ]
            Html.li [
                Bind.toggleClass (isHome, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "hover:text-gray-700"
                ]
                Html.a [ Attr.href "#/home"; text "Home" ]
            ]
            Html.li [
                Bind.toggleClass (isNewArticle, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "ml-4"
                    "hover:text-gray-700"
                ]
                Html.a [
                    Attr.href "#/editor"
                    text "New Article"
                ]
            ]
            Html.li [
                Bind.toggleClass (isSettings, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "ml-4"
                    "hover:text-gray-700"
                ]
                Html.a [
                    Attr.href "#/settings"
                    text "Settings"
                ]
            ]
            Html.li [
                Bind.toggleClass (isMyProfile u, "text-gray-700", "text-gray-400")

                Attr.classes [
                    "h-auto"
                    "ml-4"
                    "hover:text-gray-700"
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
                "container"
                "mx-auto"
                "flex"
                "flex-row"
                "justify-between"
                "py-2"
            ]
            Html.span [
                Attr.classes [
                    "text-2xl"
                    "font-mono"
                    "text-conduit-green"
                    "cursor-default"
                ]
                text "conduit"
            ]
            Html.div [
                Attr.classes [
                    "flex"
                    "justify-end"
                    "content-center"
                    "h-auto"
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
