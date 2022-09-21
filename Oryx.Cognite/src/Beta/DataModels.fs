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

    let createSpaces (items: Space seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        source
        |> withLogMessage "dms:spaces:create"
        |> withAlphaHeader
        |> HttpHandler.create items spacesUrl

    let deleteSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let query =
            ItemsWithoutCursor(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id)))
            )
        
        source
        |> withLogMessage "dms:spaces:delete"
        |> withAlphaHeader
        |> HttpHandler.delete query spacesUrl

    let listSpaces (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        let query = EmptyResponse()
        
        source
        |> withLogMessage "dms:spaces:list"
        |> withAlphaHeader
        |> HttpHandler.list query spacesUrl

    let retrieveSpaces (items: string seq) (source: HttpHandler<unit>) : HttpHandler<Space seq> =
        source
        |> withLogMessage "dms:spaces:retrieve"
        |> withAlphaHeader
        |> HttpHandler.retrieve (items |> Seq.map (fun id -> Identity.Create(id))) spacesUrl

    let applyModels (items: ModelCreate seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let request = ItemsWithSpaceExternalId<ModelCreate>(
            Items = items,
            SpaceExternalId = space
        )
        source
        |> withLogMessage "dms:models:apply"
        |> withAlphaHeader
        |> postV10 request modelsUrl

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

    let listModels (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let request = ListModelsQuery(SpaceExternalId = space)

        source
        |> withLogMessage "dms:models:list"
        |> withAlphaHeader
        |> HttpHandler.list request modelsUrl

    let retrieveModels (items: string seq) (space: string) (source: HttpHandler<unit>) : HttpHandler<Model seq> =
        let query = ItemsWithSpaceExternalId(
            Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
            SpaceExternalId = space
        )

        source
        |> withLogMessage "dms:models:retrieve"
        |> withAlphaHeader
        |> postV10 query (modelsUrl +/ "byids")

    let ingestNodes (request: NodeIngestRequest<'T>) (source: HttpHandler<unit>) : HttpHandler<'T seq> =
        http {
            let! res =
                source
                |> withLogMessage "dms:nodes:create"
                |> withAlphaHeader
                |> postV10<NodeIngestRequest<'T>, ItemsWithoutCursor<'T>> request nodesUrl
            return res.Items
        }

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

    let filterNodes (query: NodeFilterQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:list"
        |> withAlphaHeader
        |> HttpHandler.list query nodesUrl
        
    let searchNodes (query: NodeSearchQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:search"
        |> withAlphaHeader
        |> postV10 query (nodesUrl +/ "search")

    let retrieveNodes (query: RetrieveNodesRequest) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:nodes:retrieve"
        |> withAlphaHeader
        |> postV10 query (nodesUrl +/ "byids")

    let ingestEdges (request: NodeIngestRequest<'T>) (source: HttpHandler<unit>) : HttpHandler<'T seq> =
        http {
            let! res =
                source
                |> withLogMessage "dms:edges:create"
                |> withAlphaHeader
                |> postV10<NodeIngestRequest<'T>, ItemsWithoutCursor<'T>> request edgesUrl
            return res.Items
        }

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

    let filterEdges (query: NodeFilterQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:list"
        |> withAlphaHeader
        |> HttpHandler.list query edgesUrl
        
    let searchEdges (query: NodeSearchQuery) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:search"
        |> withAlphaHeader
        |> postV10 query (edgesUrl +/ "search")

    let retrieveEdges (query: RetrieveNodesRequest) (source: HttpHandler<unit>) : HttpHandler<NodeListResponse<'T>> =
        source
        |> withLogMessage "dms:edges:retrieve"
        |> withAlphaHeader
        |> postV10 query (edgesUrl +/ "byids")

    let graphQuery (query: GraphQuery) (source: HttpHandler<unit>) : HttpHandler<'T> =
        source
        |> withLogMessage "dms:query"
        |> withAlphaHeader
        |> postV10 query (Url +/ "graphquery")


