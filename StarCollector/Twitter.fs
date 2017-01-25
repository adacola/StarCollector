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

let getText (message : StatusMessage) =
    [message.Status.ExtendedTweet.FullText |> Option.ofObj; message.Status.Text |> Option.ofObj]
    |> List.pick id
