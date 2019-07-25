// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     Extension methods for <see cref="IEntityType" /> for SQL Server-specific metadata.
    /// </summary>
    public static class TdServerEntityTypeExtensions
    {
        /// <summary>
        ///     Returns a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> <c>true</c> if the entity type is mapped to a memory-optimized table. </returns>
        public static bool GetTdServerIsMemoryOptimized([NotNull] this IEntityType entityType)
            => entityType[TdServerAnnotationNames.MemoryOptimized] as bool? ?? false;

        /// <summary>
        ///     Sets a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="memoryOptimized"> The value to set. </param>
        public static void SetTdServerIsMemoryOptimized([NotNull] this IMutableEntityType entityType, bool memoryOptimized)
            => entityType.SetOrRemoveAnnotation(TdServerAnnotationNames.MemoryOptimized, memoryOptimized);

        /// <summary>
        ///     Sets a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="memoryOptimized"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIsMemoryOptimized(
            [NotNull] this IConventionEntityType entityType, bool? memoryOptimized, bool fromDataAnnotation = false)
            => entityType.SetOrRemoveAnnotation(TdServerAnnotationNames.MemoryOptimized, memoryOptimized, fromDataAnnotation);

        /// <summary>
        ///     Gets the configuration source for the memory-optimized setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source for the memory-optimized setting. </returns>
        public static ConfigurationSource? GetTdServerIsMemoryOptimizedConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(TdServerAnnotationNames.MemoryOptimized)?.GetConfigurationSource();
    }
}
