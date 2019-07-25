// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Tedd.EFCore.Teradata.Metadata;
using Tedd.EFCore.Teradata.TdServer.Internal;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     Extension methods for <see cref="IProperty" /> for SQL Server-specific metadata.
    /// </summary>
    public static class TdServerPropertyExtensions
    {
        /// <summary>
        ///     Returns the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The name to use for the hi-lo sequence. </returns>
        public static string GetTdServerHiLoSequenceName([NotNull] this IProperty property)
            => (string)property[TdServerAnnotationNames.HiLoSequenceName];

        /// <summary>
        ///     Sets the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="name"> The sequence name to use. </param>
        public static void SetTdServerHiLoSequenceName([NotNull] this IMutableProperty property, [CanBeNull] string name)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(name, nameof(name)));

        /// <summary>
        ///     Sets the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="name"> The sequence name to use. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerHiLoSequenceName(
            [NotNull] this IConventionProperty property, [CanBeNull] string name, bool fromDataAnnotation = false)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(name, nameof(name)),
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the hi-lo sequence name.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the hi-lo sequence name. </returns>
        public static ConfigurationSource? GetTdServerHiLoSequenceNameConfigurationSource([NotNull] this IConventionProperty property)
            => property.FindAnnotation(TdServerAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The schema to use for the hi-lo sequence. </returns>
        public static string GetTdServerHiLoSequenceSchema([NotNull] this IProperty property)
            => (string)property[TdServerAnnotationNames.HiLoSequenceSchema];

        /// <summary>
        ///     Sets the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="schema"> The schema to use. </param>
        public static void SetTdServerHiLoSequenceSchema([NotNull] this IMutableProperty property, [CanBeNull] string schema)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)));

        /// <summary>
        ///     Sets the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="schema"> The schema to use. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerHiLoSequenceSchema(
            [NotNull] this IConventionProperty property, [CanBeNull] string schema, bool fromDataAnnotation = false)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)),
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the hi-lo sequence schema.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the hi-lo sequence schema. </returns>
        public static ConfigurationSource? GetTdServerHiLoSequenceSchemaConfigurationSource([NotNull] this IConventionProperty property)
            => property.FindAnnotation(TdServerAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the hi-lo pattern.
        /// </summary>
        /// <returns> The sequence to use, or <c>null</c> if no sequence exists in the model. </returns>
        public static ISequence FindTdServerHiLoSequence([NotNull] this IProperty property)
        {
            var model = property.DeclaringEntityType.Model;

            if (property.GetTdServerValueGenerationStrategy() != TdServerValueGenerationStrategy.SequenceHiLo)
            {
                return null;
            }

            var sequenceName = property.GetTdServerHiLoSequenceName()
                               ?? model.GetTdServerHiLoSequenceName();

            var sequenceSchema = property.GetTdServerHiLoSequenceSchema()
                                 ?? model.GetTdServerHiLoSequenceSchema();

            return model.FindSequence(sequenceName, sequenceSchema);
        }

        /// <summary>
        ///     Returns the identity seed.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The identity seed. </returns>
        public static int? GetTdServerIdentitySeed([NotNull] this IProperty property)
            => (int?)property[TdServerAnnotationNames.IdentitySeed];

        /// <summary>
        ///     Sets the identity seed.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="seed"> The value to set. </param>
        public static void SetTdServerIdentitySeed([NotNull] this IMutableProperty property, int? seed)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentitySeed,
                seed);

        /// <summary>
        ///     Sets the identity seed.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="seed"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIdentitySeed(
            [NotNull] this IConventionProperty property, int? seed, bool fromDataAnnotation = false)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentitySeed,
                seed,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity seed.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the identity seed. </returns>
        public static ConfigurationSource? GetTdServerIdentitySeedConfigurationSource([NotNull] this IConventionProperty property)
            => property.FindAnnotation(TdServerAnnotationNames.IdentitySeed)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the identity increment.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The identity increment. </returns>
        public static int? GetTdServerIdentityIncrement([NotNull] this IProperty property)
            => (int?)property[TdServerAnnotationNames.IdentityIncrement];

        /// <summary>
        ///     Sets the identity increment.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="increment"> The value to set. </param>
        public static void SetTdServerIdentityIncrement([NotNull] this IMutableProperty property, int? increment)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentityIncrement,
                increment);

        /// <summary>
        ///     Sets the identity increment.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="increment"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIdentityIncrement(
            [NotNull] this IConventionProperty property, int? increment, bool fromDataAnnotation = false)
            => property.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentityIncrement,
                increment,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity increment.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the identity increment. </returns>
        public static ConfigurationSource? GetTdServerIdentityIncrementConfigurationSource([NotNull] this IConventionProperty property)
            => property.FindAnnotation(TdServerAnnotationNames.IdentityIncrement)?.GetConfigurationSource();

        /// <summary>
        ///     <para>
        ///         Returns the <see cref="TdServerValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        ///     </para>
        /// </summary>
        /// <returns> The strategy, or <see cref="TdServerValueGenerationStrategy.None"/> if none was set. </returns>
        public static TdServerValueGenerationStrategy GetTdServerValueGenerationStrategy([NotNull] this IProperty property)
        {
            var annotation = property[TdServerAnnotationNames.ValueGenerationStrategy];
            if (annotation != null)
            {
                return (TdServerValueGenerationStrategy)annotation;
            }

            var sharedTablePrincipalPrimaryKeyProperty = property.FindSharedTableRootPrimaryKeyProperty();
            if (sharedTablePrincipalPrimaryKeyProperty != null)
            {
                return sharedTablePrincipalPrimaryKeyProperty.GetTdServerValueGenerationStrategy()
                       == TdServerValueGenerationStrategy.IdentityColumn
                    ? TdServerValueGenerationStrategy.IdentityColumn
                    : TdServerValueGenerationStrategy.None;
            }

            if (property.ValueGenerated != ValueGenerated.OnAdd
                || property.GetDefaultValue() != null
                || property.GetDefaultValueSql() != null
                || property.GetComputedColumnSql() != null)
            {
                return TdServerValueGenerationStrategy.None;
            }

            var modelStrategy = property.DeclaringEntityType.Model.GetTdServerValueGenerationStrategy();

            if (modelStrategy == TdServerValueGenerationStrategy.SequenceHiLo
                && IsCompatibleWithValueGeneration(property))
            {
                return TdServerValueGenerationStrategy.SequenceHiLo;
            }

            return modelStrategy == TdServerValueGenerationStrategy.IdentityColumn
                   && IsCompatibleWithValueGeneration(property)
                ? TdServerValueGenerationStrategy.IdentityColumn
                : TdServerValueGenerationStrategy.None;
        }

        /// <summary>
        ///     Sets the <see cref="TdServerValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        public static void SetTdServerValueGenerationStrategy(
            [NotNull] this IMutableProperty property, TdServerValueGenerationStrategy? value)
        {
            CheckTdServerValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(TdServerAnnotationNames.ValueGenerationStrategy, value);
        }

        /// <summary>
        ///     Sets the <see cref="TdServerValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerValueGenerationStrategy(
            [NotNull] this IConventionProperty property, TdServerValueGenerationStrategy? value, bool fromDataAnnotation = false)
        {
            CheckTdServerValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(TdServerAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);
        }

        private static void CheckTdServerValueGenerationStrategy(IProperty property, TdServerValueGenerationStrategy? value)
        {
            if (value != null)
            {
                var propertyType = property.ClrType;

                if (value == TdServerValueGenerationStrategy.IdentityColumn
                    && !IsCompatibleWithValueGeneration(property))
                {
                    throw new ArgumentException(
                        TdServerStrings.IdentityBadType(
                            property.Name, property.DeclaringEntityType.DisplayName(), propertyType.ShortDisplayName()));
                }

                if (value == TdServerValueGenerationStrategy.SequenceHiLo
                    && !IsCompatibleWithValueGeneration(property))
                {
                    throw new ArgumentException(
                        TdServerStrings.SequenceBadType(
                            property.Name, property.DeclaringEntityType.DisplayName(), propertyType.ShortDisplayName()));
                }
            }
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the <see cref="TdServerValueGenerationStrategy" />.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the <see cref="TdServerValueGenerationStrategy" />. </returns>
        public static ConfigurationSource? GetTdServerValueGenerationStrategyConfigurationSource(
            [NotNull] this IConventionProperty property)
            => property.FindAnnotation(TdServerAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with any <see cref="TdServerValueGenerationStrategy"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <c>true</c> if compatible. </returns>
        public static bool IsCompatibleWithValueGeneration([NotNull] IProperty property)
        {
            var type = property.ClrType;

            return (type.IsInteger()
                    || type == typeof(decimal))
                   && (property.FindMapping()?.Converter
                       ?? property.GetValueConverter()) == null;
        }
    }
}
