module SubtleConduit.Components.Feed

open Sutil
open Sutil.Attr
open SubtleConduit.Utilities

open SubtleConduit.Types
open SubtleConduit.Services.Api.ArticleApi
open Sutil.DOM
open System
open SubtleConduit.Elmish
open SubtleConduit.Components.FeedItems
open SubtleConduit.Components.FeedTab


type TabsToShow =
    | Home of isLoggedIn: bool * tag: string option
    | MyProfile
    | OtherProfile

type SelectedTab =
    | Feed
    | Articles
    | Tag
    | MyPosts
    | FavoritedPosts

let Feed (model: State) (dispatch: Dispatch<Message>) (articleFilter: ArticleFilter option) (setArticleFilter) =
    let pageSize = 10
    let offset = Store.make 0

    let tabsToShow =
        Store.make (
            match model.Page, model.User with
            | Page.Home, _ ->
                let tag =
                    match articleFilter with
                    | Some(ArticleFilter.Tag t) -> Some t
                    | _ -> None

                TabsToShow.Home(model.User.IsSome, tag)
            | Page.Profile p, Some u when p = u.Username -> TabsToShow.MyProfile
            | Page.Profile _, _ -> TabsToShow.OtherProfile
            | _ -> TabsToShow.Home(false, None)
        )

    let articles = ObservablePromise<Articles>()

    let selectedTab =
        Store.make (
            match model.Page with
            | Page.Home ->
                match articleFilter with
                | Some(ArticleFilter.Tag t) -> SelectedTab.Tag
                | _ -> SelectedTab.Articles
            | _ -> SelectedTab.MyPosts
        )

    let getArticles user pageSize newOffset filter showFeed =
        articles.Run
        <| promise {
            try
                let! articlesFromApi = getArticles user pageSize newOffset filter showFeed
                offset <~ newOffset
                return articlesFromApi
            with :? exn ->
                //failwith exn
                return Unchecked.defaultof<Articles>
        // TODO handle error case
        }

    let tabSubscription =
        selectedTab
        |> Store.subscribe (fun st ->
            let articleFilter =
                match st with
                | SelectedTab.Tag -> articleFilter
                | SelectedTab.MyPosts -> articleFilter
                | SelectedTab.FavoritedPosts ->
                    let (Profile currentProfile) = model.Page
                    ArticleFilter.Favorited currentProfile |> Some
                | _ -> None

            let showFeed = st = SelectedTab.Feed

            getArticles model.User pageSize offset.Value articleFilter showFeed)

    let currentPage = offset .> fun o -> (o / pageSize) + 1

    let pageNumbers = // TODO refactor to array
        let total =
            articles
            .> (function
            | Result a ->
                Math.Ceiling(float a.ArticlesCount / float pageSize)
                |> int
            | _ -> 0)

        Store.zip currentPage total
        .> (fun (current, total) -> getPagesToDisplay current total)


    let favoriteArticle slug isFavorited _ =
        match model.User with
        | None -> ()
        | Some u ->
            promise {
                let! _ = favoriteArticle slug isFavorited u.Token
                // TODO Ask how to update single value in observable promise
                getArticles model.User pageSize offset.Value articleFilter false
                return ()
            }
            |> Promise.start

    let tabs () =
        Html.div [
            Attr.classes [ "flex" ]
            Bind.el (
                tabsToShow,
                fun t ->
                    match t with
                    | TabsToShow.Home(isLoggedIn, tag) ->
                        fragment [
                            if isLoggedIn then
                                FeedTab
                                    "Your Feed"
                                    (fun _ -> selectedTab <~ SelectedTab.Feed)
                                    (selectedTab .> fun st -> st = SelectedTab.Feed)
                            FeedTab
                                "Global Feed"
                                (fun _ -> selectedTab <~ SelectedTab.Articles)
                                (selectedTab .> fun st -> st = SelectedTab.Articles)
                            match tag with
                            | Some t ->
                                FeedTab
                                    t
                                    (fun _ -> selectedTab <~ SelectedTab.Tag)
                                    (selectedTab .> fun st -> st = SelectedTab.Tag)
                            | None -> ()
                        ]
                    | TabsToShow.MyProfile ->
                        fragment [
                            FeedTab
                                "My Posts"
                                (fun _ -> selectedTab <~ SelectedTab.MyPosts)
                                (selectedTab .> fun st -> st = SelectedTab.MyPosts)
                            FeedTab
                                "Favorited Posts"
                                (fun _ -> selectedTab <~ SelectedTab.FavoritedPosts)
                                (selectedTab
                                 .> fun st -> st = SelectedTab.FavoritedPosts)
                        ]
                    | TabsToShow.OtherProfile ->
                        fragment [
                            FeedTab
                                "Posts"
                                (fun _ -> selectedTab <~ SelectedTab.MyPosts)
                                (selectedTab .> fun st -> st = SelectedTab.MyPosts)
                            FeedTab
                                "Favorited Posts"
                                (fun _ -> selectedTab <~ SelectedTab.FavoritedPosts)
                                (selectedTab
                                 .> fun st -> st = SelectedTab.FavoritedPosts)
                        ]
            )
        ]

    let view =
        Html.div [
            onMount (fun _ -> getArticles model.User pageSize 0 articleFilter false) [ Once ]
            disposeOnUnmount [
                offset
                selectedTab
                tabsToShow
                tabSubscription
            ]

            Attr.classes [ "flex-auto"; "mr-6" ]
            tabs ()
            FeedItems articles favoriteArticle setArticleFilter
            Html.ul [
                Attr.classes [ "flex"; "justify-center" ]
                Bind.el (
                    pageNumbers,
                    fun pns ->
                        let lastPage = pns.[pns.Length - 1]

                        fragment [
                            Html.li [
                                Attr.classes [
                                    "cursor-pointer"
                                    "px-3"
                                    "py-2"
                                    "border"
                                    "border-gray-300"
                                    "border-r-0"
                                    "rounded-l-sm"
                                ]
                                onClick
                                    (fun _ ->
                                        if offset.Value = 0 then
                                            ()
                                        else
                                            getArticles
                                                model.User
                                                pageSize
                                                (offset.Value - pageSize)
                                                articleFilter
                                                false)
                                    []
                                text "<"
                            ]

                            for pn in pns do
                                Html.li [
                                    Bind.toggleClass (
                                        (currentPage .> fun cp -> (cp.ToString()) = pn),
                                        "bg-conduit-green" + " " + "text-white",
                                        "hover:bg-gray-100"
                                    )
                                    Attr.classes [
                                        "cursor-pointer"
                                        "px-3"
                                        "py-2"
                                        "border"
                                        "border-gray-300"
                                        "border-r-0"
                                    ]
                                    onClick
                                        (fun _ ->
                                            getArticles model.User pageSize (int pn * pageSize - 10) articleFilter false)
                                        []
                                    text pn
                                ]

                            Html.li [
                                Attr.classes [
                                    "cursor-pointer"
                                    "px-3"
                                    "py-2"
                                    "border"
                                    "border-gray-300"
                                    "rounded-r-sm"
                                ]
                                onClick
                                    (fun _ ->
                                        if (offset.Value / pageSize + 1) = int lastPage then
                                            ()
                                        else
                                            getArticles
                                                model.User
                                                pageSize
                                                (offset.Value + pageSize)
                                                articleFilter
                                                false)

                                    []

                                text ">"
                            ]
                        ]
                )
            ]
        ]

    view
