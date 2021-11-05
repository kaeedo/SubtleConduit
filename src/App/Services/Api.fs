module SubtleConduit.Services.Api

open SubtleConduit.Types

type ArticleFilter =
    | Tag of string
    | User of string

let getProfile username =

    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/profiles/{username}"

    promise {
        let! response = Fetch.fetch url []
        let! profile = response.json<JsonProfile> ()
        return Profile.fromJsonProfile profile
    }

let getTags () =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

    promise {
        let! response = Fetch.fetch url []

        let! tags = response.json<JsonTags> ()

        return Tags.fromJsonTags tags
    }

let getArticles limit offset (filter: ArticleFilter option) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/articles"

    let url =
        $"{url}?limit={limit}&offset={offset}"
        + match filter with
          | None -> ""
          | Some (Tag t) -> $"&tag={t}"
          | Some (User u) -> $"&author={u}"

    promise {
        let! response = Fetch.fetch url []

        let! articles = response.json<JsonArticles> ()
        return Articles.fromJsonArticles articles
    }

let getArticle slug =
    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/articles/{slug}"

    promise {
        let! response = Fetch.fetch url []

        let! article = response.json<JsonArticle> ()
        return Article.fromJsonArticle article
    }
