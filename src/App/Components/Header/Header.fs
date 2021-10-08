module SubtleConduit.Components.Header

open Sutil
open Sutil.Attr

let Header () =
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
                            text "Home"
                        ]
                    ]
                    Html.li [
                        class' "h-auto ml-4"
                        Html.a [
                            Attr.href "javascript:void(0);"
                            text "Sign In"
                        ]
                    ]
                    Html.li [
                        class' "h-auto ml-4"
                        Html.a [
                            Attr.href "javascript:void(0);"
                            text "Sign Up"
                        ]
                    ]
                ]
            ]
        ]

    view