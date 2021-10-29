module SubtleConduit.Pages.SignUp

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open SubtleConduit.Components
open Tailwind

let SignUpPage dispatch =
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
                text "Sign up"
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
                    Attr.href "#signin"
                    //onClick (fun _ -> dispatch (SetPage SignIn)) [ PreventDefault ]
                    text "Have an account?"
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
                    TextInput.TextInput { TextInput.Props.Placeholder = "Your Name" }
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    TextInput.TextInput { TextInput.Props.Placeholder = "Email" }
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    TextInput.TextInput { TextInput.Props.Placeholder = "Password" }
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
                        text "Sign up"
                    ]
                ]
            ]
        ]

    view
