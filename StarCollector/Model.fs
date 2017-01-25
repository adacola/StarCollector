namespace Adacola.StarCollector

open FSharp.Data

[<AutoOpen>]
module Model =
    type ConsumerKeyProvider = JsonProvider<"consumerKeySample.json">
    type ConsumerKey = ConsumerKeyProvider.Root
    type AccessTokenProvider = JsonProvider<"accessTokenSample.json">
    type AccessToken = AccessTokenProvider.Root
    type ListProvider = JsonProvider<"listSample.json">
    