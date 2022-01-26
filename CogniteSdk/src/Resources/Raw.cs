// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all raw methods.
    /// </summary>
    public class RawResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal RawResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Unit, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Exception, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, Task<Unit>>, Task<Unit>>>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of RAW databases matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of databases matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<RawDatabase>> ListDatabasesAsync(RawDatabaseQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Raw.listDatabases(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of RAW databases matching query.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of databases matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<RawDatabase>> ListDatabasesAsync(CancellationToken token = default)
        {
            var query = new RawDatabaseQuery();
            var req = Oryx.Cognite.Raw.listDatabases(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// /// Create new databases in the given project.
        /// </summary>
        /// <param name="items">Databases to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created databases.</returns>
        public async Task<IEnumerable<RawDatabase>> CreateDatabasesAsync(IEnumerable<RawDatabase> items, CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var req = Oryx.Cognite.Raw.createDatabases(items, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// /// Create new databases in the given project.
        /// </summary>
        /// <param name="names">Names of databases to create.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of created databases.</returns>
        public async Task<IEnumerable<RawDatabase>> CreateDatabasesAsync(IEnumerable<string> names, CancellationToken token = default)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var items = names.Select(name => new RawDatabase { Name = name });
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
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Raw.deleteDatabases(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
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
            if (databaseNames is null)
            {
                throw new ArgumentNullException(nameof(databaseNames));
            }

            var dto = new RawDatabaseDelete()
            {
                Recursive = recursive,
                Items = databaseNames.Select(name => new RawDatabase { Name = name })
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
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Raw.listTables(database, query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// List tables in database.
        /// </summary>
        /// <param name="database">The database to list tables from.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of tables.</returns>
        public async Task<ItemsWithCursor<RawTable>> ListTablesAsync(string database, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

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
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var req = Oryx.Cognite.Raw.createTables(database, items, ensureParent, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
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
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (tables is null)
            {
                throw new ArgumentNullException(nameof(tables));
            }

            var items = tables.Select(name => new RawTable { Name = name });
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
            if (database is null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Raw.deleteTables(database, query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
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
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (tables is null)
            {
                throw new ArgumentNullException(nameof(tables));
            }

            var dto = new RawTableDelete() { Items = tables.Select(table => new RawTable { Name = table })};
            return await DeleteTablesAsync(database, dto, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve rows from a table.
        /// </summary>
        /// <param name="database">The database to list rows from.</param>
        /// <param name="table">The table to list rows from.</param>
        /// <param name="query">The Row query.</param>
        /// <param name="options">Optional json serializer options</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<ItemsWithCursor<RawRow<T>>> ListRowsAsync<T>(string database, string table, RawRowQuery query, JsonSerializerOptions options = null, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (options == null)
            {
                var req = Oryx.Cognite.Raw.listRows<T>(database, table, query, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Oryx.Cognite.Raw.listRowsWithOptions<T>(database, table, query, options, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieve rows from a table.
        /// </summary>
        /// <param name="database">The database to list rows from.</param>
        /// <param name="table">The table to list rows from.</param>
        /// <param name="options">Optional json serializer options</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<ItemsWithCursor<RawRow<T>>> ListRowsAsync<T>(string database, string table, JsonSerializerOptions options = null, CancellationToken token = default)
        {
            var query = new RawRowQuery();
            return await ListRowsAsync<T>(database, table, query, options, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a single row from a table.
        /// </summary>
        /// <param name="database">The database to list rows from.</param>
        /// <param name="table">The table to list rows from.</param>
        /// <param name="key">Key for row to get.</param>
        /// <param name="options">Optional json serializer options</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The retrieved rows.</returns>
        public async Task<RawRow<T>> GetRowAsync<T>(string database, string table, string key, JsonSerializerOptions options = null, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("message", nameof(key));
            }

            if (options == null)
            {
                var req = Oryx.Cognite.Raw.getRow<T>(database, table, key, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Oryx.Cognite.Raw.getRowWithOptions<T>(database, table, key, options, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieve a list of cursors for parallel reads.
        /// </summary>
        /// <param name="database">The database to retrieve rows from.</param>
        /// <param name="table">The table to retrieve rows from.</param>
        /// <param name="query">Query object</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A list of cursors</returns>
        public async Task<IEnumerable<string>> RetrieveCursorsForParallelReads(string database, string table, RawRowCursorsQuery query, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            var req = Oryx.Cognite.Raw.retrieveCursors(database, table, query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of cursors for parallel reads.
        /// </summary>
        /// <param name="database">The database to retrieve rows from.</param>
        /// <param name="table">The table to retrieve rows from.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A list of cursors</returns>
        public async Task<IEnumerable<string>> RetrieveCursorsForParallelReads(string database, string table, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            var query = new RawRowCursorsQuery();

            var req = Oryx.Cognite.Raw.retrieveCursors(database, table, query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create rows in a table.
        /// </summary>
        /// <param name="database">The database to create rows from.</param>
        /// <param name="table">The table to create rows from.</param>
        /// <param name="dtos">The rows to create</param>
        /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
        /// <param name="options">Optional json serializer options</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>An empty response.</returns>
        public async Task<EmptyResponse> CreateRowsAsync(string database, string table, IEnumerable<RawRowCreate<Dictionary<string, JsonElement>>> dtos, bool ensureParent = false, JsonSerializerOptions options = null, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            if (dtos is null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (options == null)
            {
                var req = Oryx.Cognite.Raw.createRows(database, table, dtos, ensureParent, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Oryx.Cognite.Raw.createRowsWithOptions(database, table, dtos, ensureParent, options, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Create rows in a table.
        /// </summary>
        /// <param name="database">The database to create rows from.</param>
        /// <param name="table">The table to create rows from.</param>
        /// <param name="dtos">The rows to create</param>
        /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
        /// <param name="options">Optional json serializer options</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>An empty response.</returns>
        public async Task<EmptyResponse> CreateRowsAsync<T>(string database, string table, IEnumerable<RawRowCreate<T>> dtos, bool ensureParent = false, JsonSerializerOptions options = null, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            if (dtos is null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (options == null)
            {
                var req = Oryx.Cognite.Raw.createRows<T>(database, table, dtos, ensureParent, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Oryx.Cognite.Raw.createRowsWithOptions<T>(database, table, dtos, ensureParent, options, _ctx);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
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
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentException("message", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException("message", nameof(table));
            }

            if (dtos is null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            var req = Oryx.Cognite.Raw.deleteRows(database, table, dtos, _ctx);
            return await RunAsync<EmptyResponse>(req, token).ConfigureAwait(false);
        }
    }
}
