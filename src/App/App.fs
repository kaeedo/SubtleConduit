module App

open Sutil

open Sutil.Program
open SubtleConduit.Components.Header
open SubtleConduit.Pages.Home
open SubtleConduit.Pages.SignIn
open SubtleConduit.Pages.SignUp
open Sutil.DOM
open SubtleConduit.Types
open SubtleConduit.Router

// let viewPage model dispatch page =
//     match page with
//     | Home -> HomePage()
//     | SignIn -> SignInPage dispatch
//     | SignUp -> SignUpPage dispatch

let view () =

    let model, dispatch = Store.makeElmish init update ignore ()
    let navigateTo = navigateTo dispatch

    Router.on "/" (fun _ -> navigateTo Home) |> ignore

    Router.on "/signin" (fun _ -> navigateTo SignIn)
    |> ignore

    Router.on "/signup" (fun _ -> navigateTo SignUp)
    |> ignore

    Router.on "profile/:username" (fun (matchProfile: Match<ProfileData, _> option) ->
        match matchProfile with
        | Some mtc ->
            match mtc.data with
            | Some profile ->
                let username = profile.username
                navigateTo <| Profile username
            | None -> navigateTo Home
        | None -> navigateTo Home)
    |> ignore

    Router
        .notFound(fun _ -> navigateTo Home)
        .resolve ()

    let page =
        let location = getCurrentLocation ()

        match location with
        | [||] -> Page.Home
        | [| "signin" |] -> Page.SignIn
        | [| "signup" |] -> Page.SignUp
        | [| "profile"; username |] -> Page.Profile username
        | _ -> Page.Home

    navigateTo page

    fragment [
        disposeOnUnmount [
            model
        ]
        Header(model, dispatch)

        Bind.el (
            model,
            fun m ->
                match m.Page with
                | Page.Home -> HomePage()
                | SignIn -> SignInPage dispatch
                | SignUp -> SignUpPage dispatch
                | Profile p -> HomePage()
                | _ -> HomePage()
        )
    ]

view () |> mountElement "sutil-app"
