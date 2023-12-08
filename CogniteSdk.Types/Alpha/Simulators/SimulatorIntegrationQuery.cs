// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    public class SimulatorIntegrationQuery
    {
        //
        public SimulatorIntegrationFilter Filter { get; set; }

        public override string ToString() => Stringable.ToString(this);
    }


    public class SimulatorIntegrationFilter
    {
        public string[] simulatorExternalIds { get; set; }
    }
}