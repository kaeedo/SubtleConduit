module SubtleConduit.Pages.SignIn

open Sutil
open SubtleConduit.Components
open Tailwind
open Sutil.Attr

let SignInPage dispatch =
    let view =
        Html.div [
            Attr.classes [
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-col``
                tw.``items-center``
            ]
            Html.h1 [
                Attr.classes [
                    tw.``text-4xl``
                    tw.``mb-2.5``
                ]
                text "Sign In"
            ]
            Html.div [
                Attr.classes [
                    tw.``mb-4``
                ]
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
                Attr.classes [
                    tw.``w-96``
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
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
                        //Bind.attr ("value", password)
                        Attr.placeholder "Email"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
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
                        //Bind.attr ("value", password)
                        Attr.placeholder "Password"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.flex
                        tw.``justify-end``
                    ]
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
                        text "Sign in"
                    ]
                ]
            ]
        ]

    view
