module SubtleConduit.Services.Api

[<Literal>]
let TAGS_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

[<Literal>]
let ARTICLES_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/articles"

type Tags = Fable.JsonProvider.Generator<TAGS_URL>
type Articles = Fable.JsonProvider.Generator<ARTICLES_URL>

type ArticleFilter =
    | Tag of string
    | User of string


let getTags () =
    promise {
        let! response = Fetch.fetch TAGS_URL []

        let! tags = response.json<Tags> ()
        return tags
    }

let getArticles limit offset (filter: ArticleFilter option) =
    let url =
        $"{ARTICLES_URL}?limit={limit}&offset={offset}"
        + match filter with
          | None -> ""
          | Some (Tag t) -> $"&tag={t}"
          | Some (User u) -> $"&author={u}"

    promise {
        let! response = Fetch.fetch url []

        let! articles = response.json<Articles> ()
        return articles
    }
