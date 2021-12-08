module SubtleConduit.Elmish

open Types
open Sutil
open SubtleConduit.Services.Api
open LocalStorage

type Page =
    | Home
    | SignIn
    | SignUp
    | Settings
    | NewArticle of string
    | Article of string
    | Profile of string

type NavigablePage = Page of Page

type State = { Page: Page; User: User option }

type Message =
    | NavigateTo of Page
    | SuccessfulLogin of User
    | UnsuccessfulLogin of exn
    | SignUp of UpsertUser
    | UpdateUser of UpsertUser
    | SignIn of string * string
    | Logout

let private init () =
    let user =
        LocalStorage.tryGetItem SessionStorageKeys.User
        |> Option.map (User.fromJson)

    { State.Page = Page.Home; User = user }, Cmd.none

let private update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none
    | SuccessfulLogin user -> { state with User = Some user }, Cmd.ofMsg (NavigateTo Page.Home)
    | UnsuccessfulLogin errors -> state, Cmd.none
    | SignUp upsertUser ->
        let successFn (response: User) =
            LocalStorage.setItem SessionStorageKeys.User (response.toJson ())
            SuccessfulLogin response

        let errorFn error = UnsuccessfulLogin error

        state, Cmd.OfPromise.either ProfileApi.signUp upsertUser successFn (fun e -> UnsuccessfulLogin e)
    | SignIn (email, password) ->
        let credentials = (email, password)

        let successFn (response: User) =
            LocalStorage.setItem SessionStorageKeys.User (response.toJson ())
            SuccessfulLogin response

        let errorFn error = UnsuccessfulLogin error

        state, Cmd.OfPromise.either ProfileApi.signIn credentials successFn (fun e -> UnsuccessfulLogin e)
    | UpdateUser upsertUser ->
        // TODO Create update user api call
        let successFn (response: User) =
            LocalStorage.setItem SessionStorageKeys.User (response.toJson ())
            SuccessfulLogin response

        let errorFn error = UnsuccessfulLogin error

        state, Cmd.OfPromise.either ProfileApi.updateUser upsertUser successFn (fun e -> UnsuccessfulLogin e)
    | Logout ->
        LocalStorage.removeItem SessionStorageKeys.User

        { state with User = None }, Cmd.ofMsg (NavigateTo Page.Home)

let navigateTo dispatch page = NavigateTo page |> dispatch
let elmishStore = Store.makeElmish init update ignore ()
