module SubtleConduit.Components.Header

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open Tailwind

let Header dispatch =
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
                        Attr.classes [ tw.``h-auto`` ]
                        Html.a [
                            Attr.href "javascript:void(0);"
                            onClick (fun _ -> dispatch (SetPage Home)) [ PreventDefault ]
                            text "Home"
                        ]
                    ]
                    Html.li [
                        Attr.classes [
                            tw.``h-auto``
                            tw.``ml-4``
                        ]
                        Html.a [
                            Attr.href "javascript:void(0);"
                            onClick (fun _ -> dispatch (SetPage SignIn)) [ PreventDefault ]
                            text "Sign In"
                        ]
                    ]
                    Html.li [
                        Attr.classes [
                            tw.``h-auto``
                            tw.``ml-4``
                        ]
                        Html.a [
                            Attr.href "javascript:void(0);"
                            onClick (fun _ -> dispatch (SetPage SignUp)) [ PreventDefault ]
                            text "Sign Up"
                        ]
                    ]
                ]
            ]
        ]

    view
