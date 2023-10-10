// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open Oryx
open Oryx.Cognite
open CogniteSdk

/// Various HTTP handlers for the Units API.
[<RequireQualifiedAccess>]
module Units =
    [<Literal>]
    let Url = "/units"

    let SystemsUrl = Url +/ "systems"

    /// List all units
    let listUnits (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<UnitItem>> =
        source |> withLogMessage "units:listUnits" |> HttpHandler.getV10 Url

    /// List all unit systems
    let listUnitSystems (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<UnitSystem>> =
        source
        |> withLogMessage "units:listUnitSystems"
        |> HttpHandler.getV10 SystemsUrl

    /// Retrieve a single unit by external Id
    let getUnit (unitExternalId: string) (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<UnitItem>> =
        source |> withLogMessage "units:getUnit" |> getByExternalId unitExternalId Url

    /// Retrieves multiple units by external ID
    /// TODO: Move away from Identity as the Unit Catalog only have externalIds
    let retrieveUnits
        (externalIds: Identity seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<UnitItem seq> =
        source
        |> withLogMessage "units:retrieve"
        |> retrieveIgnoreUnknownIds externalIds (Option.ofNullable ignoreUnknownIds) Url
