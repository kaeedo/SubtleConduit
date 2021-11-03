module App

open Sutil
open Sutil.Program
open SubtleConduit.Services
open SubtleConduit.Components.Header
open SubtleConduit.Pages.Home
open SubtleConduit.Pages.SignIn
open SubtleConduit.Pages.SignUp
open SubtleConduit.Pages.Profile
open Sutil.DOM
open SubtleConduit.Types
open SubtleConduit.Router


let view () =
    let model, dispatch = Store.makeElmish init update ignore ()
    let navigateTo = navigateTo dispatch

    Router.on "/" (fun _ -> navigateTo Home) |> ignore

    Router.on "/signin" (fun _ -> navigateTo SignIn)
    |> ignore

    Router.on "/signup" (fun _ -> navigateTo SignUp)
    |> ignore

    Router.on "/profile/:username" (fun (matchProfile: Match<Articles.Articles.Author, _> option) ->
        match matchProfile with
        | Some mtc ->
            match mtc.data with
            | Some profile ->
                promise {
                    let! profile = Api.getProfile profile.username
                    navigateTo <| Profile profile
                }
                |> Promise.start
            | None -> navigateTo Home
        | None -> navigateTo Home)
    |> ignore

    Router
        .notFound(fun _ -> navigateTo Home)
        .resolve ()

    let page: NavigablePage =
        let location = getCurrentLocation ()

        match location with
        | [||] -> Page <| Page.Home
        | [| "signin" |] -> Page <| Page.SignIn
        | [| "signup" |] -> Page <| Page.SignUp
        | [| "profile"; username |] ->
            EventualPage
            <| promise {
                let! profile = Api.getProfile username
                return Page.Profile profile
               }
        | _ -> Page <| Page.Home

    match page with
    | Page p -> navigateTo p
    | EventualPage p ->
        promise {
            let! page = p
            navigateTo page
        }
        |> Promise.start

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
                | SignIn -> SignInPage dispatch
                | SignUp -> SignUpPage dispatch
                | Profile p -> ProfilePage p
        )
    ]

view () |> mountElement "sutil-app"
