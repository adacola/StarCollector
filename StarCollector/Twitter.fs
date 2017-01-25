module Adacola.StarCollector.Twitter

open CoreTweet
open CoreTweet.Streaming
open System.Reactive.Linq

let authenticate (consumerKey : ConsumerKey) (accessToken : AccessToken) =
    Tokens.Create(consumerKey.ConsumerKey, consumerKey.ConsumerSecret, accessToken.AccessToken, accessToken.AccessTokenSecret)

let getTweetConnection (tokens : Tokens) =
    tokens.Streaming.UserAsObservable().Publish()

let (|MyTweet|MyRetweet|Tweet|OtherMessage|) (userId, (message : StreamingMessage)) =
    match message with
    | :? StatusMessage as m ->
        if m.Status.User.Id |> Option.ofNullable |> Option.exists ((=) userId) then
            if m.Status.RetweetedStatus |> isNull then MyTweet(m) else MyRetweet(m)
        else Tweet(m)
    | _ -> OtherMessage(message)

let getUrls (message : StatusMessage) =
    [message.Status.ExtendedEntities |> Option.ofObj; message.Status.Entities |> Option.ofObj]
    |> List.tryPick id
    |> Option.bind (fun entities -> entities.Urls |> Option.ofObj)
    |> Option.map Array.toList
    |> defaultArg <| []

let getText (message : StatusMessage) =
    [
        message.Status.ExtendedTweet  |> Option.ofObj |> Option.bind (fun x -> x.FullText |> Option.ofObj)
        message.Status.FullText |> Option.ofObj
        message.Status.Text |> Option.ofObj
    ] |> List.pick id

let getExpandedText (message : StatusMessage) =
    let urls = getUrls message
    let text = getText message
    (text, urls |> List.sortBy (fun x -> -x.Indices.[0])) ||> List.fold (fun text url ->
        let [|includeStart; excludeEnd|] = url.Indices
        let pre = if includeStart = 0 then "" else text.[.. includeStart - 1]
        let post = if text.Length < excludeEnd then "" else text.[excludeEnd ..]
        pre + url.ExpandedUrl + post)

let delete (tokens : Tokens) (tweetId : int64) =
    tokens.Statuses.Destroy(tweetId)
