// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// The row read class.
    /// </summary>
    public class RawRow<T> : RawRowJson<Dictionary<string, T>>
    {
    }
}
