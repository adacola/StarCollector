module Adacola.StarCollector.BlackAndWhiteList

open System.Text.RegularExpressions

let parse (blackAndWhiteList : ListProvider.Root) =
    let excludes maybeRegex maybeString text =
        match maybeRegex, maybeString with
        | Some r, _ -> Regex.IsMatch(text, r)
        | None, Some s -> text.Contains s
        | None, None -> false
    let blackList = blackAndWhiteList.BlackList |> Array.map (fun x -> excludes x.Regex x.String)
    let whiteList = blackAndWhiteList.WhiteList |> Array.map (fun x -> excludes x.Regex x.String)
    fun text -> (blackList |> Array.exists ((|>) text)) && (whiteList |> Array.exists ((|>) text) |> not)
    