module SubtleConduit.Types

open System
open Sutil
open Browser.Dom

type Page =
    | Home
    | SignIn
    | SignUp
    | Profile of string

type ProfileData = { username: string }

type State = { Page: Page }

type Message = NavigateTo of Page

let init () = { State.Page = Page.Home }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none

let navigateTo dispatch page = NavigateTo page |> dispatch
//   | Library of string
//   | Docs of library: string * docSite: string
//   | NotFound

// let private strCaseEq s1 s2 =
//     String.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase)

// type Page =
//     | Home
//     | SignIn
//     | SignUp
//     | Profile of string
//     static member All =
//         [ "Home", Home
//           "SignIn", SignIn
//           "SignUp", SignUp
//           "Profile", Profile "" ]
//     // https://github.com/AngelMunoz/Sutil.Generator/blob/master/src/website/src/App.fs
//     static member Find(name: string) =
//         Page.All
//         |> List.tryFind (fun (pageName, page) -> strCaseEq pageName name)
//         |> Option.map snd

// type Model = { Page: Page }

// /////////////////////


// type Message = SetPage of Page

// let init () = { Model.Page = Home }, Cmd.none

// let update msg model =
//     match msg with
//     | SetPage (Profile userProfile) ->
//         window.location.href <- $"#profile/{userProfile}"
//         { model with Page = (Profile userProfile) }, Cmd.none
//     | SetPage p ->
//         window.location.href <- $"#{(string p).ToLower()}"
//         { model with Page = p }, Cmd.none
