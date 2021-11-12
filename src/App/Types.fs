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


type Page =
    | Home
    | SignIn
    | SignUp
    | Article of string
    | Profile of string

type NavigablePage = Page of Page

type State = { Page: Page }

type Message = NavigateTo of Page

let init () = { State.Page = Page.Home }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none

let navigateTo dispatch page = NavigateTo page |> dispatch
