module SubtleConduit.Pages.SignUp

open System
open Sutil
open SubtleConduit.Types
open SubtleConduit.Elmish
open Sutil.CoreElements

let SignUpPage dispatch =
    let view =
        let username = Store.make ""
        let email = Store.make ""
        let password = Store.make ""

        Html.div [
            Attr.classes [
                "container"
                "mx-auto"
                "flex"
                "flex-col"
                "items-center"
            ]
            Html.h1 [
                Attr.classes [ "text-4xl"; "mb-2.5" ]
                text "Sign up"
            ]
            Html.div [
                Attr.classes [ "mb-4" ]
                Html.a [
                    Attr.classes [ "text-conduit-green"; "text-base" ]
                    Attr.href "#/signin"
                    text "Have an account?"
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
                        Bind.attr ("value", username)
                        Attr.placeholder "Username"
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
                        onClick
                            (fun _ ->
                                dispatch (
                                    SignUp
                                        {
                                            UpsertUser.Username = username.Value
                                            Image = String.Empty
                                            Bio = String.Empty
                                            Token = None
                                            Email = email.Value
                                            Password = password.Value
                                        }
                                ))
                            []
                        text "Sign up"
                    ]
                ]
            ]
        ]

    view
