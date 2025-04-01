// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

using Oryx;
using Oryx.Cognite;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all annotations methods.
    /// </summary>
    public class AnnotationsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal AnnotationsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create Annotation definitions.
        /// </summary>
        /// <param name="annotations">List of annotation definitions to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created suggestions</returns>
        public async Task<IEnumerable<Annotation>> CreateAsync(IEnumerable<AnnotationCreate> annotations, CancellationToken token = default)
        {
            if (annotations is null) throw new ArgumentNullException(nameof(annotations));

            var req = Annotations.create(annotations, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Suggest Annotation definitions.
        /// </summary>
        /// <param name="annotations">List of annotation definitions to suggest</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created suggestions</returns>
        public async Task<IEnumerable<Annotation>> SuggestAsync(IEnumerable<AnnotationSuggest> annotations, CancellationToken token = default)
        {
            if (annotations is null) throw new ArgumentNullException(nameof(annotations));

            var req = Annotations.suggest(annotations, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple Annotations in the same project
        /// </summary>
        /// <param name="items">The ids of Annotations to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(AnnotationDelete items, CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var req = Annotations.delete(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of annotations matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of annotations matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Annotation>> ListAsync(AnnotationQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var req = Annotations.list<Annotation>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously update one or more annotations. Supports partial updates, meaning that fields omitted from the
        /// requests are not changed
        /// </summary>
        /// <param name="query">The list of annotations to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of updated annotations.</returns>
        public async Task<IEnumerable<Annotation>> UpdateAsync(IEnumerable<AnnotationUpdateItem> query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Annotations.update<Annotation>(query, GetContext(token));
            var ret = await RunAsync(req).ConfigureAwait(false);
            return ret;
        }
    }
}
