module SubtleConduit.LocalStorage

open Fable.Core

[<StringEnum>]
type LocalStorageKeys =
| User


let setItem (key: LocalStorageKeys) item =
    Browser.WebStorage.localStorage.setItem(key.ToString(), item)

let tryGetItem (key: LocalStorageKeys) =
    if Browser.WebStorage.localStorage.getItem (key.ToString()) = Fable.Core.JS.undefined
    then None
    else Some <| Browser.WebStorage.localStorage.getItem(key.ToString())

let removeItem (key: LocalStorageKeys) =
    Browser.WebStorage.localStorage.removeItem(key.ToString())
