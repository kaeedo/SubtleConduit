module SubtleConduit.Components.Tags

open Sutil
open Tailwind
open Sutil.DOM

let Tags () =
    let view () =
        let tags: IStore<string list> = Store.make []

        promise {
            let! tagsFromApi = SubtleConduit.Services.Api.getTags ()
            tags <~ tagsFromApi
        }
        |> Promise.start

        Html.div [
            disposeOnUnmount [
                tags
            ]
            Attr.classes [
                tw.``px-2``
                tw.``pt-1``
                tw.``pb-2``
                tw.``bg-gray-200``
                tw.rounded
                tw.``w-64``
            ]
            Html.h6 [
                Attr.classes [
                    tw.``mb-2``
                ]
                text "Popular Tags"
            ]
            Html.div [
                Html.ul [
                    Bind.each (
                        tags,
                        (fun tag ->
                            Html.li [
                                Attr.classes [
                                    tw.``inline-flex``
                                ]
                                Html.span [
                                    Attr.classes [
                                        tw.``px-2``
                                        tw.``py-1``
                                        tw.``rounded-xl``
                                        tw.``cursor-pointer``
                                        tw.``bg-gray-500``
                                        tw.``hover:bg-gray-700``
                                        tw.``text-white``
                                        tw.``mr-1``
                                        tw.``mb-1``
                                    ]
                                    text tag
                                ]
                            ])
                    )
                ]
            ]
        ]

    view ()
