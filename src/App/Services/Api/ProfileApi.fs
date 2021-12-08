module SubtleConduit.Services.Api.ProfileApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop
open Thoth.Json

let signUp (upsertUser: UpsertUser) =
    let url = "https://api.realworld.io/api/users"

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

let signIn (email, password) =
    let url =
        "https://api.realworld.io/api/users/login"

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
                      Accept "application/json"
                      ContentType "application/json"
                  ]
                  Body !^json ]

        let! response = response.text ()
        return User.fromJson response
    }

let updateUser (upsertUser: UpsertUser) =
    let url = "https://api.realworld.io/api/user"

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

let getProfile (token, username) =
    let url =
        $"https://api.realworld.io/api/profiles/{username}"

    promise {
        let! response =
            Fetch.fetch
                url
                [ Fetch.requestHeaders [
                      Authorization $"Token {token}"
                  ] ]

        let! profile = response.text ()
        return Profile.fromJson profile
    }

let setFollow (followUsername: string * string * bool) =
    let token, username, shouldFollow = followUsername

    let url =
        $"https://api.realworld.io/api/profiles/{username}/follow"

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
                  ] ]

        let! profile = response.text ()

        return Profile.fromJson profile
    }

let followUser username isFollowing token =
    let url =
        $"https://api.realworld.io/api/profiles/{username}/follow"

    promise {
        let! response =
            Fetch.fetch
                url
                [ Method(
                    if isFollowing then
                        HttpMethod.DELETE
                    else
                        HttpMethod.POST
                  )
                  Fetch.requestHeaders [
                      Authorization $"Token {token}"
                      ContentType "application/json"
                  ] ]

        let! response = response.text ()

        let profile = Profile.fromJson response

        return profile
    }
