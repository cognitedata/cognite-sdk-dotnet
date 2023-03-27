// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta.DataModels

[<RequireQualifiedAccess>]
module DataModels =
    [<Literal>]
    let Url = "/models"

    let spacesUrl = Url +/ "spaces"
    let modelsUrl = Url +/ "datamodels"
    let viewsUrl = Url +/ "views"
    let containersUrl = Url +/ "containers"
    let instancesUrl = Url +/ "instances"

    /// Create a list of spaces
    let upsertSpaces (items: SpaceCreate seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        source
        |> withLogMessage "models:spaces:create"
        |> HttpHandler.create items spacesUrl

    /// List spaces with pagination
    let listSpaces (query: SpaceQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Space>> =
        source
        |> withLogMessage "models:spaces:list"
        |> HttpHandler.getWithQuery query spacesUrl

    /// Retrieve spaces by id
    let retrieveSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        http {
            let url = spacesUrl +/ "byids"
            let request = ItemsWithoutCursor(Items = (items |> Seq.map (fun id -> SpaceId(Space = id))))

            let! ret =
                source
                |> withLogMessage "models:spaces:retrieve"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Delete a list of spaces
    let deleteSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<string seq> =
        http {
            let url = spacesUrl +/ "delete"
            let request = ItemsWithoutCursor(Items = (items |> Seq.map (fun id -> SpaceId(Space = id))))

            let! ret =
                source
                |> withLogMessage "models:spaces:delete"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<SpaceId>> request url
            return ret.Items |> Seq.map (fun id -> id.Space)
        }

    /// Create or update data models
    let upsertDataModels (items: DataModelCreate seq) (source: HttpHandler<unit>) : HttpHandler<DataModel seq> =
        source
        |> withLogMessage "models:datamodels:create"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.create items modelsUrl

    /// List data models
    let listDataModels (query: DataModelQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<DataModel>> =
        source
        |> withLogMessage "models:datamodels:list"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.getWithQuery query modelsUrl

    /// Filter data models
    let filterDataModels (filter: DataModelFilter) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<DataModel>> =
        source
        |> withLogMessage "models:datamodels:filter"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 filter (modelsUrl +/ "list")

    /// Retrieve data models by id
    let retrieveDataModels (items: FDMExternalId seq) (inlineViews: bool) (source: HttpHandler<unit>) : HttpHandler<DataModel seq> =
        http {
            let url = modelsUrl +/ "byids"
            let query = DataModelInlineViewsQuery(InlineViews = inlineViews)
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:datamodels:retrieve"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> withQuery (query.ToQueryParams())
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Delete data models by id
    let deleteDataModels (items: FDMExternalId seq) (source: HttpHandler<unit>) : HttpHandler<FDMExternalId seq> =
        http {
            let url = modelsUrl +/ "delete"
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:datamodels:delete"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Upsert a list of views
    let upsertViews (items: ViewCreate seq) (source: HttpHandler<unit>) : HttpHandler<View seq> =
        source
        |> withLogMessage "models:views:create"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.create items viewsUrl

    /// List views with pagination
    let listViews (query: ViewQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<View>> =
        source
        |> withLogMessage "models:views:list"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.getWithQuery query viewsUrl

    /// Retrieve views by id
    let retrieveViews (items: FDMExternalId seq) (includeInheritedProperties: bool) (source: HttpHandler<unit>) : HttpHandler<View seq> =
        http {
            let url = viewsUrl +/ "byids"
            let query = ViewIncludePropertiesQuery(IncludeInheritedProperties = includeInheritedProperties)
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:views:retrieve"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> withQuery (query.ToQueryParams())
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Delete views by id
    let deleteViews (items: FDMExternalId seq) (source: HttpHandler<unit>) : HttpHandler<FDMExternalId seq> =
        http {
            let url = viewsUrl +/ "delete"
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:views:delete"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Create or update containers
    let upsertContainers (items: ContainerCreate seq) (source: HttpHandler<unit>) : HttpHandler<Container seq> =
        source
        |> withLogMessage "models:containers:create"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.create items containersUrl

    /// List containers with pagination
    let listContainers (query: ContainersQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Container>> =
        source
        |> withLogMessage "models:containers:list"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> HttpHandler.getWithQuery query containersUrl

    /// Retrieve containers by id
    let retrieveContainers (items: ContainerId seq) (source: HttpHandler<unit>) : HttpHandler<Container seq> =
        http {
            let url = containersUrl +/ "byids"
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:containers:retrieve"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Delete containers by id
    let deleteContainers (items: ContainerId seq) (source: HttpHandler<unit>) : HttpHandler<ContainerId seq> =
        http {
            let url = containersUrl +/ "delete"
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:containers:delete"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Create or update a list of instances
    let upsertInstances (request: InstanceWriteRequest) (source: HttpHandler<unit>) : HttpHandler<SlimInstance seq> =
        http {
            let! ret =
                source
                |> withLogMessage "models:instances:create"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request instancesUrl
            return ret.Items
        }

    /// Filter instances with pagination
    let filterInstances<'T> (request: InstancesFilter) (source: HttpHandler<unit>) : HttpHandler<InstancesFilterResponse<'T>> =
        source
        |> withLogMessage "models:instances:filter"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "list")
    
    /// Retrieve instances by id
    let retrieveInstances<'T> (request: InstancesRetrieve) (source: HttpHandler<unit>) : HttpHandler<InstancesRetrieveResponse<'T>> =
        source
        |> withLogMessage "models:instances:retrieve"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "byids")

    /// Search instances
    let searchInstances<'T> (request: InstancesSearch) (source: HttpHandler<unit>) : HttpHandler<InstancesFilterResponse<'T>> =
        source
        |> withLogMessage "models:instances:search"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "search")

    /// Aggregate instances
    let aggregateInstances (request: InstancesAggregate) (source: HttpHandler<unit>) : HttpHandler<InstancesAggregateResponse> =
        source
        |> withLogMessage "models:instances:aggregate"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "aggregate")

    /// Delete instances by id
    let deleteInstances (items: InstanceIdentifier seq) (source: HttpHandler<unit>) : HttpHandler<InstanceIdentifier seq> =
        http {
            let url = instancesUrl +/ "delete"
            let request = ItemsWithoutCursor(Items = items)

            let! ret =
                source
                |> withLogMessage "models:instances:delete"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request url
            return ret.Items
        }

    /// Query instances
    let queryInstances<'T> (request: Query) (source: HttpHandler<unit>) : HttpHandler<QueryResult<'T>> =
        source
        |> withLogMessage "models:instances:query"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "query")

    /// Sync instances
    let syncInstances<'T> (request: SyncQuery) (source: HttpHandler<unit>) : HttpHandler<SyncResult<'T>> =
        source
        |> withLogMessage "models:instances:query"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (instancesUrl +/ "sync")