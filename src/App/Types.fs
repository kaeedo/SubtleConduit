module SubtleConduit.Types

open System
open Sutil
open Fable.Core.JS

[<Literal>]
let TAGS = "./services/TagsSampleResponse.json"

[<Literal>]
let ARTICLES = "./services/ArticlesSampleResponse.json"

[<Literal>]
let ARTICLE = "./services/ArticleSampleResponse.json"

[<Literal>]
let PROFILE = "./services/ProfileSampleResponse.json"

type JsonTags = Fable.JsonProvider.Generator<TAGS>

type Tags =
    { Tags: string list } // TODO: Refactor to array
    static member fromJsonTags(json: JsonTags) = { Tags = json.tags |> List.ofArray }

type JsonProfile = Fable.JsonProvider.Generator<PROFILE>

type Profile =
    { Username: string
      Bio: string option
      Image: string
      Following: bool }
    static member fromJsonProfile(json: JsonProfile) =
        { Profile.Username = json.profile.username
          Bio =
            if json.profile.bio <> null then
                Some(json.profile.bio :?> string)
            else
                None
          Image = json.profile.image
          Following = json.profile.following }

type JsonArticle = Fable.JsonProvider.Generator<ARTICLE>

type Article =
    { Slug: string
      Title: string
      Description: string
      Body: string
      CreatedAt: DateTime
      UpdatedAt: DateTime
      Tags: string list
      Favorited: bool
      FavoritesCount: int
      Author: Profile }
    static member fromJsonArticle(json: JsonArticle) =
        let profile =
            { Profile.Username = json.article.author.username
              Bio =
                if json.article.author.bio <> null then
                    Some(json.article.author.bio :?> string)
                else
                    None
              Image = json.article.author.image
              Following = json.article.author.following }

        { Article.Slug = json.article.slug
          Title = json.article.title
          Description = json.article.description
          Body = json.article.body
          CreatedAt = DateTime.Parse(json.article.createdAt)
          UpdatedAt = DateTime.Parse(json.article.updatedAt)
          Tags =
            json.article.tagList
            |> Seq.cast<string>
            |> List.ofSeq
          Favorited = json.article.favorited
          FavoritesCount = json.article.favoritesCount |> int
          Author = profile }

type JsonArticles = Fable.JsonProvider.Generator<ARTICLES>

type Articles =
    { Articles: Article list // TODO Refactor to array
      ArticlesCount: int }
    static member fromJsonArticles(json: JsonArticles) =
        let articles =
            json.articles
            |> Seq.cast<JsonArticle>
            |> Seq.map (fun a -> Article.fromJsonArticle (a))
            |> List.ofSeq

        { Articles.Articles = articles
          ArticlesCount = json.articlesCount |> int }


type Page =
    | Home
    | SignIn
    | SignUp
    | Article of string
    | Profile of Profile

type NavigablePage =
    | Page of Page
    | EventualPage of Promise<Page>

type State = { Page: Page }

type Message = NavigateTo of Page

let init () = { State.Page = Page.Home }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none

let navigateTo dispatch page = NavigateTo page |> dispatch
