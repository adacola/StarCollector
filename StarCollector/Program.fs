module Adacola.StarCollector.Main

open System
open System.IO
open System.Text

[<EntryPoint>]
let main argv = 
    let consumerKey = File.ReadAllText("consumerKey.json", Encoding.UTF8) |> ConsumerKeyProvider.Parse
    let accessToken = File.ReadAllText("accessToken.json", Encoding.UTF8) |> AccessTokenProvider.Parse
    let blackAndWhiteList = File.ReadAllText("list.json", Encoding.UTF8) |> ListProvider.Parse
    let userID = accessToken.UserId
    let excludes = BlackAndWhiteList.parse blackAndWhiteList

    let tokens = Twitter.authenticate consumerKey accessToken
    let connection = Twitter.getTweetConnection tokens
    connection.Subscribe(fun message ->
        match userID, message with
        | Twitter.MyTweet m when m |> Twitter.getText |> excludes -> Console.WriteLine("削除対象 : {0}", m |> Twitter.getText)
        | Twitter.MyTweet m -> Console.WriteLine("削除しない : {0}", m |> Twitter.getText)
        | _ -> Console.WriteLine("その他"))
    |> ignore
    connection.Connect() |> ignore

    Console.ReadLine() |> ignore
    0
