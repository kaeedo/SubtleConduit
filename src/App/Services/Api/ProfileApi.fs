module SubtleConduit.Services.Api.ProfileApi

open SubtleConduit.Types
open Fetch.Types
open Fable.Core.JsInterop

let signUp (newUser: NewUser) =
    let url =
        "https://cirosantilli-realworld-next.herokuapp.com/api/users"

    promise {
        let json = newUser.toJson ()

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

let getProfile username =

    let url =
        $"https://cirosantilli-realworld-next.herokuapp.com/api/profiles/{username}"

    promise {
        let! response = Fetch.fetch url []
        let! profile = response.text ()
        return Profile.fromJson profile
    }
