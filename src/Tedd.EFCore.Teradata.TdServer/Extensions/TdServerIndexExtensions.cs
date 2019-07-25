// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     Extension methods for <see cref="IIndex" /> for SQL Server-specific metadata.
    /// </summary>
    public static class TdServerIndexExtensions
    {
        /// <summary>
        ///     Returns a value indicating whether the index is clustered.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <c>true</c> if the index is clustered. </returns>
        public static bool? GetTdServerIsClustered([NotNull] this IIndex index)
            => (bool?)index[TdServerAnnotationNames.Clustered];

        /// <summary>
        ///     Sets a value indicating whether the index is clustered.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        public static void SetTdServerIsClustered([NotNull] this IMutableIndex index, bool? value)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.Clustered,
                value);

        /// <summary>
        ///     Sets a value indicating whether the index is clustered.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIsClustered(
            [NotNull] this IConventionIndex index, bool? value, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.Clustered,
                value,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is clustered.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is clustered. </returns>
        public static ConfigurationSource? GetTdServerIsClusteredConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(TdServerAnnotationNames.Clustered)?.GetConfigurationSource();

        /// <summary>
        ///     Returns included property names, or <c>null</c> if they have not been specified.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The included property names, or <c>null</c> if they have not been specified. </returns>
        public static IReadOnlyList<string> GetTdServerIncludeProperties([NotNull] this IIndex index)
            => (string[])index[TdServerAnnotationNames.Include];

        /// <summary>
        ///     Sets included property names.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="properties"> The value to set. </param>
        public static void SetTdServerIncludeProperties([NotNull] this IMutableIndex index, [NotNull] IReadOnlyList<string> properties)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.Include,
                properties);

        /// <summary>
        ///     Sets included property names.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <param name="properties"> The value to set. </param>
        public static void SetTdServerIncludeProperties(
            [NotNull] this IConventionIndex index, [NotNull] IReadOnlyList<string> properties, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.Include,
                properties,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the included property names.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the included property names. </returns>
        public static ConfigurationSource? GetTdServerIncludePropertiesConfigurationSource([NotNull] this IConventionIndex index)
            => index.FindAnnotation(TdServerAnnotationNames.Include)?.GetConfigurationSource();

        /// <summary>
        ///     Returns a value indicating whether the index is online.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <c>true</c> if the index is online. </returns>
        public static bool? GetTdServerIsCreatedOnline([NotNull] this IIndex index)
            => (bool?)index[TdServerAnnotationNames.CreatedOnline];

        /// <summary>
        ///     Sets a value indicating whether the index is online.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="createdOnline"> The value to set. </param>
        public static void SetTdServerIsCreatedOnline([NotNull] this IMutableIndex index, bool? createdOnline)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.CreatedOnline,
                createdOnline);

        /// <summary>
        ///     Sets a value indicating whether the index is online.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="createdOnline"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIsCreatedOnline(
            [NotNull] this IConventionIndex index, bool? createdOnline, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(
                TdServerAnnotationNames.CreatedOnline,
                createdOnline,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is online.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is online. </returns>
        public static ConfigurationSource? GetTdServerIsCreatedOnlineConfigurationSource([NotNull] this IConventionIndex index)
            => index.FindAnnotation(TdServerAnnotationNames.CreatedOnline)?.GetConfigurationSource();
    }
}
