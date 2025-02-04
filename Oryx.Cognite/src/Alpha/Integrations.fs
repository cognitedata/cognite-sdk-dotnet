// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open System

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha


[<RequireQualifiedAccess>]
module Integrations =
    open CogniteSdk
    open System.Collections.Generic

    [<Literal>]
    let Url = "/integrations"

    let checkin (request: CheckInRequest) (source: HttpHandler<unit>) : HttpHandler<CheckInResponse> =
        source
        |> withLogMessage "integrations:checkin"
        |> withAlphaHeader
        |> postV10 request (Url +/ "checkin")

    let startup (request: StartupRequest) (source: HttpHandler<unit>) : HttpHandler<CheckInResponse> =
        source
        |> withLogMessage "integrations:startup"
        |> withAlphaHeader
        |> postV10 request (Url +/ "startup")

    let createConfigRevision
        (revision: CreateConfigRevision)
        (source: HttpHandler<unit>)
        : HttpHandler<ConfigRevision> =
        source
        |> withLogMessage "integrations:createconfigrevision"
        |> withAlphaHeader
        |> postV10 revision (Url +/ "config")

    let create (integrations: CreateIntegration seq) (source: HttpHandler<unit>) : HttpHandler<Integration seq> =
        source
        |> withLogMessage "integrations:create"
        |> withAlphaHeader
        |> create integrations Url

    let delete (items: IntegrationsDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "integrations:delete"
        |> withAlphaHeader
        |> delete items Url

    let retrieve (items: IntegrationsRetrieve) (source: HttpHandler<unit>) : HttpHandler<Integration seq> =
        http {
            let! res =
                source
                |> withLogMessage "integrations:retrieve"
                |> withAlphaHeader
                |> postV10<IntegrationsRetrieve, ItemsWithoutCursor<Integration>> items (Url +/ "byids")

            return res.Items
        }

    let getConfigRevision
        (integration: string)
        (revision: Nullable<int>)
        (source: HttpHandler<unit>)
        : HttpHandler<ConfigRevision> =
        let query = ConfigRevisionQuery(Integration = integration, Revision = revision)

        source
        |> withLogMessage "integrations:getconfigrevision"
        |> withAlphaHeader
        |> getWithQuery query (Url +/ "config")

    let getTaskHistory
        (query: TaskHistoryQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TaskHistory>> =
        source
        |> withLogMessage "integrations:gettaskhistory"
        |> withAlphaHeader
        |> getWithQuery query (Url +/ "history")

    let listConfigRevisions
        (integration: string)
        (source: HttpHandler<unit>)
        : HttpHandler<ConfigRevisionMetadata seq> =
        http {
            let query = ConfigRevisionsQuery(Integration = integration)

            let! ret =
                source
                |> withLogMessage "integrations:listconfigrevisions"
                |> withAlphaHeader
                |> getWithQuery<ItemsWithoutCursor<ConfigRevisionMetadata>> query (Url +/ "config" +/ "revisions")

            return ret.Items
        }

    let listErrors (query: ErrorsQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<ErrorWithTask>> =
        source
        |> withLogMessage "integrations:listerrors"
        |> withAlphaHeader
        |> getWithQuery query (Url +/ "errors")

    let list (query: IntegrationsQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Integration>> =
        source
        |> withLogMessage "integrations:list"
        |> withAlphaHeader
        |> getWithQuery query Url

    let update
        (items: IEnumerable<UpdateItem<UpdateIntegration>>)
        (source: HttpHandler<unit>)
        : HttpHandler<Integration seq> =
        source
        |> withLogMessage "integrations:update"
        |> withAlphaHeader
        |> update items Url
