module SubtleConduit.LocalStorage

open Fable.Core

[<StringEnum>]
type SessionStorageKeys = | User


let setItem (key: SessionStorageKeys) item =
    Browser.WebStorage.sessionStorage.setItem (key.ToString(), item)

let tryGetItem (key: SessionStorageKeys) =
    if Browser.WebStorage.sessionStorage.getItem (key.ToString()) = Fable.Core.JS.undefined then
        None
    else
        Some
        <| Browser.WebStorage.sessionStorage.getItem (key.ToString())

let removeItem (key: SessionStorageKeys) =
    Browser.WebStorage.sessionStorage.removeItem (key.ToString())
