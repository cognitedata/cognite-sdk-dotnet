// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk;
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
        public async Task<ItemsWithCursor<RawDatabase>> ListDatabasesAsync(RawDatabaseQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listDatabases<ItemsWithCursor<RawDatabase>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of RAW databases matching query.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of databases matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<RawDatabase>> ListDatabasesAsync(CancellationToken token = default)
        {
            var query = new RawDatabaseQuery();
            var req = Oryx.Cognite.Raw.listDatabases<ItemsWithCursor<RawDatabase>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// /// Create new databases in the given project.
        /// </summary>
        /// <param name="items">Databases to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created databases.</returns>
        public async Task<IEnumerable<RawDatabase>> CreateDatabasesAsync(IEnumerable<RawDatabase> items, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.createDatabases<IEnumerable<RawDatabase>>(items);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// /// Create new databases in the given project.
        /// </summary>
        /// <param name="names">Names of databases to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created databases.</returns>
        public async Task<IEnumerable<RawDatabase>> CreateDatabasesAsync(IEnumerable<string> names, CancellationToken token = default)
        {
            var items = names.Select(name => new RawDatabase() { Name = name });
            return await CreateDatabasesAsync(items, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple databases in the same project,
        /// </summary>
        /// <param name="query">The list of databases to delete.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteDatabasesAsync(RawDatabaseDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.deleteDatabases<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple databases in the same project,
        /// </summary>
        /// <param name="databaseNames">The names of the databases to delete.</param>
        /// <param name="recursive">Default: false. When true, tables of this database are deleted with the database..</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteDatabasesAsync(IEnumerable<string> databaseNames, bool recursive = false, CancellationToken token = default)
        {
            var dto = new RawDatabaseDelete()
            {
                Recursive = recursive,
                Items = databaseNames.Select(name => new RawDatabase() { Name = name })
            };
            return await DeleteDatabasesAsync(dto, token).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="query">The query with optional limit and cursor.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of tables.</returns>
        public async Task<ItemsWithCursor<RawTable>> ListTablesAsync(string database, RawDatabaseQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listTables<ItemsWithCursor<RawTable>>(database, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of tables.</returns>
        public async Task<ItemsWithCursor<RawTable>> ListTablesAsync(string database, CancellationToken token = default)
        {
            var query = new RawDatabaseQuery();
            return await ListTablesAsync(database, query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="items">The tables to create.</param>
        /// <param name="ensureParent">Default: false. Create database if it doesn't exist already</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created tables.</returns>
        public async Task<IEnumerable<RawTable>> CreateTablesAsync(string database, IEnumerable<RawTable> items, bool ensureParent, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.createTables<IEnumerable<RawTable>>(database, items, ensureParent);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="tables">The names of tables to create.</param>
        /// <param name="ensureParent">Default: false. Create database if it doesn't exist already</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created tables.</returns>
        public async Task<IEnumerable<RawTable>> CreateTablesAsync(string database, IEnumerable<string> tables, bool ensureParent, CancellationToken token = default)
        {
            var items = tables.Select(name => new RawTable() { Name = name });
            return await CreateTablesAsync(database, items, ensureParent, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple tables in the same database.
        /// </summary>
        /// <param name="database">The database to delete tables from.</param>
        /// <param name="query">The query with tables to delete.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty result.</returns>
        public async Task<EmptyResponse> DeleteTablesAsync(string database, RawTableDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.deleteTables<EmptyResponse>(database, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple tables in the same database.
        /// </summary>
        /// <param name="database">The database to delete tables from.</param>
        /// <param name="tables">The names of the tables to delete.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Empty result.</returns>

        public async Task<EmptyResponse> DeleteTablesAsync(string database, IEnumerable<string> tables, CancellationToken token = default)
        {
            var dto = new RawTableDelete() { Items = tables.Select(table => new RawTable() { Name = table })};
            return await DeleteTablesAsync(database, dto, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve rows from a table.
        /// </summary>
        /// <param name="database">The database to list rows from.</param>
        /// <param name="table">The table to list rows from.</param>
        /// <param name="query">The Row query.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<ItemsWithCursor<RawRow>> ListRowsAsync(string database, string table, RawRowQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listRows<ItemsWithCursor<RawRow>>(database, table, query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve rows from a table.
        /// </summary>
        /// <param name="database">The database to list rows from.</param>
        /// <param name="table">The table to list rows from.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<ItemsWithCursor<RawRow>> ListRowsAsync(string database, string table, CancellationToken token = default)
        {
            var query = new RawRowQuery();
            return await ListRowsAsync(database, table, query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create rows in a table.
        /// </summary>
        /// <param name="database">The database to create rows from.</param>
        /// <param name="table">The table to create rows from.</param>
        /// <param name="dtos">The rows to create</param>
        /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>An empty response.</returns>
        public async Task<EmptyResponse> CreateRowsAsync(string database, string table, IEnumerable<RawRowCreate> dtos, bool ensureParent = false, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.createRows<EmptyResponse>(database, table, dtos, ensureParent);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete rows in a table.
        /// </summary>
        /// <param name="database">The database to delete rows from.</param>
        /// <param name="table">The table to delete rows from.</param>
        /// <param name="dtos">The rows to delete</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>An empty response</returns>
        public async Task<EmptyResponse> DeleteRowsAsync(string database, string table, IEnumerable<RawRowDelete> dtos, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.deleteRows<EmptyResponse>(database, table, dtos);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}