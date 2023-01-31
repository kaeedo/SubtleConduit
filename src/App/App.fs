module App

open Browser.Types
open Fable.Core.JsInterop
open System
open Sutil
open Sutil.Program
open SubtleConduit.Elmish
open SubtleConduit.Components.Header
open SubtleConduit.Pages.Home
open SubtleConduit.Pages.SignIn
open SubtleConduit.Pages.SignUp
open SubtleConduit.Pages.Settings
open SubtleConduit.Pages.Article
open SubtleConduit.Pages.Profile
open Sutil.CoreElements
open SubtleConduit.Pages.NewArticle

importSideEffects "virtual:windi.css"

module UrlParser =
    let parseHash (location: Location) =
        let hash =
            if location.hash.Length > 1 then
                location.hash.Substring 1
            else
                ""

        if hash.Contains("?") then
            let h = hash.Substring(0, hash.IndexOf("?"))
            h, hash.Substring(h.Length + 1)
        else
            hash, ""

    let parseUrl (location: Location) =
        Fable.Core.JS.console.log "parsed:"
        Fable.Core.JS.console.log (parseHash location)

        match parseHash location with
        | "/signin", _ -> Page.SignIn
        | "/signup", _ -> Page.SignUp
        | "/settings", _ -> Page.Settings
        | "/editor", "" -> Page.NewArticle String.Empty
        | "/editor", slug -> Page.NewArticle slug
        | "/article", slug -> Page.Article slug
        | "/profile", username -> Page.Profile username
        | _ -> Page.Home

let view () =
    let model, dispatch = elmishStore

    let navigationListener =
        Navigable.listenLocation (UrlParser.parseUrl, dispatch << NavigateTo)

    fragment [
        unsubscribeOnUnmount [ navigationListener ]
        disposeOnUnmount [ model ]
        Header(model, dispatch)

        Bind.el (
            model,
            fun m ->
                Fable.Core.JS.console.log (m.Page.ToString())

                match m.Page with
                | Page.Home -> HomePage m dispatch
                | Page.SignIn -> SignInPage dispatch
                | Page.SignUp -> SignUpPage dispatch
                | Page.Settings -> SettingsPage m dispatch
                | Page.NewArticle slug -> NewArticlePage m dispatch slug
                | Page.Article a -> ArticlePage m dispatch a
                | Page.Profile p -> ProfilePage m dispatch p
        )
    ]

view () |> mountElement "sutil-app"
