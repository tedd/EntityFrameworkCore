// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Tedd.EFCore.Teradata.Metadata;
using Tedd.EFCore.Teradata.TdServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Tedd.EFCore.Teradata
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="PropertyBuilder" />.
    /// </summary>
    public static class TdServerPropertyBuilderExtensions
    {
        /// <summary>
        ///     Configures the key property to use a sequence-based hi-lo pattern to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="name"> The name of the sequence. </param>
        /// <param name="schema"> The schema of the sequence. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder ForTdServerUseSequenceHiLo(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string name = null,
            [CanBeNull] string schema = null)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var property = propertyBuilder.Metadata;

            name ??= TdServerModelExtensions.DefaultHiLoSequenceName;

            var model = property.DeclaringEntityType.Model;

            if (model.FindSequence(name, schema) == null)
            {
                model.AddSequence(name, schema).IncrementBy = 10;
            }

            property.SetTdServerValueGenerationStrategy(TdServerValueGenerationStrategy.SequenceHiLo);
            property.SetTdServerHiLoSequenceName(name);
            property.SetTdServerHiLoSequenceSchema(schema);
            property.SetTdServerIdentitySeed(null);
            property.SetTdServerIdentityIncrement(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use a sequence-based hi-lo pattern to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="name"> The name of the sequence. </param>
        /// <param name="schema"> The schema of the sequence. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> ForTdServerUseSequenceHiLo<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string name = null,
            [CanBeNull] string schema = null)
            => (PropertyBuilder<TProperty>)ForTdServerUseSequenceHiLo((PropertyBuilder)propertyBuilder, name, schema);

        /// <summary>
        ///     Configures the database sequence used for the hi-lo pattern to generate values for the key property,
        ///     when targeting SQL Server.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="name"> The name of the sequence. </param>
        /// <param name="schema">The schema of the sequence. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> A builder to further configure the sequence. </returns>
        public static IConventionSequenceBuilder ForTdServerHasHiLoSequence(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema,
            bool fromDataAnnotation = false)
        {
            if (!propertyBuilder.ForTdServerCanSetHiLoSequence(name, schema))
            {
                return null;
            }

            propertyBuilder.Metadata.SetTdServerHiLoSequenceName(name, fromDataAnnotation);
            propertyBuilder.Metadata.SetTdServerHiLoSequenceSchema(schema, fromDataAnnotation);

            return name == null
                ? null
                : propertyBuilder.Metadata.DeclaringEntityType.Model.Builder.HasSequence(name, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns a value indicating whether the given name and schema can be set for the hi-lo sequence.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="name"> The name of the sequence. </param>
        /// <param name="schema">The schema of the sequence. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the given name and schema can be set for the hi-lo sequence. </returns>
        public static bool ForTdServerCanSetHiLoSequence(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            return propertyBuilder.CanSetAnnotation(TdServerAnnotationNames.HiLoSequenceName, name, fromDataAnnotation)
                   && propertyBuilder.CanSetAnnotation(TdServerAnnotationNames.HiLoSequenceSchema, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder ForTdServerUseIdentityColumn(
            [NotNull] this PropertyBuilder propertyBuilder,
            int seed = 1,
            int increment = 1)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetTdServerValueGenerationStrategy(TdServerValueGenerationStrategy.IdentityColumn);
            property.SetTdServerIdentitySeed(seed);
            property.SetTdServerIdentityIncrement(increment);
            property.SetTdServerHiLoSequenceName(null);
            property.SetTdServerHiLoSequenceSchema(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> ForTdServerUseIdentityColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            int seed = 1,
            int increment = 1)
            => (PropertyBuilder<TProperty>)ForTdServerUseIdentityColumn((PropertyBuilder)propertyBuilder, seed, increment);

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        [Obsolete("Use ForTdServerUseIdentityColumn instead")]
        public static PropertyBuilder UseTdServerIdentityColumn(
            [NotNull] this PropertyBuilder propertyBuilder,
            int seed = 1,
            int increment = 1)
            => propertyBuilder.ForTdServerUseIdentityColumn(seed, increment);

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        [Obsolete("Use ForTdServerUseIdentityColumn instead")]
        public static PropertyBuilder<TProperty> UseTdServerIdentityColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            int seed = 1,
            int increment = 1)
            => propertyBuilder.ForTdServerUseIdentityColumn(seed, increment);

        /// <summary>
        ///     Configures the seed for SQL Server IDENTITY.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder ForTdServerHasIdentitySeed(
            [NotNull] this IConventionPropertyBuilder propertyBuilder, int? seed, bool fromDataAnnotation = false)
        {
            if (propertyBuilder.ForTdServerCanSetIdentitySeed(seed, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetTdServerIdentitySeed(seed, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the seed for SQL Server IDENTITY.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="seed"> The value that is used for the very first row loaded into the table. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the given value can be set as the seed for SQL Server IDENTITY. </returns>
        public static bool ForTdServerCanSetIdentitySeed(
            [NotNull] this IConventionPropertyBuilder propertyBuilder, int? seed, bool fromDataAnnotation = false)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return propertyBuilder.CanSetAnnotation(TdServerAnnotationNames.IdentitySeed, seed, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the increment for SQL Server IDENTITY.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder ForTdServerHasIdentityIncrement(
            [NotNull] this IConventionPropertyBuilder propertyBuilder, int? increment, bool fromDataAnnotation = false)
        {
            if (propertyBuilder.ForTdServerCanSetIdentityIncrement(increment, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetTdServerIdentityIncrement(increment, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the increment for SQL Server IDENTITY.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="increment"> The incremental value that is added to the identity value of the previous row that was loaded. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the given value can be set as the default increment for SQL Server IDENTITY. </returns>
        public static bool ForTdServerCanSetIdentityIncrement(
            [NotNull] this IConventionPropertyBuilder propertyBuilder, int? increment, bool fromDataAnnotation = false)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return propertyBuilder.CanSetAnnotation(TdServerAnnotationNames.IdentityIncrement, increment, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the value generation strategy for the key property, when targeting SQL Server.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="valueGenerationStrategy"> The value generation strategy. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder ForTdServerHasValueGenerationStrategy(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            TdServerValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetAnnotation(
                TdServerAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetTdServerValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
                if (valueGenerationStrategy != TdServerValueGenerationStrategy.IdentityColumn)
                {
                    propertyBuilder.ForTdServerHasIdentitySeed(null, fromDataAnnotation);
                    propertyBuilder.ForTdServerHasIdentityIncrement(null, fromDataAnnotation);
                }

                if (valueGenerationStrategy != TdServerValueGenerationStrategy.SequenceHiLo)
                {
                    propertyBuilder.ForTdServerHasHiLoSequence(null, null, fromDataAnnotation);
                }

                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the value generation strategy.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="valueGenerationStrategy"> The value generation strategy. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the given value can be set as the default value generation strategy. </returns>
        public static bool ForTdServerCanSetValueGenerationStrategy(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            TdServerValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return (valueGenerationStrategy == null
                    || TdServerPropertyExtensions.IsCompatibleWithValueGeneration(propertyBuilder.Metadata))
                   && propertyBuilder.CanSetAnnotation(
                       TdServerAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation);
        }
    }
}
