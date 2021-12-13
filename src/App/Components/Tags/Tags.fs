module SubtleConduit.Components.Tags

open System
open Sutil
open Tailwind
open Sutil.DOM
open SubtleConduit.Types
open SubtleConduit.Services.Api
open Sutil.Attr

let private tags = ObservablePromise<Tags>()

let private getTags () =
    tags.Run
    <| promise {
        let! tags = ArticleApi.getTags ()
        // TODO Logging
        match tags with
        | Ok t -> return t
        | Result.Error e -> return { Tags.Tags = [] }
       }

let Tags (articleFilter: ArticleApi.ArticleFilter option) (setArticleFilter: ArticleApi.ArticleFilter option -> unit) =
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
                        | Result t when t.Tags |> List.isEmpty ->
                            fragment [
                                text "Nothing to show"
                            ]
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
                                            onClick
                                                (fun _ ->
                                                    setArticleFilter
                                                    <| Some(ArticleApi.ArticleFilter.Tag t))
                                                []
                                            text t
                                        ]
                                    ]
                            ])
                    )
                ]
            ]
        ]

    view
