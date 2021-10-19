module SubtleConduit.Services.Api

[<Literal>]
let TAGS_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

// Type is created automatically from the url
type Todos = Fable.JsonProvider.Generator<TAGS_URL>


let getTags () =
    promise {
        let! response = Fetch.fetch TAGS_URL []

        let! tags = response.json<string array> ()
        return tags
    }
