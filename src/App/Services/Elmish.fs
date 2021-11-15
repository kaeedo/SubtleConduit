module SubtleConduit.Elmish

open Types
open Sutil
open SubtleConduit.Services
open LocalStorage

type Page =
    | Home
    | SignIn
    | SignUp
    | Article of string
    | Profile of string

type NavigablePage = Page of Page

type State = { Page: Page; User: User option }

type Message =
    | NavigateTo of Page
    | SuccessfulLogin of User
    | UnsuccessfulLogin of exn
    | SignUp of NewUser

let init () =
    let user =
        LocalStorage.tryGetItem LocalStorageKeys.User
        |> Option.map (User.fromJson)
    { State.Page = Page.Home; User = user }, Cmd.none

let update (msg: Message) (state: State) =
    match msg with
    | NavigateTo page -> { state with Page = page }, Cmd.none
    | SuccessfulLogin user ->
        { state with User = Some user }, Cmd.ofMsg (NavigateTo Page.Home)
    | UnsuccessfulLogin errors -> state, Cmd.none
    | SignUp newUser ->
        let successFn (response: User) =
            LocalStorage.setItem LocalStorageKeys.User (response.toJson())
            SuccessfulLogin response
        let errorFn error = UnsuccessfulLogin error

        state, Cmd.OfPromise.either Api.signUp newUser successFn (fun e -> UnsuccessfulLogin e)

let navigateTo dispatch page = NavigateTo page |> dispatch
