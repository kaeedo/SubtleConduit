module SubtleConduit.Pages.SignUp

open System
open Sutil
open SubtleConduit.Types
open SubtleConduit.Elmish
open Sutil.Attr

let SignUpPage dispatch =
    let view =
        let username = Store.make ""
        let email = Store.make ""
        let password = Store.make ""

        Html.div [
            Attr.classes [
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-col``
                tw.``items-center``
            ]
            Html.h1 [
                Attr.classes [ tw.``text-4xl``; tw.``mb-2.5`` ]
                text "Sign up"
            ]
            Html.div [
                Attr.classes [ tw.``mb-4`` ]
                Html.a [
                    Attr.classes [
                        tw.``text-conduit-green``
                        tw.``text-base``
                    ]
                    Attr.href "#signin"
                    text "Have an account?"
                ]
            ]
            Html.form [
                Attr.classes [ tw.``w-96`` ]
                Html.div [
                    Attr.classes [ tw.``mb-4`` ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", username)
                        Attr.placeholder "Username"
                    ]
                ]
                Html.div [
                    Attr.classes [ tw.``mb-4`` ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", email)
                        Attr.placeholder "Email"
                    ]
                ]
                Html.div [
                    Attr.classes [ tw.``mb-4`` ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "password"
                        Bind.attr ("value", password)
                        Attr.placeholder "Password"
                    ]
                ]
                Html.div [
                    Attr.classes [ tw.flex; tw.``justify-end`` ]
                    Html.button [
                        Attr.classes [
                            tw.flex
                            tw.``bg-conduit-green``
                            tw.``hover:bg-conduit-green-500``
                            tw.``text-white``
                            tw.rounded
                            tw.``px-6``
                            tw.``py-3``
                            tw.``text-xl``
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
