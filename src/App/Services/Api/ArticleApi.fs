module SubtleConduit.Services.Api.ArticleApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop

// api.realworld.io

type ArticleFilter =
    | Tag of string
    | Author of string
    | Favorited of string

let getTags () =
    let url = "https://api.realworld.io/api/tags"

    promise {
        let! response = Fetch.fetch url []
        let! tags = response.text ()
        return Tags.fromJson tags
    }

let getArticles (user: User option) limit offset (filter: ArticleFilter option) showFeed =
    let url =
        if showFeed then
            "https://api.realworld.io/api/articles/feed"
        else
            "https://api.realworld.io/api/articles"

    let url =
        $"{url}?limit={limit}&offset={offset}"
        + match filter with
          | None -> ""
          | Some(Tag t) -> $"&tag={t}"
          | Some(Author u) -> $"&author={u}"
          | Some(Favorited u) -> $"&favorited={u}"

    promise {
        let! response =
            Fetch.fetch url [
                Fetch.requestHeaders [
                    match user with
                    | None -> ()
                    | Some u -> Authorization $"Token {u.Token}"
                    Accept "application/json; charset=utf-8"
                    ContentType "application/json; charset=utf-8"
                ]
            ]

        let! articles = response.text ()

        return Articles.fromJson articles
    }

let getArticle (slug, token) =
    let url = $"https://api.realworld.io/api/articles/{slug}"

    promise {
        let! response =
            Fetch.fetch url [
                Method HttpMethod.GET
                Fetch.requestHeaders [
                    Authorization $"Token {token}"
                    Accept "application/json; charset=utf-8"
                    ContentType "application/json; charset=utf-8"
                ]
            ]

        let! article = response.text ()
        return Article.fromJson article
    }

let deleteArticle slug token =
    let url = $"https://api.realworld.io/api/articles/{slug}"

    promise {
        let! response =
            Fetch.fetch url [
                Method HttpMethod.DELETE
                Fetch.requestHeaders [ Authorization $"Token {token}" ]
            ]

        return ()
    }

let createArticle (article: UpsertArticle) =
    let url = "https://api.realworld.io/api/articles"

    promise {
        let json = article.toJson ()

        let! response =
            Fetch.fetch url [
                Method HttpMethod.POST
                Fetch.requestHeaders [
                    Authorization $"Token {article.Token}"
                    Accept "application/json; charset=utf-8"
                    ContentType "application/json; charset=utf-8"
                ]
                Body !^json
            ]

        let! response = response.text ()

        let article = Article.fromJson response

        return article.Slug
    }

let editArticle slug (article: UpsertArticle) =
    let url = $"https://api.realworld.io/api/articles/{slug}"

    promise {
        let json = article.toJson ()

        let! response =
            Fetch.fetch url [
                Method HttpMethod.PUT
                Fetch.requestHeaders [
                    Authorization $"Token {article.Token}"
                    Accept "application/json; charset=utf-8"
                    ContentType "application/json; charset=utf-8"
                ]
                Body !^json
            ]

        let! response = response.text ()

        let article = Article.fromJson response

        return article.Slug
    }

let favoriteArticle slug isFavorited token =
    let url =
        $"https://api.realworld.io/api/articles/{slug}/favorite"

    promise {
        let! response =
            Fetch.fetch url [
                Method(
                    if isFavorited then
                        HttpMethod.DELETE
                    else
                        HttpMethod.POST
                )
                Fetch.requestHeaders [
                    Authorization $"Token {token}"
                    Accept "application/json; charset=utf-8"
                    ContentType "application/json; charset=utf-8"
                ]
            ]

        let! response = response.text ()

        let article = Article.fromJson response

        return article
    }
