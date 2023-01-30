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
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-col``
                tw.``items-center``
            ]
            Html.h1 [
                Attr.classes [ tw.``text-4xl``; tw.``mb-2.5`` ]
                text "Sign In"
            ]
            Html.div [
                Attr.classes [ tw.``mb-4`` ]
                Html.a [
                    Attr.classes [
                        tw.``text-conduit-green``
                        tw.``text-base``
                    ]
                    Attr.href "#signup"
                    text "Need an account?"
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
                        onClick (fun _ -> dispatch (Message.SignIn(email.Value, password.Value))) []
                        text "Sign in"
                    ]
                ]
            ]
        ]

    view
