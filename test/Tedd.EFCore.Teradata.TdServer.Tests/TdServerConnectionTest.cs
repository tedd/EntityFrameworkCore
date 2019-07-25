// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Tedd.EFCore.Teradata.TdServer.Diagnostics.Internal;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Teradata.Client.Provider;

namespace Tedd.EFCore.Teradata
{
    public class TdServerConnectionTest
    {
        [ConditionalFact]
        public void Creates_SQL_Server_connection_string()
        {
            using (var connection = new TdServerConnection(CreateDependencies()))
            {
                Assert.IsType<TdConnection>(connection.DbConnection);
            }
        }

        [ConditionalFact]
        public void Can_create_master_connection()
        {
            using (var connection = new TdServerConnection(CreateDependencies()))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal("Database=DBC;" + Config.GetConnectionString(), master.ConnectionString);
                    Assert.Equal(60, master.CommandTimeout);
                }
            }
        }

       
        [ConditionalFact]
        public void Master_connection_string_none_default_command_timeout()
        {
            var options = new DbContextOptionsBuilder()
                .UseTdServer(
                   Config.GetConnectionString(),
                    b => b.CommandTimeout(55))
                .Options;

            using (var connection = new TdServerConnection(CreateDependencies(options)))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(55, master.CommandTimeout);
                }
            }
        }

        public static RelationalConnectionDependencies CreateDependencies(DbContextOptions options = null)
        {
            options ??= new DbContextOptionsBuilder()
                .UseTdServer(Config.GetConnectionString())
                .Options;

            return new RelationalConnectionDependencies(
                options,
                new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new TdServerLoggingDefinitions()),
                new DiagnosticsLogger<DbLoggerCategory.Database.Connection>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new TdServerLoggingDefinitions()),
                new NamedConnectionStringResolver(options),
                new RelationalTransactionFactory(new RelationalTransactionFactoryDependencies()),
                new CurrentDbContext(new FakeDbContext()));
        }

        private class FakeDbContext : DbContext
        {
        }
    }
}
