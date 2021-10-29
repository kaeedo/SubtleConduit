module SubtleConduit.Services.Api

[<Literal>]
let TAGS_URL = "./services/TagsSampleResponse.json"

[<Literal>]
let ARTICLES_URL = "./services/ArticlesSampleResponse.json"

type Tags = Fable.JsonProvider.Generator<TAGS_URL>
type Articles = Fable.JsonProvider.Generator<ARTICLES_URL>

type ArticleFilter =
    | Tag of string
    | User of string


let getTags () =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

    promise {
        let! response = Fetch.fetch url []

        let! tags = response.json<Tags> ()
        return tags
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

        let! articles = response.json<Articles> ()
        return articles
    }
