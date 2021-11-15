module SubtleConduit.Services.Api

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop

type ArticleFilter =
    | Tag of string
    | User of string

let signUp (newUser: NewUser) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/users"

    promise {
        let json = newUser.toJson ()

        let! response = Fetch.fetch url [ Method HttpMethod.POST; Fetch.requestHeaders [ContentType "application/json"]; Body !^json ]
        let! response = response.text ()
        return User.fromJson response
    }

let getProfile username =

    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/profiles/{username}"

    promise {
        let! response = Fetch.fetch url []
        let! profile = response.text ()
        return Profile.fromJson profile
    }

let getTags () =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/tags"

    promise {
        let! response = Fetch.fetch url []

        let! tags = response.text ()

        return Tags.fromJson tags
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

        let! articles = response.text ()

        return Articles.fromJson articles
    }

let getArticle slug =
    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/articles/{slug}"

    promise {
        let! response = Fetch.fetch url []

        let! article = response.text ()
        return Article.fromJson article
    }
