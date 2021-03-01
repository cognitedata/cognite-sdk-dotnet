// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

[<RequireQualifiedAccess>]
module DataSets =
    [<Literal>]
    let Url = "/datasets"

    /// <summary>
    /// Create new data sets in the given project.
    /// </summary>
    /// <param name="assets">The data sets to create.</param>
    /// <returns>List of created data sets.</returns>
    let create (items: DataSetCreate seq) : HttpHandler<unit, DataSet seq> =
        withLogMessage "DataSets:create"
        >=> create items Url
