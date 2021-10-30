module SubtleConduit.Pages.Profile

open Sutil
open SubtleConduit.Components
open SubtleConduit.Types
open Tailwind

let ProfilePage (profile: Profile) =
    let view =
        Html.div [
            Html.div [
                Attr.classes [
                    tw.``bg-conduit-green``
                    tw.``w-screen``
                    tw.``p-8``
                ]
                Html.div [
                    Attr.classes [
                        tw.container
                        tw.``mx-auto``
                        tw.``text-white``
                    ]
                    Html.h1 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-6xl``
                            tw.``font-mono``
                            tw.``font-bold``
                            tw.``text-shadow-lg``
                            tw.``mb-2``
                            tw.``cursor-default``
                        ]
                        Html.img [
                            Attr.classes [
                                tw.``w-8``
                                tw.``h-8``
                                tw.``rounded-3xl``
                                tw.``self-center``
                            ]
                            Attr.src profile.profile.image
                        ]
                    ]
                    Html.h2 [
                        Attr.classes [
                            tw.``mx-auto``
                            tw.``text-center``
                            tw.``text-2xl``
                            tw.``font-light``
                            tw.``cursor-default``
                        ]
                        text profile.profile.username
                    ]
                ]
            ]
            Html.div [
                Attr.classes [
                    tw.container
                    tw.``mx-auto``
                    tw.``mt-8``
                    tw.flex
                    tw.``justify-between``
                ]
                // Bind.el (
                //     articleFilter,
                //     (fun af ->
                //         let setter = Store.set articleFilter

                //         fragment [
                //             Feed.Feed af setter
                //             Tags.Tags af setter
                //         ])
                // )
                ]
        ]

    view
