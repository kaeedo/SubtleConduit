module SubtleConduit.Services.Api

[<Literal>]
let TAGS_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

[<Literal>]
let ARTICLES_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/articles"

type Tags = Fable.JsonProvider.Generator<TAGS_URL>
type Articles = Fable.JsonProvider.Generator<ARTICLES_URL>


let getTags () =
    promise {
        let! response = Fetch.fetch TAGS_URL []

        let! tags = response.json<Tags> ()
        return tags
    }

let getArticles (limit, offset) =
    promise {
        let! response = Fetch.fetch $"{ARTICLES_URL}?limit={limit}&offset={offset}" []

        let! articles = response.json<Articles> ()
        return articles
    }
