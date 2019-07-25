// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Tedd.EFCore.Teradata.Metadata.Conventions;
using Tedd.EFCore.Teradata.Migrations;
using Tedd.EFCore.Teradata.TdServer.Diagnostics.Internal;
using Tedd.EFCore.Teradata.TdServer.Infrastructure.Internal;
using Tedd.EFCore.Teradata.TdServer.Internal;
using Tedd.EFCore.Teradata.TdServer.Migrations.Internal;
using Tedd.EFCore.Teradata.TdServer.Query.Internal;
using Tedd.EFCore.Teradata.TdServer.Query.Pipeline;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Tedd.EFCore.Teradata.TdServer.Update.Internal;
using Tedd.EFCore.Teradata.TdServer.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class TdServerServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the Microsoft SQL Server database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />. You use this method when using dependency injection
        ///         in your application, such as with ASP.NET. For more information on setting up dependency
        ///         injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        ///     <para>
        ///         You only need to use this functionality when you want Entity Framework to resolve the services it uses
        ///         from an external dependency injection container. If you are not using an external
        ///         dependency injection container, Entity Framework will take care of creating the services it requires.
        ///     </para>
        /// </summary>
        /// <example>
        ///     <code>
        ///            public void ConfigureServices(IServiceCollection services)
        ///            {
        ///                var connectionString = "connection string to database";
        ///
        ///                services
        ///                    .AddEntityFrameworkTdServer()
        ///                    .AddDbContext&lt;MyContext&gt;((serviceProvider, options) =>
        ///                        options.UseTdServer(connectionString)
        ///                               .UseInternalServiceProvider(serviceProvider));
        ///            }
        ///        </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkTdServer([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, TdServerLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<TdServerOptionsExtension>>()
                .TryAdd<IValueGeneratorCache>(p => p.GetService<ITdServerValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, TdServerTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, TdServerSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, TdServerMigrationsAnnotationProvider>()
                .TryAdd<IModelValidator, TdServerModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, TdServerConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<ITdServerUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, TdServerModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, TdServerValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<ITdServerConnection>())
                .TryAdd<IMigrationsSqlGenerator, TdServerMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, TdServerDatabaseCreator>()
                .TryAdd<IHistoryRepository, TdServerHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, TdServerCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, TdServerExecutionStrategyFactory>()
                .TryAdd<ISingletonOptions, ITdServerOptions>(p => p.GetService<ITdServerOptions>())

                // New Query Pipeline
                .TryAdd<IMethodCallTranslatorProvider, TdServerMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, TdServerMemberTranslatorProvider>()
                .TryAdd<IQuerySqlGeneratorFactory, TdServerQuerySqlGeneratorFactory>()
                .TryAdd<IShapedQueryOptimizerFactory, TdServerShapedQueryOptimizerFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, TdServerSqlTranslatingExpressionVisitorFactory>()


                .TryAddProviderSpecificServices(
                    b => b
                        .TryAddSingleton<ITdServerValueGeneratorCache, TdServerValueGeneratorCache>()
                        .TryAddSingleton<ITdServerOptions, TdServerOptions>()
                        .TryAddSingleton<ITdServerUpdateSqlGenerator, TdServerUpdateSqlGenerator>()
                        .TryAddSingleton<ITdServerSequenceValueGeneratorFactory, TdServerSequenceValueGeneratorFactory>()
                        .TryAddScoped<ITdServerConnection, TdServerConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
