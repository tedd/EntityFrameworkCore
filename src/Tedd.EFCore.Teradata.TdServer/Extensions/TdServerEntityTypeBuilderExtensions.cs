// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class TdServerEntityTypeBuilderExtensions
    {
        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder ForTdServerIsMemoryOptimized(
            [NotNull] this EntityTypeBuilder entityTypeBuilder, bool memoryOptimized = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetTdServerIsMemoryOptimized(memoryOptimized);

            return entityTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> ForTdServerIsMemoryOptimized<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder, bool memoryOptimized = true)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForTdServerIsMemoryOptimized((EntityTypeBuilder)entityTypeBuilder, memoryOptimized);

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="collectionOwnershipBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static OwnedNavigationBuilder ForTdServerIsMemoryOptimized(
            [NotNull] this OwnedNavigationBuilder collectionOwnershipBuilder, bool memoryOptimized = true)
        {
            Check.NotNull(collectionOwnershipBuilder, nameof(collectionOwnershipBuilder));

            collectionOwnershipBuilder.OwnedEntityType.SetTdServerIsMemoryOptimized(memoryOptimized);

            return collectionOwnershipBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <typeparam name="TRelatedEntity"> The entity type that this relationship targets. </typeparam>
        /// <param name="collectionOwnershipBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static OwnedNavigationBuilder<TEntity, TRelatedEntity> ForTdServerIsMemoryOptimized<TEntity, TRelatedEntity>(
            [NotNull] this OwnedNavigationBuilder<TEntity, TRelatedEntity> collectionOwnershipBuilder, bool memoryOptimized = true)
            where TEntity : class
            where TRelatedEntity : class
            => (OwnedNavigationBuilder<TEntity, TRelatedEntity>)ForTdServerIsMemoryOptimized(
                (OwnedNavigationBuilder)collectionOwnershipBuilder, memoryOptimized);

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionEntityTypeBuilder ForTdServerIsMemoryOptimized(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? memoryOptimized,
            bool fromDataAnnotation = false)
        {
            if (entityTypeBuilder.ForTdServerCanSetIsMemoryOptimized(memoryOptimized, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetTdServerIsMemoryOptimized(memoryOptimized, fromDataAnnotation);
                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the mapped table can be configured as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the mapped table can be configured as memory-optimized. </returns>
        public static bool ForTdServerCanSetIsMemoryOptimized(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? memoryOptimized,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(TdServerAnnotationNames.MemoryOptimized, memoryOptimized, fromDataAnnotation);
        }
    }
}
