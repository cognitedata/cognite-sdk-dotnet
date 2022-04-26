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

    let templateGroupVersionsUrl externalId = Url +/ externalId +/ "versions"

    let templateGroupInstanceUrl externalId version = Url +/ externalId +/ "versions" +/ (version |> string)

    /// Create a list of template groups
    let create (items: TemplateGroupCreate seq) (source: HttpHandler<unit>) : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:create"
        |> create items Url


    let upsert (items: TemplateGroupCreate seq) (source: HttpHandler<unit>) : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:upsert"
        |> Handler.create items (Url +/ "upsert")

    
    let retrieve (items: string seq) (ignoreUnkownIds: Nullable<bool>) (source: HttpHandler<unit>) : HttpHandler<TemplateGroup seq> =
        source
        |> withLogMessage "templategroups:retrieve"
        |> retrieveIgnoreUnknownIds (items |> Seq.map (fun id -> Identity.Create id)) (Option.ofNullable ignoreUnkownIds) Url


    let filter (query: TemplateInstanceFilterQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TemplateGroup>> =
        source
        |> withLogMessage "templategroups:filter"
        |> list query Url


    let delete (items: string seq) (ignoreUnknownIds: bool) (source: HttpHandler<unit>) : HttpHandler<unit> =
        let query = ItemsWithIgnoreUnknownIds(Items=(items |> Seq.map (fun id -> CogniteExternalId(id))), IgnoreUnknownIds=ignoreUnknownIds)
        source
        |> withLogMessage "templategroups:delete"
        |> delete query Url

    /// <summary>
    /// Retrieves a list of versions of a given template group, and a version if given limit is exceeded.
    /// </summary>
    let listVersions
        (externalId: string)
        (query: TemplateGroupVersionFilter)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<TemplateGroupVersion>> =
        let url = templateGroupVersionsUrl externalId

        source
        |> withLogMessage "templategroups:listVersions"
        |> Handler.list query url

    /// <summary>
    /// Runs a query by using GraphQL against a domain.
    /// </summary>
    /// <param name="externalId">Unique reference to the Template Group.</param>
    /// <param name="templateGroupVersion">Select a specific version of the Template Group schema</param>
    /// <param name="query">The GraphQL query.</param>
    /// <returns>The GraphQL result.</returns>
    let graphql
        (externalId: string)
        (templateGroupVersion: int)
        (query: string)
        (source: HttpHandler<unit>)
        : HttpHandler<GraphQlResult> =
        let url =
            templateGroupVersionsUrl externalId
            +/ (templateGroupVersion |> string)
            +/ "graphql"

        let query = GraphQlQuery(Query = query)

        source
        |> withLogMessage "templategroups:schema"
        |> postV10 query url
