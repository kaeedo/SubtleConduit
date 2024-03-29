module SubtleConduit.Components.Tags

open Sutil

open Sutil.CoreElements
open SubtleConduit.Types
open SubtleConduit.Services.Api


let private tags = ObservablePromise<Tags>()

let private getTags () =
    tags.Run
    <| promise {
        let! tags = ArticleApi.getTags ()
        // TODO Logging
        match tags with
        | Ok t -> return t
        | Result.Error e -> return Unchecked.defaultof<Tags>
    }

let Tags (articleFilter: ArticleApi.ArticleFilter option) (setArticleFilter: ArticleApi.ArticleFilter option -> unit) =
    let view =
        Html.div [
            onMount (fun _ -> getTags ()) [ Once ]
            Attr.classes [
                "px-2"
                "pt-1"
                "pb-2"
                "bg-gray-100"
                "rounded"
                "w-40"
                "h-full"
            ]
            Html.h6 [
                Attr.classes [ "mb-2" ]
                text "Popular Tags"
            ]
            Html.div [
                Html.ul [
                    Attr.testId "tags"
                    Bind.el (
                        tags,
                        (function
                        | PromiseState.Waiting -> text "Loading"
                        | PromiseState.Error e -> text $"Error occured: {e.Message}"
                        | PromiseState.Result t when t.Tags |> List.isEmpty -> fragment [ text "Nothing to show" ]
                        | PromiseState.Result tagsResult ->
                            fragment [
                                for t in tagsResult.Tags do
                                    Html.li [
                                        Attr.classes [ "inline-flex" ]
                                        Html.span [
                                            Attr.classes [
                                                "px-2"
                                                "py-1"
                                                "rounded-xl"
                                                "cursor-pointer"
                                                "bg-gray-500"
                                                "hover:bg-gray-600"
                                                "text-white"
                                                "mr-1"
                                                "mb-1"
                                                "text-xs"
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
