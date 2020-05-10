// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

/// Various 3D HTTP handlers.

[<RequireQualifiedAccess>]
module ThreeDModel =
    [<Literal>]
    let Url = "/3d/models"

    /// <summary>
    /// Retrieves list of 3DModels matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DModels matching given filters and optional cursor</returns>
    let list (query: ThreeDModelQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<ThreeDModel>, 'a> =
        withLogMessage "3DModels:list"
        >=> list query Url

    /// <summary>
    /// Create new 3D models in the given project.
    /// </summary>
    /// <param name="items">The 3D models to create.</param>
    /// <returns>List of created 3D models.</returns>
    let create (items: ThreeDModelCreate seq) : HttpHandler<HttpResponseMessage, ThreeDModel seq, 'a> =
        withLogMessage "3DModel:create"
        >=> create items Url

    /// <summary>
    /// Delete multiple 3DModels in the same project.
    /// </summary>
    /// <param name="threeDModels">The list of 3DModels to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (threeDModels: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        withLogMessage "3DModel:delete"
        >=> delete threeDModels Url

    /// <summary>
    /// Retrieves information about multiple 3DModels in the same project. A maximum of 1000 3DModel IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the 3DModels to get.</param>
    /// <returns>3DModels with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ThreeDModel seq, 'a> =
        withLogMessage "3DModels:retrieve"
        >=> retrieve ids Url

    /// Update one or more 3DModels. Supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated 3DModels.
    let update (query: IEnumerable<UpdateItem<ThreeDModelUpdate>>) : HttpHandler<HttpResponseMessage, ThreeDModel seq, 'a>  =
        withLogMessage "3DModels:update"
        >=> update query Url

[<RequireQualifiedAccess>]
module ThreeDRevision =
    let Url = "/3d/models"

    /// <summary>
    /// Retrieves list of 3DRevisions matching query, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="query">The query to use.</param>
    /// <returns>List of 3DRevisions matching given filters and optional cursor</returns>
    let list (modelId: string) (query: ThreeDRevisionQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<ThreeDRevision>, 'a> =
        let url = Url +/ modelId
        withLogMessage "3DRevisions:list"
        >=> list query url

    /// <summary>
    /// Create new 3D Revisions in the given project.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="items">The 3D Revisions to create.</param>
    /// <returns>List of created 3D Revisions.</returns>
    let create (modelId: string) (items: ThreeDRevisionCreate seq) : HttpHandler<HttpResponseMessage, ThreeDRevision seq, 'a> =
        let url = Url +/ modelId
        withLogMessage "3DRevision:create"
        >=> create items url

    /// <summary>
    /// Delete multiple 3DRevisions in the same project.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="threeDRevisions">The list of 3DRevisions to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (modelId: string) (threeDRevisions: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let url = Url +/ modelId
        withLogMessage "3DRevision:delete"
        >=> delete threeDRevisions url

    /// <summary>
    /// Retrieves information about multiple 3DRevisions in the same project. A maximum of 1000 3DRevision IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="ids">The ids of the 3DRevisions to get.</param>
    /// <returns>3DRevisions with given ids.</returns>
    let retrieve (modelId: string) (ids: Identity seq) : HttpHandler<HttpResponseMessage, ThreeDRevision seq, 'a> =
        let url = Url +/ modelId
        withLogMessage "3DRevisions:retrieve"
        >=> retrieve ids url

    /// <summary>
    /// Update one or more 3DRevisions. Supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated 3DRevisions.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="query">The update query.</param>
    /// <returns>Corresponing revisions after applying the updates.</returns>
    let update (modelId: string) (query: IEnumerable<UpdateItem<ThreeDRevisionUpdate>>) : HttpHandler<HttpResponseMessage, ThreeDRevision seq, 'a>  =
        let url = Url +/ modelId
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
    let updateThumbnail (modelId: string) (revisionId: string) (query: IEnumerable<UpdateItem<ThreeDRevisionUpdateThumbnail>>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a>  =
        let url = Url +/ modelId +/ "revisions" +/ revisionId +/ "thumbnail"
        let request = ItemsWithoutCursor<UpdateItem<ThreeDRevisionUpdateThumbnail>>(Items = query)
        withLogMessage "3DRevisions:update"
        >=> postV10<ItemsWithoutCursor<UpdateItem<ThreeDRevisionUpdateThumbnail>>, EmptyResponse, 'a> request url

    /// <summary>
    /// Retrieves list of 3DRevisionLogs matching severity.
    /// </summary>
    /// <param name="modelId">The model to get revisions from.</param>
    /// <param name="revisionId">The revision to get logs from.</param>
    /// <param name="severity">Minimum severity to retrieve (3 = INFO, 5 = WARN, 7 = ERROR).</param>
    /// <returns>List of 3DRevisions matching given filters and optional cursor</returns>
    let listLogs (modelId: string) (revisionId: string) (severity: int64) : HttpHandler<HttpResponseMessage, ItemsWithCursor<ThreeDRevisionLog>, 'a> =
        let url = Url +/ modelId +/ "revisions" +/ revisionId +/ "logs"
        withLogMessage "3DRevisionLogs:list"
        >=> Handler.list severity url

module ThreeDNodes =
    