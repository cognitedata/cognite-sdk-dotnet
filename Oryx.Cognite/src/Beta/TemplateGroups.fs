// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System
open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module TemplateGroups =
    [<Literal>]
    let Url = "/templategroups"

    let templateGroupVersionsUrl externalId =
        Url +/ externalId +/ "versions"

    /// <summary>
    /// Retrieves list of template groups and a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="query">The query with limit and cursor.</param>
    /// <returns>List of domains.</returns>
    let list (query: TemplateGroupFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroup>> =
        let url = Url
        withLogMessage "templategroups:list"
        >=> list query url

    /// <summary>
    /// Retrieves a list of versions of a given template group, and a version if given limit is exceeded.
    /// </summary>
    let listVersions (externalId: string) (query: TemplateGroupVersionFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroupVersion>> =
        let url = templateGroupVersionsUrl externalId
        withLogMessage "templategroups:listVersions"
        >=> Handler.list query url

    /// <summary>
    /// Runs a query by using GraphQL against a domain.
    /// </summary>
    /// <param name="externalId">Unique reference to the Template Group.</param>
    /// <param name="templateGroupVersion">Select a specific version of the Template Group schema</param>
    /// <param name="query">The GraphQL query.</param>
    /// <returns>The GraphQL result.</returns>
    let graphql (externalId: string) (templateGroupVersion: int) (query: string) : HttpHandler<unit, GraphQlResult> =
        let url =
            templateGroupVersionsUrl externalId
            +/ (templateGroupVersion |> string)
            +/ "graphql"

        let query = GraphQlQuery(Query = query)

        withLogMessage "templategroups:schema"
        >=> postV10 query url
