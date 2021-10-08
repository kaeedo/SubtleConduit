module App

open Sutil

open Sutil.Program
open SubtleConduit.Components.Header
open SubtleConduit.Pages.Home

let view () = 
    Html.div [ 
        Header()
        HomePage()
    ]

view () |> mountElement "sutil-app"
