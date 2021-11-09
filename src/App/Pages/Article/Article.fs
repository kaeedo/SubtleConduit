module SubtleConduit.Pages.Article

// open Fable.Core
// open Fable.Core.JsInterop
// open Feliz
// open Feliz.UseElmish
// open Elmish

open System
open Sutil
open SubtleConduit.Services.Api
open SubtleConduit.Types
open Tailwind
open Sutil.DOM
open SubtleConduit.Components

type ArticleState =
    { Title: string
      Description: string
      Body: string
      CreatedAt: DateTime
      Tags: string list
      Favorited: bool
      FavoritesCount: int
      Author: Profile }

type ArticleMsg =
    | GetArticle of string
    | Set of Article
    | Error of string

let init _ =
    { ArticleState.Title = ""
      Description = ""
      Body = ""
      CreatedAt = new DateTime()
      Tags = []
      Favorited = false
      FavoritesCount = 0
      Author =
        { Profile.Username = None
          Bio = None
          Image = None
          Following = None } },
    Cmd.none

let mapResultToArticleState (result: Article) =
    { ArticleState.Title = result.Title
      Description = result.Description
      Body = result.Body
      CreatedAt = result.CreatedAt
      Tags =
        match result.Tags with
        | Some t -> t
        | None -> []
      Favorited = result.Favorited
      FavoritesCount = result.FavoritesCount
      Author = result.Author }

let update msg state =
    let setResult result =
        let (Ok result) = result
        Set result

    match msg with
    | GetArticle slug -> state, Cmd.OfPromise.either getArticle slug setResult (fun _ -> Error "error")
    | Set result ->
        let newState = mapResultToArticleState result
        newState, Cmd.none
    | Error e -> state, Cmd.none


let ArticlePage (slug: string) =
    let state, dispatch = Store.makeElmish init update ignore ()

    let view =
        Html.div [
            disposeOnUnmount [
                state
            ]
            Bind.el (state, (fun s -> text s.Body))
        ]

    view
