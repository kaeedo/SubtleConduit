module App

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
open Sutil.DOM
open SubtleConduit.Router
open SubtleConduit.Pages.NewArticle


let view () =
    let model, dispatch = elmishStore
    let navigateTo = navigateTo dispatch

    Router.on "/" (fun _ -> navigateTo Home) |> ignore

    Router.on "/signin" (fun _ -> navigateTo Page.SignIn)
    |> ignore

    Router.on "/signup" (fun _ -> navigateTo Page.SignUp)
    |> ignore

    Router.on "/settings" (fun _ -> navigateTo Page.Settings)
    |> ignore

    Router.on "/editor" (fun _ -> navigateTo Page.NewArticle)
    |> ignore

    Router.on "/article/:slug" (fun (matchSlug: Match<{| slug: string |}, _> option) ->
        match matchSlug with
        | Some mtc ->
            match mtc.data with
            | Some slug -> navigateTo <| Article slug.slug
            | None -> navigateTo Home
        | None -> navigateTo Home)
    |> ignore

    Router.on "/profile/:username" (fun (matchProfile: Match<{| username: string |}, _> option) ->
        // when navigation
        match matchProfile with
        | Some mtc ->
            match mtc.data with
            | Some username -> navigateTo <| Profile username.username
            | None -> navigateTo Home
        | None -> navigateTo Home)
    |> ignore

    Router
        .notFound(fun _ -> navigateTo Home)
        .resolve ()

    let page: NavigablePage =
        // When loading url from blank (or refresh)
        let location = getCurrentLocation ()

        match location with
        | [||] -> Page <| Page.Home
        | [| "signin" |] -> Page <| Page.SignIn
        | [| "signup" |] -> Page <| Page.SignUp
        | [| "settings" |] -> Page <| Page.Settings
        | [| "editor" |] -> Page <| Page.NewArticle
        | [| "article"; slug |] -> Page <| Page.Article slug
        | [| "profile"; username |] -> Page <| Page.Profile username
        | _ -> Page <| Page.Home

    match page with
    | Page p -> navigateTo p

    fragment [
        disposeOnUnmount [
            model
        ]
        Header(model, dispatch)

        Bind.el (
            model,
            fun m ->
                match m.Page with
                | Page.Home -> HomePage dispatch
                | Page.SignIn -> SignInPage dispatch
                | Page.SignUp -> SignUpPage dispatch
                | Page.Settings -> SettingsPage m dispatch
                | Page.NewArticle -> NewArticlePage m
                | Article a -> ArticlePage a
                | Profile p -> ProfilePage p
        )
    ]

view () |> mountElement "sutil-app"
