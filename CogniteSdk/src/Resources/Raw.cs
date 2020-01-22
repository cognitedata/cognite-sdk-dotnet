// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Raw;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all raw methods.
    /// </summary>
    public class RawResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal RawResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of RAW databases matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of databases matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<DatabaseDto>> ListDatabasesAsync(DatabaseQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listDatabases<ItemsWithCursor<DatabaseDto>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// /// Create new databases in the given project.
        /// </summary>
        /// <param name="items">Databases to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created databases.</returns>
        public async Task<IEnumerable<DatabaseDto>> CreateDatabasesAsync(IEnumerable<DatabaseDto> items, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.createDatabases<IEnumerable<DatabaseDto>>(items);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple databases in the same project,
        /// </summary>
        /// <param name="query">The list of databases to delete.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteDatabasesAsync(DatabaseDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.deleteDatabases<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="query">The query with optional limit and cursor.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of tables.</returns>
        public async Task<ItemsWithCursor<TableDto>> ListTablesAsync(string database, DatabaseQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listTables<ItemsWithCursor<TableDto>>(database, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="items">The tables to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created tables.</returns>
        public async Task<ItemsWithoutCursor<TableDto>> CreateTablesAsync(string database, ItemsWithoutCursor<TableDto> items, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.createTables<ItemsWithoutCursor<TableDto>>(database, items);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple tables in the same database.
        /// </summary>
        /// <param name="database">The database to delete tables from.</param>
        /// <param name="query">The query with tables to delete.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty result.</returns>
        public async Task<EmptyResponse> DeleteTablesAsync(string database, TableDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.deleteTables<EmptyResponse>(database, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve rows from a table.
        /// </summary>
        /// <param name="database">The ids of the events to get.</param>
        /// <param name="table">The ids of the events to get.</param>
        /// <param name="query">The Row query.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<ItemsWithCursor<RowReadDto>> RetrieveRowsAsync(string database, string table, RowQueryDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.retrieveRows<ItemsWithCursor<RowReadDto>>(database, table, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}