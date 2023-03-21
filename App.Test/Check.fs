[<Microsoft.FSharp.Core.AutoOpen>]
module App.Test.Check

open System
open Microsoft.FSharp.Quotations.Patterns
open Swensen.Unquote

let testWithReason expression reason =
    try
        Assertions.test expression
    with e ->
        let diff =
            match expression with
            | Call(_, methodInfo, [ a; b ]) when methodInfo.Name = "op_Equality" ->
                let a = a.Eval()
                let b = b.Eval()

                let diffMethod =
                    typeof<DEdge.Diffract.Differ>
                        .GetMethod(nameof DEdge.Diffract.Differ.Diff)
                        .MakeGenericMethod [| a.GetType() |]

                let diff: DEdge.Diffract.Diff option =
                    downcast diffMethod.Invoke(null, [| b; a; null |])

                let diffString = DEdge.Diffract.Differ.ToString diff
                $"\nDiff =\n{diffString}"
            | _ -> String.Empty

        raise (
            Exception(
                reason
                + diff
                + "\n----------------------------\nUnquote Message:\n"
                + e.Message
            )
        )

let test expression = testWithReason expression String.Empty
