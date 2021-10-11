module SubtleConduit.Components.Header

open Sutil
open Sutil.Attr
open SubtleConduit.Types
open SubtleConduit.Router

let Header dispatch =
    let view =
        Html.nav [
            class' "container mx-auto flex flex-row justify-between py-2"
            Html.span [
                class' "text-2xl font-mono text-conduit-green"
                text "conduit"
            ]
            Html.div [
                class' "flex justify-end content-center h-auto"
                Html.ul [
                    class' "flex flex-row"
                    Html.li [
                        class' "h-auto"
                        Html.a [
                            Attr.href "javascript:void(0);"
                            onClick (fun _ -> dispatch (SetPage Home)) [ PreventDefault ]
                            text "Home"
                        ]
                    ]
                    Html.li [
                        class' "h-auto ml-4"
                        Html.a [
                            Attr.href "javascript:void(0);"
                            onClick (fun _ -> dispatch (SetPage SignIn)) [ PreventDefault ]
                            text "Sign In"
                        ]
                    ]
                    Html.li [
                        class' "h-auto ml-4"
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
