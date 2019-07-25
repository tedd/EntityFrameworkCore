// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Tedd.EFCore.Teradata.Migrations.Operations;
using Tedd.EFCore.Teradata.TdServer.Internal;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Teradata.Client.Provider;

namespace Tedd.EFCore.Teradata.TdServer.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    public class TdServerDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly ITdServerConnection _connection;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TdServerDatabaseCreator(
            [NotNull] RelationalDatabaseCreatorDependencies dependencies,
            [NotNull] ITdServerConnection connection,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder)
            : base(dependencies)
        {
            _connection = connection;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TimeSpan RetryTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                Dependencies.MigrationCommandExecutor
                    .ExecuteNonQuery(CreateCreateOperations(), masterConnection);

                ClearPool();
            }

            Exists(retryOnNotExists: true);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override async Task CreateAsync(CancellationToken cancellationToken = default)
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await Dependencies.MigrationCommandExecutor
                    .ExecuteNonQueryAsync(CreateCreateOperations(), masterConnection, cancellationToken);

                ClearPool();
            }

            await ExistsAsync(retryOnNotExists: true, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool HasTables()
            => Dependencies.ExecutionStrategyFactory
                .Create()
                .Execute(
                    _connection,
                    connection => (int)CreateHasTablesCommand()
                                      .ExecuteScalar(
                                          new RelationalCommandParameterObject(
                                              connection,
                                              null,
                                              Dependencies.CurrentDbContext.Context,
                                              Dependencies.CommandLogger)) != 0);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(
                _connection,
                async (connection, ct) => (int)await CreateHasTablesCommand()
                                              .ExecuteScalarAsync(
                                                  new RelationalCommandParameterObject(
                                                      connection,
                                                      null,
                                                      Dependencies.CurrentDbContext.Context,
                                                      Dependencies.CommandLogger),
                                                  cancellationToken: ct) != 0, cancellationToken);

        private IRelationalCommand CreateHasTablesCommand()
            => _rawSqlCommandBuilder
                .Build(@"
SELECT COUNT(*)
FROM (SELECT TableName FROM dbc.TablesV WHERE DataBaseName=(SELECT DATABASE) SAMPLE 1) AS Tables");

        private IReadOnlyList<MigrationCommand> CreateCreateOperations()
        {
            var builder = new TdConnectionStringBuilder(_connection.DbConnection.ConnectionString);
            return Dependencies.MigrationsSqlGenerator.Generate(
                new[]
                {
                    new TdServerCreateDatabaseOperation
                    {
                        Name = builder.Database,
                        // [Tedd] TODO: Disabled so it compiles
                        //FileName = builder.AttachDBFilename
                    }
                });
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool Exists()
            => Exists(retryOnNotExists: false);

        private bool Exists(bool retryOnNotExists)
            => Dependencies.ExecutionStrategyFactory.Create().Execute(
                DateTime.UtcNow + RetryTimeout, giveUp =>
                {
                    while (true)
                    {
                        try
                        {
                            using (new TransactionScope(TransactionScopeOption.Suppress))
                            {
                                _connection.Open(errorsExpected: true);
                                _connection.Close();
                            }

                            return true;
                        }
                        catch (TdException e)
                        {
                            if (!retryOnNotExists
                                && IsDoesNotExist(e))
                            {
                                return false;
                            }

                            if (DateTime.UtcNow > giveUp
                                || !RetryOnExistsFailure(e))
                            {
                                throw;
                            }

                            Thread.Sleep(RetryDelay);
                        }
                    }
                });

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
            => ExistsAsync(retryOnNotExists: false, cancellationToken: cancellationToken);

        private Task<bool> ExistsAsync(bool retryOnNotExists, CancellationToken cancellationToken)
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(
                DateTime.UtcNow + RetryTimeout, async (giveUp, ct) =>
                {
                    while (true)
                    {
                        try
                        {
                            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                            {
                                await _connection.OpenAsync(ct, errorsExpected: true);

                                await _connection.CloseAsync();
                            }

                            return true;
                        }
                        catch (TdException e)
                        {
                            if (!retryOnNotExists
                                && IsDoesNotExist(e))
                            {
                                return false;
                            }

                            if (DateTime.UtcNow > giveUp
                                || !RetryOnExistsFailure(e))
                            {
                                throw;
                            }

                            await Task.Delay(RetryDelay, ct);
                        }
                    }
                }, cancellationToken);

        // Login failed is thrown when database does not exist (See Issue #776)
        // Unable to attach database file is thrown when file does not exist (See Issue #2810)
        // Unable to open the physical file is thrown when file does not exist (See Issue #2810)
        // [Tedd] TODO: These numbers are for MS SQL
        private static bool IsDoesNotExist(TdException exception) =>
            exception.ErrorCode == 4060 || exception.ErrorCode == 1832 || exception.ErrorCode == 5120;

        // See Issue #985
        private bool RetryOnExistsFailure(TdException exception)
        {
            // This is to handle the case where Open throws (Number 233):
            //   Microsoft.Data.SqlClient.SqlException: A connection was successfully established with the
            //   server, but then an error occurred during the login process. (provider: Named Pipes
            //   Provider, error: 0 - No process is on the other end of the pipe.)
            // It appears that this happens when the database has just been created but has not yet finished
            // opening or is auto-closing when using the AUTO_CLOSE option. The workaround is to flush the pool
            // for the connection and then retry the Open call.
            // Also handling (Number -2):
            //   Microsoft.Data.SqlClient.SqlException: Connection Timeout Expired.  The timeout period elapsed while
            //   attempting to consume the pre-login handshake acknowledgment.  This could be because the pre-login
            //   handshake failed or the server was unable to respond back in time.
            // And (Number 4060):
            //   Microsoft.Data.SqlClient.SqlException: Cannot open database "X" requested by the login. The
            //   login failed.
            // And (Number 1832)
            //   Microsoft.Data.SqlClient.SqlException: Unable to Attach database file as database xxxxxxx.
            // And (Number 5120)
            //   Microsoft.Data.SqlClient.SqlException: Unable to open the physical file xxxxxxx.
            // [Tedd] TODO: These numbers are for MS SQL
            if (exception.ErrorCode == 233
                || exception.ErrorCode == -2
                || exception.ErrorCode == 4060
                || exception.ErrorCode == 1832
                || exception.ErrorCode == 5120)
            {
                ClearPool();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void Delete()
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                Dependencies.MigrationCommandExecutor
                    .ExecuteNonQuery(CreateDropCommands(), masterConnection);
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await Dependencies.MigrationCommandExecutor
                    .ExecuteNonQueryAsync(CreateDropCommands(), masterConnection, cancellationToken);
            }
        }

        private IReadOnlyList<MigrationCommand> CreateDropCommands()
        {
            var databaseName = _connection.DbConnection.Database;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException(TdServerStrings.NoInitialCatalog);
            }

            var operations = new MigrationOperation[]
            {
                new TdServerDropDatabaseOperation
                {
                    Name = databaseName
                }
            };

            return Dependencies.MigrationsSqlGenerator.Generate(operations);
        }

        // [Tedd] TODO: Disabled these
        // Clear connection pools in case there are active connections that are pooled
        private static void ClearAllPools() {}

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        private void ClearPool() {}

    //// Clear connection pools in case there are active connections that are pooled
    //private static void ClearAllPools() => TdConnection.ClearAllPools();

    //// Clear connection pool for the database connection since after the 'create database' call, a previously
    //// invalid connection may now be valid.
    //private void ClearPool() => TdConnection.ClearPool((TdConnection)_connection.DbConnection);
}
}
