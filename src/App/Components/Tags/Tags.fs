module SubtleConduit.Components.Tags

open Sutil
open Tailwind
open Sutil.DOM

let Tags () =
    let view () =
        let tags: IStore<string array> =
            Store.make [|
                "wef"
                "rthrth"
            |]

        let otherTags: IStore<string list> =
            Store.make [
                "wef"
                "rthrth"
            ]


        // promise {
        //     let! tagsFromApi = SubtleConduit.Services.Api.getTags ()
        //     tags <~ tagsFromApi

        // //let otherTagsFromApi = tagsFromApi |> List.ofArray
        // //otherTags <~ otherTagsFromApi
        // }
        // |> Promise.start

        Html.div [
            disposeOnUnmount [
                tags
                otherTags
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
                    Bind.each (otherTags, (fun tag -> Html.li [ text tag ]))
                ]
            ]
        ]

    view ()
