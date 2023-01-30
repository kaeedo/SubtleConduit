module SubtleConduit.Pages.SignIn

open System
open Sutil
open Sutil.Attr
open Sutil.DOM
open SubtleConduit.Elmish

let SignInPage dispatch =
    let view =
        let email = Store.make String.Empty
        let password = Store.make String.Empty

        Html.div [
            disposeOnUnmount [ email; password ]

            Attr.classes [
                "container"
                "mx-auto"
                "flex"
                "flex-col"
                "items-center"
            ]
            Html.h1 [
                Attr.classes [ "text-4xl"; "mb-2.5" ]
                text "Sign In"
            ]
            Html.div [
                Attr.classes [ "mb-4" ]
                Html.a [
                    Attr.classes [
                        "text-conduit-green"
                        "text-base"
                    ]
                    Attr.href "#signup"
                    text "Need an account?"
                ]
            ]
            Html.form [
                Attr.classes [ "w-96" ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", email)
                        Attr.placeholder "Email"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "password"
                        Bind.attr ("value", password)
                        Attr.placeholder "Password"
                    ]
                ]
                Html.div [
                    Attr.classes [ "flex"; "justify-end" ]
                    Html.button [
                        Attr.classes [
                            "flex"
                            "bg-conduit-green"
                            "hover:bg-conduit-green-500"
                            "text-white"
                            "rounded"
                            "px-6"
                            "py-3"
                            "text-xl"
                        ]
                        Attr.typeSubmit
                        onClick (fun _ -> dispatch (Message.SignIn(email.Value, password.Value))) []
                        text "Sign in"
                    ]
                ]
            ]
        ]

    view
