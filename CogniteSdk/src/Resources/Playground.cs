// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class PlaygroundResource : Resource
    {
        /// <summary>
        /// Client Functions extension methods
        /// </summary>
        public Playground.FunctionResource Functions { get; set; }

        /// <summary>
        /// Client FunctionCalls extension methods.
        /// </summary>
        /// <value></value>
        public Playground.FunctionCallResource FunctionCalls { get; set; }

        /// <summary>
        /// Client FunctionSchedules extension methods
        /// </summary>
        public Playground.FunctionScheduleResource FunctionSchedules { get; set; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal PlaygroundResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
            Functions = new Playground.FunctionResource(authHandler, ctx);
            FunctionCalls = new Playground.FunctionCallResource(authHandler, ctx);
            FunctionSchedules = new Playground.FunctionScheduleResource(authHandler, ctx);
        }
    }
}