// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Tedd.EFCore.Teradata.Metadata;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     Extension methods for <see cref="IModel" /> for SQL Server-specific metadata.
    /// </summary>
    public static class TdServerModelExtensions
    {
        /// <summary>
        ///     The default name for the hi-lo sequence.
        /// </summary>
        public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";

        /// <summary>
        ///     Returns the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The name to use for the default hi-lo sequence. </returns>
        public static string GetTdServerHiLoSequenceName([NotNull] this IModel model)
            => (string)model[TdServerAnnotationNames.HiLoSequenceName]
               ?? DefaultHiLoSequenceName;

        /// <summary>
        ///     Sets the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="name"> The value to set. </param>
        public static void SetTdServerHiLoSequenceName([NotNull] this IMutableModel model, [CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, nameof(name));

            model.SetOrRemoveAnnotation(TdServerAnnotationNames.HiLoSequenceName, name);
        }

        /// <summary>
        ///     Sets the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="name"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerHiLoSequenceName(
            [NotNull] this IConventionModel model, [CanBeNull] string name, bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(name, nameof(name));

            model.SetOrRemoveAnnotation(TdServerAnnotationNames.HiLoSequenceName, name, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default hi-lo sequence name.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default hi-lo sequence name. </returns>
        public static ConfigurationSource? GetTdServerHiLoSequenceNameConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(TdServerAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the schema to use for the default hi-lo sequence.
        ///     <see cref="TdServerPropertyBuilderExtensions.ForTdServerUseSequenceHiLo" />
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The schema to use for the default hi-lo sequence. </returns>
        public static string GetTdServerHiLoSequenceSchema([NotNull] this IModel model)
            => (string)model[TdServerAnnotationNames.HiLoSequenceSchema];

        /// <summary>
        ///     Sets the schema to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetTdServerHiLoSequenceSchema([NotNull] this IMutableModel model, [CanBeNull] string value)
        {
            Check.NullButNotEmpty(value, nameof(value));

            model.SetOrRemoveAnnotation(TdServerAnnotationNames.HiLoSequenceSchema, value);
        }

        /// <summary>
        ///     Sets the schema to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerHiLoSequenceSchema(
            [NotNull] this IConventionModel model, [CanBeNull] string value, bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(value, nameof(value));

            model.SetOrRemoveAnnotation(TdServerAnnotationNames.HiLoSequenceSchema, value, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default hi-lo sequence schema.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default hi-lo sequence schema. </returns>
        public static ConfigurationSource? GetTdServerHiLoSequenceSchemaConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(TdServerAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the default identity seed.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default identity seed. </returns>
        public static int GetTdServerIdentitySeed([NotNull] this IModel model)
            => (int?)model[TdServerAnnotationNames.IdentitySeed] ?? 1;

        /// <summary>
        ///     Sets the default identity seed.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="seed"> The value to set. </param>
        public static void SetTdServerIdentitySeed([NotNull] this IMutableModel model, int? seed)
            => model.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentitySeed,
                seed);

        /// <summary>
        ///     Sets the default identity seed.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="seed"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIdentitySeed([NotNull] this IConventionModel model, int? seed, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentitySeed,
                seed,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default schema.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default schema. </returns>
        public static ConfigurationSource? GetTdServerIdentitySeedConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(TdServerAnnotationNames.IdentitySeed)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the default identity increment.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default identity increment. </returns>
        public static int GetTdServerIdentityIncrement([NotNull] this IModel model)
            => (int?)model[TdServerAnnotationNames.IdentityIncrement] ?? 1;

        /// <summary>
        ///     Sets the default identity increment.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="increment"> The value to set. </param>
        public static void SetTdServerIdentityIncrement([NotNull] this IMutableModel model, int? increment)
            => model.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentityIncrement,
                increment);

        /// <summary>
        ///     Sets the default identity increment.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="increment"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerIdentityIncrement(
            [NotNull] this IConventionModel model, int? increment, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(
                TdServerAnnotationNames.IdentityIncrement,
                increment,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default identity increment.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default identity increment. </returns>
        public static ConfigurationSource? GetTdServerIdentityIncrementConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(TdServerAnnotationNames.IdentityIncrement)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="TdServerValueGenerationStrategy" /> to use for properties
        ///     of keys in the model, unless the property has a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default <see cref="TdServerValueGenerationStrategy" />. </returns>
        public static TdServerValueGenerationStrategy? GetTdServerValueGenerationStrategy([NotNull] this IModel model)
            => (TdServerValueGenerationStrategy?)model[TdServerAnnotationNames.ValueGenerationStrategy];

        /// <summary>
        ///     Attempts to set the <see cref="TdServerValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetTdServerValueGenerationStrategy([NotNull] this IMutableModel model, TdServerValueGenerationStrategy? value)
            => model.SetOrRemoveAnnotation(TdServerAnnotationNames.ValueGenerationStrategy, value);

        /// <summary>
        ///     Attempts to set the <see cref="TdServerValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetTdServerValueGenerationStrategy(
            [NotNull] this IConventionModel model, TdServerValueGenerationStrategy? value, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(TdServerAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default <see cref="TdServerValueGenerationStrategy" />.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default <see cref="TdServerValueGenerationStrategy" />. </returns>
        public static ConfigurationSource? GetTdServerValueGenerationStrategyConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(TdServerAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();
    }
}
