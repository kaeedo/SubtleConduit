module SubtleConduit.Types

open System
open Sutil
open Fable.Core.JS
open Thoth.Json

let inline private encoder<'T> =
    Encode.Auto.generateEncoderCached<'T> (caseStrategy = CamelCase)

let inline private decoder<'T> extras =
    Decode.Auto.generateDecoderCached<'T> (caseStrategy = CamelCase, extra = extras)

type Tags =
    { Tags: string list }
    static member fromJson(json: string) : Result<Tags, string> =
        Decode.Auto.fromString<Tags> (json, caseStrategy = CamelCase)

type User =
    { Email: string
      Token: string
      Username: string
      Bio: string
      Image: string
      Following: bool }
    static member Encoder = encoder<User>

    static member Decoder: Decoder<User> =
        Decode.object (fun get ->
            { User.Username = get.Required.Field "username" Decode.string
              User.Email = get.Required.Field "email" Decode.string
              User.Token = get.Required.Field "token" Decode.string
              Bio =
                get.Optional.Field "bio" Decode.string
                |> Option.defaultValue ""
              Image =
                get.Optional.Field "image" Decode.string
                |> Option.defaultValue ""
              Following =
                get.Optional.Field "following" Decode.bool
                |> Option.defaultValue false })

    static member fromJson(json: string) =
        Decode.unsafeFromString (Decode.field "user" User.Decoder) json

type Profile =
    { Username: string
      Bio: string
      Image: string
      Following: bool }
    static member Encoder = encoder<Profile>

    static member Decoder: Decoder<Profile> =
        Decode.object (fun get ->
            { Profile.Username = get.Required.Field "username" Decode.string
              Bio =
                get.Optional.Field "bio" Decode.string
                |> Option.defaultValue ""
              Image =
                get.Optional.Field "image" Decode.string
                |> Option.defaultValue ""
              Following =
                get.Optional.Field "following" Decode.bool
                |> Option.defaultValue false })

    static member fromJson(json: string) =
        Decode.unsafeFromString (Decode.field "profile" Profile.Decoder) json

type Article =
    { Slug: string
      Title: string
      Description: string
      Body: string
      CreatedAt: DateTime
      UpdatedAt: DateTime
      TagList: string list
      Favorited: bool
      FavoritesCount: int
      Author: Profile }
    static member Encoder = encoder<Article>

    static member Decoder: Decoder<Article> =
        let extras =
            Extra.empty
            |> Extra.withCustom Profile.Encoder (Profile.Decoder)

        decoder<Article> extras

    static member fromJson(json: string) =
        let decoder = Decode.field "article" Article.Decoder

        Decode.unsafeFromString decoder json

type Articles =
    { Articles: Article list // TODO Refactor to array
      ArticlesCount: int }

    static member Encoder = encoder<Articles>

    static member Decoder: Decoder<Articles> =
        let extras =
            Extra.empty
            |> Extra.withCustom Article.Encoder (Article.Decoder)

        decoder<Articles> extras

    static member fromJson(json: string) =
        Decode.unsafeFromString Articles.Decoder json

type NewUser =
    { Username: string
      Email: string
      Password: string }

    static member Encoder = encoder<{| User: NewUser |}>
    static member Decoder = decoder<NewUser> Extra.empty

    member this.toJson() =
        NewUser.Encoder {| User = this |}
        |> Encode.toString 0

type ApiErrors =
    { Body: string list } // TODO refactor this to array
    static member Decoder =
        Decode.field
            "errors"
            (Decode.object (fun get -> { Body = get.Required.Field "body" (Decode.list Decode.string) }))

type Page =
    | Home
    | SignIn
    | SignUp
    | Article of string
    | Profile of string

type NavigablePage = Page of Page

type State = { Page: Page; User: User option }

type Message =
    | NavigateTo of Page
    | SuccessfulLogin of User
    | UnsuccessfulLogin of exn
    | SignUp of NewUser * (NewUser -> Promise<User>)

let init () =
    { State.Page = Page.Home; User = None }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none
    | SuccessfulLogin user -> { state with User = Some user }, Cmd.ofMsg (NavigateTo Page.Home)
    | UnsuccessfulLogin errors -> state, Cmd.none
    | SignUp (newUser, apiFn) ->
        let successFn response = SuccessfulLogin response
        let errorFn error = UnsuccessfulLogin error

        state, Cmd.OfPromise.either apiFn newUser (fun r -> SuccessfulLogin r) (fun e -> UnsuccessfulLogin e)

let navigateTo dispatch page = NavigateTo page |> dispatch
