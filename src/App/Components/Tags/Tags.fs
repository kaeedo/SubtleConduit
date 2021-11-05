module SubtleConduit.Components.Tags

open Sutil
open Tailwind
open Sutil.DOM
open SubtleConduit.Types
open SubtleConduit.Services
open Sutil.Attr

let private tags = ObservablePromise<Tags>()

let private getTags () =
    tags.Run
    <| promise {
        let! tagsFromApi = Api.getTags ()
        return tagsFromApi
       }

let Tags (articleFilter: Api.ArticleFilter option) (setArticleFilter: Api.ArticleFilter option -> unit) =
    let view =
        Html.div [
            onMount (fun _ -> getTags ()) [ Once ]
            Attr.classes [
                tw.``px-2``
                tw.``pt-1``
                tw.``pb-2``
                tw.``bg-gray-100``
                tw.rounded
                tw.``w-40``
                tw.``h-full``
            ]
            Html.h6 [
                Attr.classes [
                    tw.``mb-2``
                ]
                text "Popular Tags"
            ]
            Html.div [
                Html.ul [
                    Bind.el (
                        tags,
                        (function
                        | Waiting -> text "Loading"
                        | Error e -> text $"Error occured: {e.Message}"
                        | Result tagsResult ->
                            fragment [
                                for t in tagsResult.Tags do
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
                                            onClick (fun _ -> setArticleFilter <| Some(Api.ArticleFilter.Tag t)) []
                                            text t
                                        ]
                                    ]
                            ])
                    )
                ]
            ]
        ]

    view
