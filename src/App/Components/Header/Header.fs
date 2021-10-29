module SubtleConduit.Components.Header

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open Tailwind

let Header (model, dispatch) =
    let isHome = model .> fun m -> m.Page = Page.Home
    let isSignIn = model .> fun m -> m.Page = Page.SignIn
    let isSignUp = model .> fun m -> m.Page = Page.SignUp

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
                            //onClick (fun _ -> dispatch (SetPage Home)) [ PreventDefault ]
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
                            //onClick (fun _ -> dispatch (SetPage SignIn)) [ PreventDefault ]
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
                            //onClick (fun _ -> dispatch (SetPage SignUp)) [ PreventDefault ]
                            text "Sign Up"
                        ]
                    ]
                ]
            ]
        ]

    view
