module SubtleConduit.Components.Tags

open Sutil
open Tailwind
open Sutil.DOM
open SubtleConduit.Services

let Tags () =
    let view () =
        let tags: IStore<string list> = Store.make []

        promise {
            let! tagsFromApi = Api.getTags ()
            tags <~ (tagsFromApi.tags |> List.ofArray) // TODO Refactor to use array once Sutil supports it
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
                tw.``bg-gray-100``
                tw.rounded
                tw.``w-40``
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
                                        tw.``hover:bg-gray-600``
                                        tw.``text-white``
                                        tw.``mr-1``
                                        tw.``mb-1``
                                        tw.``text-xs``
                                    ]
                                    text tag
                                ]
                            ])
                    )
                ]
            ]
        ]

    view ()
