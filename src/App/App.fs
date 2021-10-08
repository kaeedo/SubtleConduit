module App

open System
open Sutil
open Sutil.DOM
open Sutil.Attr
open Feliz
open type Feliz.length
open Sutil.Program

let view() =
    Html.div [
        class' "container mx-auto flex flex-row justify-between py-2"
        text "Hello World"
    ]

view() |> mountElement "sutil-app"