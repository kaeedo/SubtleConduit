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

let viewPage model dispatch page =
    match page with
    | Home -> HomePage()
    | SignIn -> SignInPage dispatch
    | SignUp -> SignUpPage dispatch

let view () =

    let model, dispatch = Store.makeElmish init update ignore ()

    let page =
        model
        |> Store.map (fun s -> s.Page)
        |> Store.distinct

    let routerSubscription =
        Navigable.listenLocation parseRoute (SetPage >> dispatch)

    fragment [
        disposeOnUnmount [ model ]
        unsubscribeOnUnmount [
            routerSubscription
        ]
        Header(model, dispatch)

        Bind.el (page, (viewPage model dispatch))
    ]

view () |> mountElement "sutil-app"
