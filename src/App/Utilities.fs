module SubtleConduit.Utilities

open System

let getPagesToDisplay currentPage totalPages = // TODO make this an array
    let delta =
        match totalPages with
        | x when x <= 8 -> 8
        | x when currentPage > 5 && currentPage < x - 4 -> 3
        | _ -> 5

    let (lower, upper) =
        Math.Floor(float currentPage - float delta / 2.0)
        |> int,
        Math.Ceiling(float currentPage + float delta / 2.0)
        |> int

    let (lower, upper) =
        if lower - 1 = 1 || upper + 1 = totalPages then
            lower + 1, upper + 1
        else
            lower, upper


    let pages =
        if currentPage > delta then
            let rangeLower = Math.Min(lower, totalPages - delta)
            let rangeUpper = Math.Min(upper, totalPages)

            [ rangeLower .. rangeUpper ]
            |> List.map (fun p -> p.ToString())
        else
            [ 1 .. (Math.Min(totalPages, delta + 1)) ]
            |> List.map (fun p -> p.ToString())

    let withDots value pair =
        if pages.Length + 1 <> totalPages then
            pair
        else
            [ value ]

    let pages =
        if pages.[0] <> "1" then
            withDots "1" [ "1"; "..." ] @ pages
        else
            pages

    let pages =
        if (int pages.[pages.Length - 1]) < totalPages then
            let dots =
                withDots (totalPages.ToString()) [ "..."; totalPages.ToString() ]

            pages @ dots
        else
            pages

    pages
