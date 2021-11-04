module SubtleConduit.Types

open Sutil
open Fable.Core.JS

[<Literal>]
let TAGS = "./services/TagsSampleResponse.json"

[<Literal>]
let ARTICLES = "./services/ArticlesSampleResponse.json"

[<Literal>]
let ARTICLE = "./services/ArticleSampleResponse.json"

[<Literal>]
let PROFILE = "./services/ProfileSampleResponse.json"

type Tags = Fable.JsonProvider.Generator<TAGS>
type Articles = Fable.JsonProvider.Generator<ARTICLES>
type Article = Fable.JsonProvider.Generator<ARTICLE>
type Profile = Fable.JsonProvider.Generator<PROFILE>

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
