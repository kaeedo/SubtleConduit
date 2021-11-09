module SubtleConduit.Types

open System
open Sutil
open Fable.Core.JS
open Thoth.Json


type Tags =
    { Tags: string list }
    static member fromJson(json: string) : Result<Tags, string> =
        Decode.Auto.fromString<Tags> (json, caseStrategy = CamelCase)

type Profile =
    { Username: string option
      Bio: string option
      Image: string option
      Following: bool option }
    static member fromJson(json: string) =
        Decode.Auto.fromString<Profile> (json, caseStrategy = CamelCase)

type Article =
    { Slug: string
      Title: string
      Description: string
      Body: string
      CreatedAt: DateTime
      UpdatedAt: DateTime
      Tags: string list option
      Favorited: bool
      FavoritesCount: int
      Author: Profile }
    static member fromJson(json: string) =
        Decode.Auto.fromString<Article> (json, caseStrategy = CamelCase)

type Articles =
    { Articles: Article list // TODO Refactor to array
      ArticlesCount: int }
    static member fromJson(json: string) =
        Decode.Auto.fromString<Articles> (json, caseStrategy = CamelCase)


type Page =
    | Home
    | SignIn
    | SignUp
    | Article of string
    | Profile of Profile

type NavigablePage =
    | Page of Page
    | EventualPage of Promise<Page>

type State = { Page: Page }

type Message = NavigateTo of Page

let init () = { State.Page = Page.Home }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none

let navigateTo dispatch page = NavigateTo page |> dispatch
