module SubtleConduit.Router

open System
open Browser.Types
open SubtleConduit.Types

let parseHash (location: Location) =
    let hash =
        if location.hash.Length > 1 then
            location.hash.Substring 1
        else
            String.Empty

    if hash.Contains("?") then
        let h = hash.Substring(0, hash.IndexOf("?"))
        h, hash.Substring(h.Length + 1)
    else
        hash, String.Empty

let parseUrl (location: Location) = parseHash location

let parseRoute (location: Location) : Page =
    let hash, query = parseUrl location

    match Page.Find hash with
    | Some p -> p
    | None -> Home
