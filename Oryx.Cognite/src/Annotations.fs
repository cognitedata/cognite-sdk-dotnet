// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various annotation HTTP handlers.

[<RequireQualifiedAccess>]
module Annotations =
    [<Literal>]
    let Url = "/annotations"

    /// <summary>
    /// Create new annotations in the given project.
    /// </summary>
    /// <param name="annotations">The annotations to create.</param>
    /// <returns>List of created annotations.</returns>
    let create (annotations: AnnotationCreate seq) (source: HttpHandler<unit>) : HttpHandler<Annotation seq> =
        source |> withLogMessage "Annotation:create" |> create annotations Url

    /// <summary>
    /// Suggest new annotations in the given project.
    /// </summary>
    /// <param name="annotations">The annotations to suggest.</param>
    /// <returns>List of created annotations.</returns>
    let suggest (annotations: AnnotationSuggest seq) (source: HttpHandler<unit>) : HttpHandler<Annotation seq> =
        source |> withLogMessage "Annotation:suggest" |> suggest annotations Url

    /// <summary>
    /// Delete multiple annotations in the same project.
    /// </summary>
    /// <param name="items">The list of ids for Annotations to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (items: AnnotationDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source |> withLogMessage "Annotations:delete" |> delete items Url

    /// <summary>
    /// Retrieves list of annotations matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of annotations matching given filters and optional cursor</returns>
    let list (query: AnnotationQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<#Annotation>> =
        source |> withLogMessage "Annotations:list" |> list query Url

    /// Update one or more annotations. Supports partial updates, meaning that fields omitted from the requests are not changed.
    let update
        (query: IEnumerable<UpdateItem<AnnotationUpdate>>)
        (source: HttpHandler<unit>)
        : HttpHandler<#Annotation seq> =
        source |> withLogMessage "Annotations:update" |> update query Url
