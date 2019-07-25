// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tedd.EFCore.Teradata.Infrastructure;
using Teradata.Client.Provider;
using Tedd.EFCore.Teradata;

#pragma warning disable IDE0022 // Use block body for methods
// ReSharper disable SuggestBaseTypeForParameter
namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TdServerTestStore : RelationalTestStore
    {
        public const int CommandTimeout = 600;
        private static string CurrentDirectory => Environment.CurrentDirectory;

        public static TdServerTestStore GetNorthwindStore()
            => (TdServerTestStore)TdServerNorthwindTestStoreFactory.Instance
                .GetOrCreate(TdServerNorthwindTestStoreFactory.Name).Initialize(null, (Func<DbContext>)null, null, null);

        public static TdServerTestStore GetOrCreate(string name)
            => new TdServerTestStore(name);

        public static TdServerTestStore GetOrCreateInitialized(string name)
            => new TdServerTestStore(name).InitializeTdServer(null, (Func<DbContext>)null, null);

        public static TdServerTestStore GetOrCreate(string name, string scriptPath)
            => new TdServerTestStore(name, scriptPath: scriptPath);

        public static TdServerTestStore Create(string name, bool useFileName = false)
            => new TdServerTestStore(name, useFileName, shared: false);

        public static TdServerTestStore CreateInitialized(string name, bool useFileName = false, bool? multipleActiveResultSets = null)
            => new TdServerTestStore(name, useFileName, shared: false, multipleActiveResultSets: multipleActiveResultSets)
                .InitializeTdServer(null, (Func<DbContext>)null, null);

        private readonly string _scriptPath;

        private TdServerTestStore(
            string name,
            bool useFileName = false,
            bool? multipleActiveResultSets = null,
            string scriptPath = null,
            bool shared = true)
            : base(name, shared)
        {
            if (scriptPath != null)
            {
                _scriptPath = Path.Combine(Path.GetDirectoryName(typeof(TdServerTestStore).GetTypeInfo().Assembly.Location), scriptPath);
            }

            ConnectionString = CreateConnectionString(Name, multipleActiveResultSets);
            Connection = new TdConnection(ConnectionString);
        }

        public TdServerTestStore InitializeTdServer(
            IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (TdServerTestStore)Initialize(serviceProvider, createContext, seed, null);

        public TdServerTestStore InitializeTdServer(
            IServiceProvider serviceProvider, Func<TdServerTestStore, DbContext> createContext, Action<DbContext> seed)
            => InitializeTdServer(serviceProvider, () => createContext(this), seed);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            if (CreateDatabase(clean))
            {
                if (_scriptPath != null)
                {
                    ExecuteScript(_scriptPath);
                }
                else
                {
                    using (var context = createContext())
                    {
                        context.Database.EnsureCreatedResiliently();
                        seed?.Invoke(context);
                    }
                }
            }
        }

        public virtual DbContextOptionsBuilder AddProviderOptions(
            DbContextOptionsBuilder builder,
            Action<TdServerDbContextOptionsBuilder> configureTdServer)
            => builder.UseTdServer(Connection, b => configureTdServer?.Invoke(b));

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => AddProviderOptions(builder, configureTdServer: null);

        private bool CreateDatabase(Action<DbContext> clean)
        {
            using (var master = new TdConnection(CreateConnectionString("dbc", multipleActiveResultSets: false)))
            {
                if (ExecuteScalar<int>(master, $"SELECT COUNT(*) FROM dbc.DatabasesV WHERE DatabaseName = '{Name}'") > 0)
                {
                    if (_scriptPath != null)
                    {
                        return false;
                    }

                    //if (_fileName == null)
                    //{
                    //    using (var context = new DbContext(
                    //        AddProviderOptions(
                    //                new DbContextOptionsBuilder()
                    //                    .EnableServiceProviderCaching(false))
                    //            .Options))
                    //    {
                    //        clean?.Invoke(context);
                    //        Clean(context);
                    //        return true;
                    //    }
                    //}

                    // Delete the database to ensure it's recreated with the correct file path
                    DeleteDatabase();
                }

                ExecuteNonQuery(master, GetCreateDatabaseStatement(Name));
                WaitForExists((TdConnection)Connection);
            }

            return true;
        }

        public override void Clean(DbContext context)
        { }
        //=> context.Database.EnsureClean();

        public void ExecuteScript(string scriptPath)
        {
            var script = File.ReadAllText(scriptPath);
            Execute(
                Connection, command =>
                {
                    foreach (var batch in
                        new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                            .Split(script).Where(b => !string.IsNullOrEmpty(b)))
                    {
                        command.CommandText = batch;
                        command.ExecuteNonQuery();
                    }

                    return 0;
                }, "");
        }

        private static void WaitForExists(TdConnection connection)
        {
            //if (TestEnvironment.IsSqlAzure)
            //{
            //    new TestTdServerRetryingExecutionStrategy().Execute(connection, WaitForExistsImplementation);
            //}
            //else
            //{
            WaitForExistsImplementation(connection);
            //}
        }

        private static void WaitForExistsImplementation(TdConnection connection)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }

                    //TdConnection.ClearPool(connection);

                    connection.Open();
                    connection.Close();
                    return;
                }
                catch (TdException e)
                {
                    // [Tedd] TODO: Error codes are from MSSQL
                    if (++retryCount >= 30
                        || e.ErrorCode != 233 && e.ErrorCode != -2 && e.ErrorCode != 4060 && e.ErrorCode != 1832 && e.ErrorCode != 5120)
                    {
                        throw;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private static string GetCreateDatabaseStatement(string name)
        {
            var result = $"CREATE DATABASE {name} AS PERMANENT = 1000000 BYTES;";

            //if (TestEnvironment.IsSqlAzure)
            //{
            //    var elasticGroupName = TestEnvironment.ElasticPoolName;
            //    result += Environment.NewLine +
            //              (string.IsNullOrEmpty(elasticGroupName)
            //                  ? " ( Edition = 'basic' )"
            //                  : $" ( SERVICE_OBJECTIVE = ELASTIC_POOL ( name = {elasticGroupName} ) )");
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(fileName))
            //    {
            //        var logFileName = Path.ChangeExtension(fileName, ".ldf");
            //        result += Environment.NewLine +
            //                  $" ON (NAME = '{name}', FILENAME = '{fileName}')" +
            //                  $" LOG ON (NAME = '{name}_log', FILENAME = '{logFileName}')";
            //    }
            //}

            return result;
        }

        public void DeleteDatabase()
        {
            using (var master = new TdConnection(CreateConnectionString("dbc")))
            {
                ExecuteNonQuery(master, string.Format("DELETE DATABASE {0};", Name));
                ExecuteNonQuery(master, string.Format("DROP DATABASE {0}; ", Name));

                //TdConnection.ClearAllPools();
            }
        }

        public override void OpenConnection()
        {
            //if (TestEnvironment.IsSqlAzure)
            //{
            //    new TestTdServerRetryingExecutionStrategy().Execute(Connection, connection => connection.Open());
            //}
            //else
            {
                Connection.Open();
            }
        }

        public override Task OpenConnectionAsync() => Connection.OpenAsync();
        //=> TestEnvironment.IsSqlAzure
        //    ? new TestTdServerRetryingExecutionStrategy().ExecuteAsync(Connection, connection => connection.OpenAsync())
        //    : Connection.OpenAsync();

        public T ExecuteScalar<T>(string sql, params object[] parameters)
            => ExecuteScalar<T>(Connection, sql, parameters);

        private static T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => (T)command.ExecuteScalar(), sql, false, parameters);

        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] parameters)
            => ExecuteScalarAsync<T>(Connection, sql, parameters);

        private static Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, async command => (T)await command.ExecuteScalarAsync(), sql, false, parameters);

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(Connection, sql, parameters);

        private static int ExecuteNonQuery(DbConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
            => ExecuteNonQueryAsync(Connection, sql, parameters);

        private static Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, command => command.ExecuteNonQueryAsync(), sql, false, parameters);

        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
            => Query<T>(Connection, sql, parameters);

        private static IEnumerable<T> Query<T>(DbConnection connection, string sql, object[] parameters = null)
            => Execute(
                connection, command =>
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        var results = Enumerable.Empty<T>();
                        while (dataReader.Read())
                        {
                            results = results.Concat(new[] { dataReader.GetFieldValue<T>(0) });
                        }

                        return results;
                    }
                }, sql, false, parameters);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
            => QueryAsync<T>(Connection, sql, parameters);

        private static Task<IEnumerable<T>> QueryAsync<T>(DbConnection connection, string sql, object[] parameters = null)
            => ExecuteAsync(
                connection, async command =>
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var results = Enumerable.Empty<T>();
                        while (await dataReader.ReadAsync())
                        {
                            results = results.Concat(new[] { await dataReader.GetFieldValueAsync<T>(0) });
                        }

                        return results;
                    }
                }, sql, false, parameters);

        private static T Execute<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => new TestTdServerRetryingExecutionStrategy().Execute(
                    new
                    {
                        connection,
                        execute,
                        sql,
                        useTransaction,
                        parameters
                    },
                    state => ExecuteCommand(state.connection, state.execute, state.sql, state.useTransaction, state.parameters));

        private static T ExecuteCommand<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql, bool useTransaction, object[] parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            connection.Open();
            try
            {
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        command.Transaction = transaction;
                        result = execute(command);
                    }

                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static Task<T> ExecuteAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql,
            bool useTransaction = false, IReadOnlyList<object> parameters = null)
            => TestEnvironment.IsSqlAzure
                ? new TestTdServerRetryingExecutionStrategy().ExecuteAsync(
                    new
                    {
                        connection,
                        executeAsync,
                        sql,
                        useTransaction,
                        parameters
                    },
                    state => ExecuteCommandAsync(state.connection, state.executeAsync, state.sql, state.useTransaction, state.parameters))
                : ExecuteCommandAsync(connection, executeAsync, sql, useTransaction, parameters);

        private static async Task<T> ExecuteCommandAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql, bool useTransaction,
            IReadOnlyList<object> parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            await connection.OpenAsync();
            try
            {
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        result = await executeAsync(command);
                    }

                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static DbCommand CreateCommand(
            DbConnection connection, string commandText, IReadOnlyList<object> parameters = null)
        {
            var command = (TdCommand)connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    command.Parameters.Add(new TdParameter("p" + i, parameters[i]));
                }
            }

            return command;
        }

        public override void Dispose()
        {
            base.Dispose();

            // Clean up the database using a local file, as it might get deleted later
            DeleteDatabase();
        }

        public static string CreateConnectionString(string name, bool? multipleActiveResultSets = null)
        {
            var builder = new TdConnectionStringBuilder(TestEnvironment.DefaultConnection)
            {
                //MultipleActiveResultSets = multipleActiveResultSets ?? new Random().Next(0, 2) == 1,
                //InitialCatalog = name
                Database = name
            };
            //if (fileName != null)
            //{
            //    builder.AttachDBFilename = fileName;
            //}

            return builder.ToString();
        }
    }
}
