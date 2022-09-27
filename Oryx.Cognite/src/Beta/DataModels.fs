// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module DataModels =
    [<Literal>]
    let Url = "/datamodelstorage"

    let spacesUrl = Url +/ "spaces"
    let modelsUrl = Url +/ "models"
    let nodesUrl = Url +/ "nodes"
    let edgesUrl = Url +/ "edges"

    /// Create a list of spaces
    let createSpaces (items: Space seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        source
        |> withLogMessage "dms:spaces:create"
        |> withAlphaHeader
        |> HttpHandler.create items spacesUrl

    /// Delete a list of spaces
    let deleteSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let query =
            ItemsWithoutCursor(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id)))
            )
        
        source
        |> withLogMessage "dms:spaces:delete"
        |> withAlphaHeader
        |> HttpHandler.delete query spacesUrl

    /// List all spaces
    let listSpaces (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        let query = EmptyResponse()
        
        source
        |> withLogMessage "dms:spaces:list"
        |> withAlphaHeader
        |> HttpHandler.list query spacesUrl

    /// Retrieve spaces with given ids
    let retrieveSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        source
        |> withLogMessage "dms:spaces:retrieve"
        |> withAlphaHeader
        |> HttpHandler.retrieve (items |> Seq.map (fun id -> Identity.Create(id))) spacesUrl

    /// Apply a list of data models in the given space
    let applyModels (items: ModelCreate seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let request = ItemsWithSpaceExternalId<ModelCreate>(
            Items = items,
            SpaceExternalId = space
        )
        http {
            let! res = 
                source
                |> withLogMessage "dms:models:apply"
                |> withAlphaHeader
                |> postV10<ItemsWithSpaceExternalId<ModelCreate>, ItemsWithoutCursor<Model>> request modelsUrl
            return res.Items
        }
        
    /// Delete a list of data models in the given space
    let deleteModels (items: string seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let query =
            ItemsWithSpaceExternalId(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                SpaceExternalId = space
            )

        source
        |> withLogMessage "dms:models:delete"
        |> withAlphaHeader
        |> HttpHandler.delete query modelsUrl

    /// List all models in the given space
    let listModels (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let request = ListModelsQuery(SpaceExternalId = space)

        http {
            let! res =
                source
                |> withLogMessage "dms:models:list"
                |> withAlphaHeader
                |> HttpHandler.list<ListModelsQuery, ItemsWithoutCursor<Model>> request modelsUrl
            return res.Items
        }

    /// Retrieve models by externalId in the given space
    let retrieveModels (items: string seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let query = ItemsWithSpaceExternalId(
            Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
            SpaceExternalId = space
        )
        http {
            let! res =
                source
                |> withLogMessage "dms:models:retrieve"
                |> withAlphaHeader
                |> postV10<ItemsWithSpaceExternalId<CogniteExternalId>, ItemsWithoutCursor<Model>> query (modelsUrl +/ "byids")
            return res.Items
        }

    /// Ingest a list of nodes
    let ingestNodes (request: NodeIngestRequest<'T>) (source: HttpHandler<unit>) : HttpHandler<'T seq> =
        http {
            let! res =
                source
                |> withLogMessage "dms:nodes:create"
                |> withAlphaHeader
                |> postV10<NodeIngestRequest<'T>, ItemsWithoutCursor<'T>> request nodesUrl
            return res.Items
        }

    /// Delete a list of nodes
    let deleteNodes (items: string seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let query =
            ItemsWithSpaceExternalId(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                SpaceExternalId = space
            )

        source
        |> withLogMessage "dms:nodes:delete"
        |> withAlphaHeader
        |> HttpHandler.delete query nodesUrl

    /// List nodes with a filter
    let filterNodes (query: NodeFilterQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:list"
        |> withAlphaHeader
        |> HttpHandler.list query nodesUrl
        
    /// Search nodes, retrieves up to 1000
    let searchNodes (query: NodeSearchQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:search"
        |> withAlphaHeader
        |> postV10 query (nodesUrl +/ "search")

    /// Retrieve nodes by externalId
    let retrieveNodes (query: RetrieveNodesRequest) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:retrieve"
        |> withAlphaHeader
        |> postV10 query (nodesUrl +/ "byids")

    /// Ingest a list of edges
    let ingestEdges (request: EdgeIngestRequest<'T>) (source: HttpHandler<unit>) : HttpHandler<'T seq> =
        http {
            let! res =
                source
                |> withLogMessage "dms:edges:create"
                |> withAlphaHeader
                |> postV10<EdgeIngestRequest<'T>, ItemsWithoutCursor<'T>> request edgesUrl
            return res.Items
        }

    /// Delete a list of edges
    let deleteEdges (items: string seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let query =
            ItemsWithSpaceExternalId(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                SpaceExternalId = space
            )

        source
        |> withLogMessage "dms:edges:delete"
        |> withAlphaHeader
        |> HttpHandler.delete query edgesUrl

    /// List edges with a filter
    let filterEdges (query: NodeFilterQuery) (source: HttpHandler<unit>) : HttpHandler<EdgeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:list"
        |> withAlphaHeader
        |> HttpHandler.list query edgesUrl
        
    /// Search edges, retrieves up to 1000
    let searchEdges (query: NodeSearchQuery) (source: HttpHandler<unit>) : HttpHandler<EdgeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:search"
        |> withAlphaHeader
        |> postV10 query (edgesUrl +/ "search")

    /// Retrieve edges by externalId
    let retrieveEdges (query: RetrieveNodesRequest) (source: HttpHandler<unit>) : HttpHandler<EdgeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:retrieve"
        |> withAlphaHeader
        |> postV10 query (edgesUrl +/ "byids")

    /// Execute a graph query.
    let graphQuery (query: GraphQuery) (source: HttpHandler<unit>) : HttpHandler<'T> =
        source
        |> withLogMessage "dms:query"
        |> withAlphaHeader
        |> postV10 query (Url +/ "graphquery")


