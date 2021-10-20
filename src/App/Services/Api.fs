module SubtleConduit.Services.Api

open Fable.Core.JS

[<Literal>]
let TAGS_URL =
    "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

// Type is created automatically from the url
type Tags = Fable.JsonProvider.Generator<TAGS_URL>


let getTags () : Promise<string list> =
    promise {
        let! response = Fetch.fetch TAGS_URL []

        let! tags = response.json<Tags> ()
        return tags.tags |> List.ofArray // TODO Refactor to use array once Sutil supports it
    }
