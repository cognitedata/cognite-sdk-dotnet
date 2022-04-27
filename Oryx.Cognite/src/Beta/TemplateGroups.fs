// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module TemplateGroups =
    [<Literal>]
    let Url = "/templategroups"

    let versionsUrl externalId = Url +/ externalId +/ "versions"

    let baseVersionedUrl externalId version =
        Url
        +/ externalId
        +/ "versions"
        +/ (version |> string)

    let instancesUrl externalId version =
        baseVersionedUrl externalId version +/ "instances"

    let viewsUrl externalId version =
        baseVersionedUrl externalId version +/ "views"


    /// Create a list of template groups.
    let create (items: TemplateGroupCreate seq) (source: HttpHandler<unit>) : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:create"
        |> create items Url

    /// Upsert a list of template groups.
    let upsert (items: TemplateGroupCreate seq) (source: HttpHandler<unit>) : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:upsert"
        |> Handler.create items (Url +/ "upsert")

    /// Retrieve a list of template groups by external id.
    let retrieve
        (items: string seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:retrieve"
        |> retrieveIgnoreUnknownIds
            (items |> Seq.map (fun id -> Identity.Create id))
            (Option.ofNullable ignoreUnknownIds)
            Url

    /// Filter template groups.
    let filter
        (query: TemplateInstanceFilterQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TemplateGroup>> =
        source
        |> withLogMessage "templategroups:filter"
        |> list query Url

    /// Delete a list of template groups.
    let delete (items: string seq) (ignoreUnknownIds: bool) (source: HttpHandler<unit>) : HttpHandler<unit> =
        let query =
            ItemsWithIgnoreUnknownIds(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                IgnoreUnknownIds = ignoreUnknownIds
            )

        source
        |> withLogMessage "templategroups:delete"
        |> delete query Url

    /// Upsert a version. If the "version" number is left out, this will create a new template version.
    let upsertVersion
        (externalId: string)
        (item: TemplateVersionCreate)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateVersion> =
        source
        |> withLogMessage "templateversions:create"
        |> postV10 item ((versionsUrl externalId) +/ "upsert")

    /// Filter versions in the template group, with optional cursor if the given limit is exceeded.
    let filterVersions
        (externalId: string)
        (query: TemplateVersionQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TemplateVersion>> =
        source
        |> withLogMessage "templateversions:filter"
        |> Handler.list query (versionsUrl externalId)

    /// Delete a given version from the template group.
    let deleteVersions (externalId: string) (item: int) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> withLogMessage "templateversions:delete"
        |> Handler.delete (TemplateVersionItem(Version = item)) (versionsUrl externalId)

    /// Create a list of instances for the given template group version.
    let createInstances
        (externalId: string)
        (version: int)
        (items: TemplateInstanceCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateInstance seq> =
        source
        |> withLogMessage "templateinstances:create"
        |> Handler.create items (instancesUrl externalId version)

    /// Upsert instances for the given template group version.
    let upsertInstances
        (externalId: string)
        (version: int)
        (items: TemplateInstanceCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateInstance seq> =
        source
        |> withLogMessage "templateinstances:create"
        |> Handler.create items ((instancesUrl externalId version) +/ "upsert")

    /// Update instances for the given template group version.
    let updateInstances
        (externalId: string)
        (version: int)
        (items: UpdateItem<TemplateInstanceUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateInstance seq> =
        source
        |> withLogMessage "templateinstances:update"
        |> Handler.update items ((instancesUrl externalId version) +/ "update")

    /// Retrieve instances for the given template group version, optionally ignoring unknown ids.
    let retrieveInstances
        (externalId: string)
        (version: int)
        (items: string seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateInstance seq> =
        source
        |> withLogMessage "templateinstances:retrieve"
        |> retrieveIgnoreUnknownIds
            (items |> Seq.map (fun id -> Identity.Create id))
            (Option.ofNullable ignoreUnknownIds)
            (instancesUrl externalId version)

    /// List instances for the given template group version with optional filter, returns cursor if the number of results exceed limit.
    let filterInstances
        (externalId: string)
        (version: int)
        (query: TemplateInstanceFilterQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TemplateInstance>> =
        source
        |> withLogMessage "templateinstances:filter"
        |> Handler.list query (instancesUrl externalId version)

    /// Delete instances for the given template group version, optionally ignoring unknown ids.
    let deleteInstances
        (externalId: string)
        (version: int)
        (items: string seq)
        (ignoreUnknownIds: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<unit> =
        let query =
            ItemsWithIgnoreUnknownIds(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                IgnoreUnknownIds = ignoreUnknownIds
            )

        source
        |> withLogMessage "templateinstances:delete"
        |> Handler.delete query (instancesUrl externalId version)

    /// Create a list of views for the given template group version.
    let createViews
        (externalId: string)
        (version: int)
        (items: TemplateViewCreate<'TFilter> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateView<'TFilter> seq> =
        source
        |> withLogMessage "templateviews:create"
        |> Handler.create items (viewsUrl externalId version)

    /// Upsert a list of views for the given template group version.
    let upsertViews
        (externalId: string)
        (version: int)
        (items: TemplateViewCreate<'TFilter> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<TemplateView<'TFilter> seq> =
        source
        |> withLogMessage "templateviews:upsert"
        |> Handler.create items ((viewsUrl externalId version) +/ "upsert")

    /// List views for the given template group version with optional filter, returns cursor if the number of results exceed limit.
    let filterViews
        (externalId: string)
        (version: int)
        (query: TemplateViewFilterQuery<'TFilter>)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TemplateView<'TResultFilter>>> =
        source
        |> withLogMessage "templateviews:filter"
        |> Handler.list query (viewsUrl externalId version)

    /// Resolve the view for the given template group version, returning a list of results with cursor if the number of results exceed limit.
    let resolveView
        (externalId: string)
        (version: int)
        (req: TemplateViewResolveRequest<'TFilter>)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<'TResult>> =
        source
        |> withLogMessage "templateviews:resolve"
        |> postV10 req ((viewsUrl externalId version) +/ "resolve")

    /// Delete a list of views for the given template group version, optionall ignoring unknown ids.
    let deleteViews
        (externalId: string)
        (version: int)
        (items: string seq)
        (ignoreUnknownIds: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<unit> =
        let query =
            ItemsWithIgnoreUnknownIds(
                Items = (items |> Seq.map (fun id -> CogniteExternalId(id))),
                IgnoreUnknownIds = ignoreUnknownIds
            )

        source
        |> withLogMessage "templateinstances:delete"
        |> Handler.delete query (viewsUrl externalId version)

    /// Query the given template group version.
    let query
        (externalId: string)
        (version: int)
        (query: GraphQlQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<GraphQlResult<'TResult>> =
        source
        |> withLogMessage "templateversions:query"
        |> postV10 query ((baseVersionedUrl externalId version) +/ "graphql")
