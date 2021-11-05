// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Class to write a row to a table in Raw.
    /// </summary>
    public class RawRowCreate<T> : RawRowCreateJson<Dictionary<string, T>>
    {
    }
}
