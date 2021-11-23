module SubtleConduit.Services.Api.ProfileApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop

let signUp (upsertUser: UpsertUser) =
    let url =
        "https://conduit.productionready.io/api/users"

    promise {
        let json = upsertUser.toJson ()

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.POST
                  Fetch.requestHeaders [
                      ContentType "application/json"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let updateUser (upsertUser: UpsertUser) =
    let url =
        "https://conduit.productionready.io/api/user"

    promise {
        let json = upsertUser.toJson ()

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.PUT
                  Fetch.requestHeaders [
                      Accept "application/json"
                      Authorization $"Token {upsertUser.Token.Value}"
                      ContentType "application/json"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let getProfile username =

    let url =
        $"https://conduit.productionready.io/api/profiles/{username}"

    promise {
        let! response = Fetch.fetch url []
        let! profile = response.text ()
        return Profile.fromJson profile
    }
