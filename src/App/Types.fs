module SubtleConduit.Types

open System
open Thoth.Json
open SubtleConduit.Utilities

let inline private encoder<'T> =
    Encode.Auto.generateEncoderCached<'T> (caseStrategy = CamelCase)

let inline private decoder<'T> extras =
    Decode.Auto.generateDecoderCached<'T> (caseStrategy = CamelCase, extra = extras)

type Tags =
    {
        Tags: string list
    }

    static member fromJson(json: string) : Result<Tags, string> =
        Decode.Auto.fromString<Tags> (json, caseStrategy = CamelCase)

type User =
    {
        Email: string
        Token: string
        Username: string
        Bio: string
        Image: string
        Following: bool
    }

    static member Encoder = encoder<{| User: User |}>

    static member Decoder: Decoder<User> =
        Decode.object (fun get -> {
            User.Username = get.Required.Field "username" Decode.string
            User.Email = get.Required.Field "email" Decode.string
            User.Token = get.Required.Field "token" Decode.string
            Bio =
                get.Optional.Field "bio" Decode.string
                |> Option.defaultValue ""
            Image =
                get.Optional.Field "image" Decode.string
                |> Option.defaultValue ""
            Following =
                get.Optional.Field "following" Decode.bool
                |> Option.defaultValue false
        })

    static member fromJson(json: string) =
        Decode.unsafeFromString (Decode.field "user" User.Decoder) json

    member x.toJson() =
        Encode.Auto.toString (0, {| User = x |}, caseStrategy = CamelCase)

type Profile =
    {
        Username: string
        Bio: string
        Image: string
        Following: bool
    }

    static member Encoder = encoder<Profile>

    static member Decoder: Decoder<Profile> =
        Decode.object (fun get -> {
            Profile.Username = get.Required.Field "username" Decode.string
            Bio =
                get.Optional.Field "bio" Decode.string
                |> Option.defaultValue ""
            Image =
                get.Optional.Field "image" Decode.string
                |> Option.defaultValue ""
            Following =
                get.Optional.Field "following" Decode.bool
                |> Option.defaultValue false
        })

    static member fromJson(json: string) =
        Decode.unsafeFromString (Decode.field "profile" Profile.Decoder) json

type Article =
    {
        Slug: string
        Title: string
        Description: string
        Body: string
        CreatedAt: DateTime
        UpdatedAt: DateTime
        TagList: string list
        Favorited: bool
        FavoritesCount: int
        Author: Profile
    }

    static member Encoder = encoder<Article>

    static member Decoder: Decoder<Article> =
        let extras =
            Extra.empty
            |> Extra.withCustom Profile.Encoder (Profile.Decoder)

        decoder<Article> extras

    static member fromJson(json: string) =
        let decoder = Decode.field "article" Article.Decoder


        Decode.unsafeFromString decoder json

type Articles =
    {
        Articles: Article list // TODO Refactor to array
        ArticlesCount: int
    }

    static member Encoder = encoder<Articles>

    static member Decoder: Decoder<Articles> =
        let extras =
            Extra.empty
            |> Extra.withCustom Article.Encoder (Article.Decoder)

        decoder<Articles> extras

    static member fromJson(json: string) =
        Decode.unsafeFromString Articles.Decoder json

type UpsertUser =
    {
        Username: string
        Image: string
        Bio: string
        Token: string option
        Email: string
        Password: string
    }

    static member Encoder(user: UpsertUser) =
        let userEncoder =
            Encode.object [
                "username", Encode.string user.Username
                "image", Encode.option Encode.string (Option.ofString user.Image)
                "bio", Encode.option Encode.string (Option.ofString user.Bio)
                "email", Encode.string user.Email
                if not (String.IsNullOrWhiteSpace user.Password) then
                    "password", Encode.string user.Password
                if user.Token.IsSome then
                    "token", Encode.option Encode.string user.Token
            ]

        Encode.object [ "user", userEncoder ]

    static member Decoder = decoder<UpsertUser> Extra.empty

    member this.toJson() =
        Encode.toString 0 (UpsertUser.Encoder this)

type UpsertArticle =
    {
        Title: string
        Description: string
        Body: string
        TagList: string list
        Token: string
    }

    static member Encoder(article: UpsertArticle) =
        let articleEncoder =
            Encode.object [
                if not (String.IsNullOrWhiteSpace article.Title) then
                    "title", Encode.string article.Title
                if not (String.IsNullOrWhiteSpace article.Description) then
                    "description", Encode.string article.Description
                if not (String.IsNullOrWhiteSpace article.Body) then
                    "body", Encode.string article.Body
                if not (String.IsNullOrWhiteSpace article.Token) then
                    "token", Encode.string article.Token

                if article.TagList |> List.isEmpty |> not then
                    "tagList", Encode.list (article.TagList |> List.map (Encode.string))
            ]

        Encode.object [ "article", articleEncoder ]

    static member Decoder = decoder<UpsertArticle> Extra.empty

    member this.toJson() =
        Encode.toString 0 (UpsertArticle.Encoder this)

type ApiErrors =
    {
        Body: string list
    } // TODO refactor this to array

    static member Decoder =
        Decode.field
            "errors"
            (Decode.object (fun get -> {
                Body = get.Required.Field "body" (Decode.list Decode.string)
            }))
