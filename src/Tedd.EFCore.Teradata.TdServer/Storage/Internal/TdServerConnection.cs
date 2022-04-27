// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
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
    public class TdServerConnection : RelationalConnection, ITdServerConnection, IAsyncDisposable
    {
        //private bool? _multipleActiveResultSetsEnabled;

        // Compensate for slow SQL Server database creation
        private const int DefaultMasterConnectionCommandTimeout = 60;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TdServerConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override DbConnection CreateDbConnection() => new TdConnection(ConnectionString);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ITdServerConnection CreateMasterConnection()
        {
            var connectionStringBuilder = new TdConnectionStringBuilder(ConnectionString)
            {
                // [Tedd] TODO: no master
                Database = "DBC"
            };
            //connectionStringBuilder.Remove("AttachDBFilename");

            var contextOptions = new DbContextOptionsBuilder()
                .UseTdServer(
                    connectionStringBuilder.ConnectionString,
                    b => b.CommandTimeout(CommandTimeout ?? DefaultMasterConnectionCommandTimeout))
                .Options;

            return new TdServerConnection(Dependencies.With(contextOptions));
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool IsMultipleActiveResultSetsEnabled
            => false;
        //(bool)(_multipleActiveResultSetsEnabled
        //              ?? (_multipleActiveResultSetsEnabled
        //                  = new TdConnectionStringBuilder(ConnectionString).MultipleActiveResultSets));

        /// <summary>
        ///     Indicates whether the store connection supports ambient transactions
        /// </summary>
        protected override bool SupportsAmbientTransactions => false; // [Tedd] TODO: I can't find anything googling Teradata ambient transaction
    }
}
