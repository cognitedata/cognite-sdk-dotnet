// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

/// Various 3D HTTP handlers.

[<RequireQualifiedAccess>]
module ThreeDModels =
    [<Literal>]
    let Url = "/3d/models"

    /// <summary>
    /// Retrieves list of 3DModels matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DModels matching given filters and optional cursor</returns>
    let list (query: ThreeDModelQuery) : HttpHandler<unit, ItemsWithCursor<ThreeDModel>> =
        withLogMessage "3DModels:list"
        >=> getWithQuery query Url

    /// <summary>
    /// Create new 3D models in the given project.
    /// </summary>
    /// <param name="items">The 3D models to create.</param>
    /// <returns>List of created 3D models.</returns>
    let create (items: ThreeDModelCreate seq) : HttpHandler<unit, ThreeDModel seq> =
        withLogMessage "3DModel:create"
        >=> create items Url

    /// <summary>
    /// Delete multiple 3DModels in the same project.
    /// </summary>
    /// <param name="threeDModels">The list of 3DModels to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: IEnumerable<Identity>) : HttpHandler<unit, EmptyResponse> =
        let items = ItemsWithoutCursor(Items=ids)
        withLogMessage "3DModel:delete"
        >=> delete items Url

    /// <summary>
    /// Retrieves information about a 3DModel in the same project.
    /// </summary>
    /// <param name="modelId">The id of the 3DModel to get.</param>
    /// <returns>3DModels with given ids.</returns>
    let retrieve (modelId: int64) : HttpHandler<unit, ThreeDModel> =
        let url = Url
        withLogMessage "3DModels:retrieve"
        >=> getById modelId url

    /// Update one or more 3DModels. Supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated 3DModels.
    let update (query: IEnumerable<UpdateItem<ThreeDModelUpdate>>) : HttpHandler<unit, ThreeDModel seq>  =
        withLogMessage "3DModels:update"
        >=> update query Url

[<RequireQualifiedAccess>]
module ThreeDRevisions =
    let Url = "/3d/models"

    /// <summary>
    /// Retrieves list of 3DRevisions matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DRevisions matching given filters and optional cursor</returns>
    let list (modelId: int64) (query: ThreeDRevisionQuery) : HttpHandler<unit, ItemsWithCursor<ThreeDRevision>> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions"
        withLogMessage "3DRevisions:list"
        >=> getWithQuery query url

    /// <summary>
    /// Create new 3D Revisions in the given project.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="items">The 3D Revisions to create.</param>
    /// <returns>List of created 3D Revisions.</returns>
    let create (modelId: int64) (items: ThreeDRevisionCreate seq) : HttpHandler<unit, ThreeDRevision seq> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions"
        withLogMessage "3DRevision:create"
        >=> create items url

    /// <summary>
    /// Delete multiple 3DRevisions in the same project.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="threeDRevisions">The list of 3DRevisions to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (modelId: int64) (ids: IEnumerable<Identity>) : HttpHandler<unit, EmptyResponse> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions"
        let items = ItemsWithoutCursor(Items=ids)
        withLogMessage "3DRevision:delete"
        >=> delete items url

    /// <summary>
    /// Retrieves information about multiple 3DRevisions in the same project. A maximum of 1000 3DRevision IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="ids">The ids of the 3DRevisions to get.</param>
    /// <returns>3DRevisions with given ids.</returns>
    let retrieve (modelId: int64) (revisionId: int64) : HttpHandler<unit, ThreeDRevision> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions"
        withLogMessage "3DRevisions:retrieve"
        >=> getById revisionId url

    /// <summary>
    /// Update one or more 3DRevisions. Supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated 3DRevisions.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="query">The update query.</param>
    /// <returns>Corresponing revisions after applying the updates.</returns>
    let update (modelId: int64) (query: IEnumerable<UpdateItem<ThreeDRevisionUpdate>>) : HttpHandler<unit, ThreeDRevision seq>  =
        let url = Url +/ sprintf "%d" modelId +/ "revisions"
        withLogMessage "3DRevisions:update"
        >=> update query url

    /// <summary>
    /// Update one or more 3DRevisions. Supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated 3DRevisions.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="revisionId">The revision to get logs from.</param>
    /// <param name="query">The update query.</param>
    /// <returns>Corresponing revisions after applying the updates.</returns>
    let updateThumbnail (modelId: int64) (revisionId: int64) (fileId: int64) : HttpHandler<unit, EmptyResponse>  =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "thumbnail"
        let query = ThreeDUpdateThumbnailQuery(FileId=fileId)
        withLogMessage "3DRevisions:update"
        >=> postWithQuery () query url

    /// <summary>
    /// Retrieves list of 3DRevisionLogs matching severity.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="revisionId">The revision to get logs from.</param>
    /// <param name="severity">Minimum severity to retrieve (3 = INFO, 5 = WARN, 7 = ERROR).</param>
    /// <returns>List of 3DRevisions matching given filters and optional cursor</returns>
    let listLogs (modelId: int64) (revisionId: int64) (query: ThreeDRevisionLogQuery) : HttpHandler<unit, ItemsWithCursor<ThreeDRevisionLog>> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "logs"
        withLogMessage "3DRevisionLogs:list"
        >=> getWithQuery query url

module ThreeDNodes =
    let Url = "/3d/models"

    /// <summary>
    /// Retrieves list of 3DNode matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="modelId">The model to get nodes from.</param>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DNode matching given query and optional cursor</returns>
    let list (modelId: int64) (revisionId: int64) (query: ThreeDNodeQuery) : HttpHandler<unit, ItemsWithCursor<ThreeDNode>> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "nodes"
        withLogMessage "3DNode:list"
        >=> getWithQuery query url

module ThreeDAssetMappings =
    let Url = "3d/models"

    /// <summary>
    /// Retrieves list of 3DAssetMapping matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="modelId">The model to get asset mappings from.</param>
    /// <param name="revisionId">The revision to get asset mappings from.</param>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DAssetMapping matching given query and optional cursor</returns>
    let list (modelId: int64) (revisionId: int64) (query: ThreeDAssetMappingQuery) : HttpHandler<unit, ItemsWithCursor<ThreeDAssetMapping>> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "mappings"
        withLogMessage "3DAssetMapping:list"
        >=> list query url

    /// <summary>
    /// Create new 3D AssetMapping in the given project.
    /// </summary>
    /// <param name="modelId">The model to get asset mappings from.</param>
    /// <param name="revisionId">The revision to get asset mappings from.</param>
    /// <param name="items">The 3D asset mappings to create.</param>
    /// <returns>List of created 3D AssetMappings.</returns>
    let create (modelId: int64) (revisionId: int64) (items: ThreeDAssetMappingCreate seq) : HttpHandler<unit, ThreeDAssetMapping seq> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "mappings"
        withLogMessage "3DAssetMapping:create"
        >=> create items url

    /// Delete 3D AssetMappings in the given project.
    /// </summary>
    /// <param name="modelId">The model to delete asset mappings from.</param>
    /// <param name="revisionId">The revision to delete asset mappings from.</param>
    /// <param name="assetMappings">AssetMappings to delete</param>
    /// <returns>Empty result.</returns>
    let delete (modelId: int64) (revisionId: int64) (assetMappings: IEnumerable<Identity>) : HttpHandler<unit, EmptyResponse> =
        let url = Url +/ sprintf "%d" modelId +/ "revisions" +/ sprintf "%d" revisionId +/ "mappings"
        let items = ItemsWithoutCursor(Items=assetMappings)
        withLogMessage "3DAssetMapping:delete"
        >=> delete items url