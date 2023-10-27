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
        source |> withLogMessage "units:listUnits" |> getV10 Url

    /// List all unit systems
    let listUnitSystems (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<UnitSystem>> =
        source
        |> withLogMessage "units:listUnitSystems"
        |> getV10 SystemsUrl

    /// Retrieve a single unit by external Id
    let getUnit (unitExternalId: string) (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<UnitItem>> =
        source |> withLogMessage "units:getUnit" |> getByExternalId unitExternalId Url

    /// Retrieves multiple units by external ID
    let retrieveUnits
        (items: string seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<UnitItem seq> =
            http {
                let url = Url +/ "byids"
                
                let request =
                    ItemsWithoutCursor(Items = (items |> Seq.map (fun id -> Identity(id))))
                    
                let! ret =
                    source
                    |> withLogMessage "units:retrieve"
                    |> postV10<_, ItemsWithoutCursor<_>> request url
                
                return ret.Items
            }