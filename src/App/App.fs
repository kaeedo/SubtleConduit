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
    let parseUrl (location: Location) =
        let url = (location.hash).Split("/").[1..2]
        let page = url[0]
        let parameter = url[1]
        // 8D211B52-2900-48B1-8906-60994AFB09D7@test.com

        match page, parameter with
        | "signin", _ ->
            Fable.Core.JS.console.log "Parsed signing"
            Page.SignIn
        | "signup", _ ->
            Fable.Core.JS.console.log "Parsed sign up"
            Page.SignUp
        | "settings", x ->
            Fable.Core.JS.console.log "Parsed seettinggs"
            Fable.Core.JS.console.log x
            Page.Settings
        | "editor", "" ->
            Fable.Core.JS.console.log "Parsed editor empty"
            Page.NewArticle String.Empty
        | "editor", slug ->
            Fable.Core.JS.console.log "Parsed editor"
            Fable.Core.JS.console.log slug
            Page.NewArticle slug
        | "article", slug ->
            Fable.Core.JS.console.log "Parsed article"
            Fable.Core.JS.console.log slug
            Page.Article slug
        | "profile", username ->
            Fable.Core.JS.console.log "Parsed profile"
            Fable.Core.JS.console.log username
            Page.Profile username
        | x ->
            Fable.Core.JS.console.log "Parsed other"
            Fable.Core.JS.console.log x
            Page.Home

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
