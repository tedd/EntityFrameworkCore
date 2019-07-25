// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata.Metadata.Conventions
{
    /// <summary>
    ///     <para>
    ///         A builder for building conventions for SQL Server.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped" /> and multiple registrations
    ///         are allowed. This means that each <see cref="DbContext" /> instance will use its own
    ///         set of instances of this service.
    ///         The implementations may depend on other services registered with any lifetime.
    ///         The implementations do not need to be thread-safe.
    ///     </para>
    /// </summary>
    public class TdServerConventionSetBuilder : RelationalConventionSetBuilder
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        /// <summary>
        ///     Creates a new <see cref="TdServerConventionSetBuilder" /> instance.
        /// </summary>
        /// <param name="dependencies"> The core dependencies for this service. </param>
        /// <param name="relationalDependencies"> The relational dependencies for this service. </param>
        /// <param name="sqlGenerationHelper"> The SQL generation helper to use. </param>
        public TdServerConventionSetBuilder(
            [NotNull] ProviderConventionSetBuilderDependencies dependencies,
            [NotNull] RelationalConventionSetBuilderDependencies relationalDependencies,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper)
            : base(dependencies, relationalDependencies)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
        }

        /// <summary>
        ///     Builds and returns the convention set for the current database provider.
        /// </summary>
        /// <returns> The convention set for the current database provider. </returns>
        public override ConventionSet CreateConventionSet()
        {
            var conventionSet = base.CreateConventionSet();

            var valueGenerationStrategyConvention = new TdServerValueGenerationStrategyConvention(Dependencies, RelationalDependencies);
            conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);
            conventionSet.ModelInitializedConventions.Add(
                new RelationalMaxIdentifierLengthConvention(128, Dependencies, RelationalDependencies));

            ValueGenerationConvention valueGenerationConvention = new TdServerValueGenerationConvention(Dependencies, RelationalDependencies);
            ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, valueGenerationConvention);

            var TdServerInMemoryTablesConvention = new TdServerMemoryOptimizedTablesConvention(Dependencies, RelationalDependencies);
            conventionSet.EntityTypeAnnotationChangedConventions.Add(TdServerInMemoryTablesConvention);

            ReplaceConvention(conventionSet.EntityTypePrimaryKeyChangedConventions, valueGenerationConvention);

            conventionSet.KeyAddedConventions.Add(TdServerInMemoryTablesConvention);

            ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGenerationConvention);

            ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGenerationConvention);

            var TdServerIndexConvention = new TdServerIndexConvention(Dependencies, RelationalDependencies, _sqlGenerationHelper);

            conventionSet.EntityTypeBaseTypeChangedConventions.Add(TdServerIndexConvention);

            ConventionSet.AddBefore(
                conventionSet.ModelFinalizedConventions,
                valueGenerationStrategyConvention,
                typeof(ValidatingConvention));

            conventionSet.IndexAddedConventions.Add(TdServerInMemoryTablesConvention);
            conventionSet.IndexAddedConventions.Add(TdServerIndexConvention);

            conventionSet.IndexUniquenessChangedConventions.Add(TdServerIndexConvention);

            conventionSet.IndexAnnotationChangedConventions.Add(TdServerIndexConvention);

            conventionSet.PropertyNullabilityChangedConventions.Add(TdServerIndexConvention);

            StoreGenerationConvention storeGenerationConvention =
                new TdServerStoreGenerationConvention(Dependencies, RelationalDependencies);
            conventionSet.PropertyAnnotationChangedConventions.Add(TdServerIndexConvention);
            ReplaceConvention(conventionSet.PropertyAnnotationChangedConventions, storeGenerationConvention);
            ReplaceConvention(
                conventionSet.PropertyAnnotationChangedConventions, (RelationalValueGenerationConvention)valueGenerationConvention);

            ReplaceConvention(conventionSet.ModelFinalizedConventions, storeGenerationConvention);

            return conventionSet;
        }

        /// <summary>
        ///     <para>
        ///         Call this method to build a <see cref="ConventionSet" /> for SQL Server when using
        ///         the <see cref="ModelBuilder" /> outside of <see cref="DbContext.OnModelCreating" />.
        ///     </para>
        ///     <para>
        ///         Note that it is unusual to use this method.
        ///         Consider using <see cref="DbContext" /> in the normal way instead.
        ///     </para>
        /// </summary>
        /// <returns> The convention set. </returns>
        public static ConventionSet Build()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkTdServer()
                .AddDbContext<DbContext>(
                    (p, o) =>
                        o.UseTdServer("Server=.")
                            .UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DbContext>())
                {
                    return ConventionSet.CreateConventionSet(context);
                }
            }
        }
    }
}
