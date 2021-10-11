module SubtleConduit.Types

open System
open Sutil
open Browser.Dom

let private strCaseEq s1 s2 =
    String.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase)

type Page =
    | Home
    | SignIn
    | SignUp
    static member All =
        [ "Home", Home
          "SignIn", SignIn
          "SignUp", SignUp ]

    static member Find(name: string) =
        Page.All
        |> List.tryFind (fun (pageName, page) -> strCaseEq pageName name)
        |> Option.map snd

type Model = { Page: Page }

/////////////////////


type Message = SetPage of Page

let init () = { Model.Page = Home }, Cmd.none

let update msg model =
    match msg with
    | SetPage p ->
        window.location.href <- "#" + (string p).ToLower()
        { model with Page = p }, Cmd.none
