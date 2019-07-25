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
    ///     SQL Server specific extension methods for <see cref="KeyBuilder" />.
    /// </summary>
    public static class TdServerKeyBuilderExtensions
    {
        /// <summary>
        ///     Configures whether the key is clustered when targeting SQL Server.
        /// </summary>
        /// <param name="keyBuilder"> The builder for the key being configured. </param>
        /// <param name="clustered"> A value indicating whether the key is clustered. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static KeyBuilder ForTdServerIsClustered([NotNull] this KeyBuilder keyBuilder, bool clustered = true)
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder));

            keyBuilder.Metadata.SetTdServerIsClustered(clustered);

            return keyBuilder;
        }

        /// <summary>
        ///     Configures whether the key is clustered when targeting SQL Server.
        /// </summary>
        /// <param name="keyBuilder"> The builder for the key being configured. </param>
        /// <param name="clustered"> A value indicating whether the key is clustered. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionKeyBuilder ForTdServerIsClustered(
            [NotNull] this IConventionKeyBuilder keyBuilder,
            bool? clustered,
            bool fromDataAnnotation = false)
        {
            if (keyBuilder.ForTdServerCanSetIsClustered(clustered, fromDataAnnotation))
            {
                keyBuilder.Metadata.SetTdServerIsClustered(clustered, fromDataAnnotation);
                return keyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the key can be configured as clustered.
        /// </summary>
        /// <param name="keyBuilder"> The builder for the key being configured. </param>
        /// <param name="clustered"> A value indicating whether the key is clustered. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the key can be configured as clustered. </returns>
        public static bool ForTdServerCanSetIsClustered(
            [NotNull] this IConventionKeyBuilder keyBuilder,
            bool? clustered,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder));

            return keyBuilder.CanSetAnnotation(TdServerAnnotationNames.Clustered, clustered, fromDataAnnotation);
        }
    }
}
