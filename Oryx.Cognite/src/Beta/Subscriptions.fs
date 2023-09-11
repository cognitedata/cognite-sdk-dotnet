// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System.Collections.Generic

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module Subscriptions =
    [<Literal>]
    let Url = "/timeseries/subscriptions"

    /// Create a list of datapoint subscriptions
    let create (items: SubscriptionCreate seq) (source: HttpHandler<unit>) : HttpHandler<Subscription seq> =
        source
        |> withLogMessage "timeseries:subscriptions:create"
        |> withBetaHeader
        |> HttpHandler.create items Url

    /// Delete list of datapoint subscriptions by externalId, optionally ignoring unknown IDs
    let delete (items: string seq) (ignoreUnknownIds: bool) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        http {
            let items = items |> Seq.map (fun f -> CogniteExternalId(f))

            let request =
                ItemsWithIgnoreUnknownIds(Items = items, IgnoreUnknownIds = ignoreUnknownIds)

            return!
                source
                |> withLogMessage "timeseries:subscriptions:delete"
                |> withBetaHeader
                |> HttpHandler.delete request Url
        }

    /// Retrieve the next batch of data from a datapoint subscription
    let listData (query: ListSubscriptionData) (source: HttpHandler<unit>) : HttpHandler<SubscriptionDataResponse> =
        source
        |> withLogMessage "timeseries:subscriptions:listdata"
        |> withBetaHeader
        |> HttpHandler.postV10 query (Url +/ "data/list")

    /// List timeseries in a datapoint subscription with pagination
    let listMembers
        (query: ListSubscriptionMembers)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<WrappedTimeSeriesId>> =
        source
        |> withLogMessage "timeseries:subscriptions:listmembers"
        |> withBetaHeader
        |> HttpHandler.getWithQuery query (Url +/ "members")

    /// List subscriptions in a project with pagination
    let list (query: ListSubscriptions) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Subscription>> =
        source
        |> withLogMessage "timeseries:subscriptions:list"
        |> withBetaHeader
        |> HttpHandler.getWithQuery query Url

    /// Retrieve subscriptions by externalId, optionally ignoring unknown IDs
    let retrieve
        (items: string seq)
        (ignoreUnknownIds: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<Subscription seq> =
        http {
            let items = items |> Seq.map (fun f -> CogniteExternalId(f))

            let request =
                ItemsWithIgnoreUnknownIds(Items = items, IgnoreUnknownIds = ignoreUnknownIds)

            let! ret =
                source
                |> withLogMessage "timeseries:subscriptions:retrieve"
                |> withBetaHeader
                |> HttpHandler.postV10<_, ItemsWithoutCursor<_>> request (Url +/ "byids")

            return ret.Items
        }

    /// Update a list of subscriptions
    let update
        (query: IEnumerable<UpdateItem<SubscriptionUpdate>>)
        (source: HttpHandler<unit>)
        : HttpHandler<Subscription seq> =
        source
        |> withLogMessage "timeseries:subscriptions:update"
        |> withBetaHeader
        |> HttpHandler.update query Url
