module SubtleConduit.Services.Api.ProfileApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop
open Thoth.Json

let signUp (upsertUser: UpsertUser) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/users"

    promise {
        let json = upsertUser.toJson ()

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.POST
                  Fetch.requestHeaders [
                      Accept "application/json; charset=utf-8"
                      ContentType "application/json; charset=utf-8"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let signIn (email, password) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/users/login"

    promise {
        let json =
            let encoder =
                Encode.object [
                    "email", Encode.string email
                    "password", Encode.string password
                ]

            Encode.toString
                0
                (Encode.object [
                    "user", encoder
                 ])

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.POST
                  Fetch.requestHeaders [
                      Accept "application/json; charset=utf-8"
                      ContentType "application/json; charset=utf-8"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let updateUser (upsertUser: UpsertUser) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/user"

    promise {
        let json = upsertUser.toJson ()

        let! response =
            Fetch.fetch
                url
                [ Method HttpMethod.PUT
                  Fetch.requestHeaders [
                      Authorization $"Token {upsertUser.Token.Value}"
                      Accept "application/json; charset=utf-8"
                      ContentType "application/json; charset=utf-8"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let getProfile (token, username) =
    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/profiles/{username}"

    promise {
        let! response =
            Fetch.fetch
                url
                [ Fetch.requestHeaders [
                      Authorization $"Token {token}"
                      Accept "application/json; charset=utf-8"
                      ContentType "application/json; charset=utf-8"
                  ] ]

        let! profile = response.text ()
        return Profile.fromJson profile
    }

let setFollow (followUsername: string * string * bool) =
    let token, username, shouldFollow = followUsername

    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/profiles/{username}/follow"

    promise {
        let! response =
            Fetch.fetch
                url
                [ if shouldFollow then
                      Method HttpMethod.POST
                  else
                      Method HttpMethod.DELETE
                  Fetch.requestHeaders [
                      Authorization $"Token {token}"
                      Accept "application/json; charset=utf-8"
                      ContentType "application/json; charset=utf-8"
                  ] ]

        let! profile = response.text ()

        return Profile.fromJson profile
    }
