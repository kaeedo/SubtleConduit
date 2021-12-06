module SubtleConduit.Services.Api.ArticleApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop

type ArticleFilter =
    | Tag of string
    | User of string

let getTags () =
    let url = "https://api.realworld.io/api/tags"

    promise {
        let! response = Fetch.fetch url []

        let! tags = response.text ()

        return Tags.fromJson tags
    }

let getArticles (user: User option) limit offset (filter: ArticleFilter option) =
    let url = "https://api.realworld.io/api/articles"

    let url =
        $"{url}?limit={limit}&offset={offset}"
        + match filter with
          | None -> ""
          | Some (Tag t) -> $"&tag={t}"
          | Some (User u) -> $"&author={u}"

    promise {
        let! response =
            Fetch.fetch
                url
                [ Fetch.requestHeaders [
                      match user with
                      | None -> ()
                      | Some u -> Authorization $"Token {u.Token}"
                  ] ]

        let! articles = response.text ()

        return Articles.fromJson articles
    }

let getArticle slug =
    let url =
        $"https://api.realworld.io/api/articles/{slug}"

    promise {
        let! response = Fetch.fetch url []

        let! article = response.text ()
        return Article.fromJson article
    }

let deleteArticle slug token =
    let url =
        $"https://api.realworld.io/api/articles/{slug}"

    promise {
        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.DELETE
                  Fetch.requestHeaders [
                      Authorization $"Token {token}"
                  ] ]

        return ()
    }

let createArticle (article: UpsertArticle) =
    let url = "https://api.realworld.io/api/articles"

    promise {
        let json = article.toJson ()

        Fable.Core.JS.console.log (json)

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.POST
                  Fetch.requestHeaders [
                      Authorization $"Token {article.Token}"
                      ContentType "application/json"
                  ]
                  Body !^json ]

        let! response = response.text ()
        // TODO: Redirect to new article
        return response
    }
