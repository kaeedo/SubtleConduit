module SubtleConduit.Pages.NewArticle

open System
open Sutil
open SubtleConduit.Types
open SubtleConduit.Elmish
open Sutil.DOM
open Sutil.Attr
open SubtleConduit.Services.Api
open SubtleConduit.Router

let NewArticlePage (model: State) (slug: string) =
    let view =
        let user = model.User.Value
        let token = user.Token

        let title = Store.make String.Empty
        let description = Store.make String.Empty
        let body = Store.make String.Empty
        let tag = Store.make ""
        let tags = Store.make []

        let addTag _ =
            tags <~= (fun ts -> [ tag.Value ] @ ts)
            tag <~ String.Empty

        let removeTag tag _ =
            tags
            <~= (fun ts -> ts |> List.filter (fun t -> t <> tag))

        let onSubmit (e: Browser.Types.Event) =
            e.preventDefault ()

            let article = {
                UpsertArticle.Title = title.Value
                Description = description.Value
                Body = body.Value
                TagList = tags.Value
                Token = token
            }

            promise {
                let! newSlug =
                    if String.IsNullOrWhiteSpace(slug) then
                        ArticleApi.createArticle article
                    else
                        ArticleApi.editArticle slug article

                Router.navigate $"article/{newSlug}"
                <| Some(newSlug :> obj)
            }
            |> ignore

        let getArticleToEdit slug =
            promise {
                let! article = ArticleApi.getArticle (slug, "")
                title <~ article.Title
                description <~ article.Description
                body <~ article.Body
                tags <~ article.TagList
            }
            |> ignore

        Html.div [
            onMount
                (fun _ ->
                    if not (String.IsNullOrWhiteSpace(slug)) then
                        getArticleToEdit slug)
                [ Once ]
            disposeOnUnmount [ title; description; body; tags ]

            Attr.classes [
                "container"
                "mx-auto"
                "flex"
                "flex-col"
                "items-center"
            ]
            Html.form [
                on "submit" (onSubmit) []
                Attr.classes [ "w-9/12" ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                            "h-16"
                            "text-xl"
                        ]
                        type' "text"
                        Bind.attr ("value", title)
                        Attr.placeholder "Article Title"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", description)
                        Attr.placeholder "What's this article about?"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.textarea [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        Attr.rows 8
                        Bind.attr ("value", body)
                        Attr.placeholder "Write your article (in markdown)"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4"; "relative" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", tag)
                        onKeyboard
                            "keypress"
                            (fun k ->
                                if k.key = "Enter" then
                                    k.preventDefault ()
                                    addTag ())
                            []
                        Attr.placeholder "Enter tag"
                    ]
                    Html.div [
                        Attr.classes [
                            "absolute"
                            "cursor-pointer"
                            "top-2"
                            "right-2"
                            "bg-gray-500"
                            "hover:bg-gray-600"
                            "text-white"
                            "rounded"
                            "px-3"
                            "pt-px"
                            "pb-1.5"
                            "text-xl"
                        ]
                        text "+"
                        onClick (addTag) []
                    ]
                ]
                Html.ul [
                    Attr.classes [ "flex" ]
                    Bind.each (
                        tags,
                        (fun t ->
                            Html.li [
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
                                Html.span [
                                    Attr.classes [ "mr-1"; "font-bold" ]
                                    text "X"
                                ]
                                Html.span [ text t ]
                                onClick (removeTag t) []
                            ])
                    )
                ]
                Html.div [
                    Attr.classes [ "flex"; "justify-end" ]
                    Html.button [
                        Attr.classes [
                            "flex"
                            "bg-conduit-green"
                            "hover:bg-conduit-green-500"
                            "text-white"
                            "rounded"
                            "px-6"
                            "py-3"
                            "text-xl"
                        ]
                        type' "submit"
                        text (
                            if String.IsNullOrWhiteSpace(slug) then
                                "Publish Article"
                            else
                                "Edit Article"
                        )
                    ]
                ]
            ]
        ]

    view
