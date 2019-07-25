// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Tedd.EFCore.Teradata.TdServer.Update.Internal;

namespace Tedd.EFCore.Teradata.TdServer.ValueGeneration.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class TdServerSequenceValueGeneratorFactory : ITdServerSequenceValueGeneratorFactory
    {
        private readonly ITdServerUpdateSqlGenerator _sqlGenerator;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TdServerSequenceValueGeneratorFactory(
            [NotNull] ITdServerUpdateSqlGenerator sqlGenerator)
        {
            _sqlGenerator = sqlGenerator;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ValueGenerator Create(
            IProperty property,
            TdServerSequenceValueGeneratorState generatorState,
            ITdServerConnection connection,
            IRawSqlCommandBuilder rawSqlCommandBuilder,
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> commandLogger)
        {
            var type = property.ClrType.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(long))
            {
                return new TdServerSequenceHiLoValueGenerator<long>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(int))
            {
                return new TdServerSequenceHiLoValueGenerator<int>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(decimal))
            {
                return new TdServerSequenceHiLoValueGenerator<decimal>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(short))
            {
                return new TdServerSequenceHiLoValueGenerator<short>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(byte))
            {
                return new TdServerSequenceHiLoValueGenerator<byte>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(char))
            {
                return new TdServerSequenceHiLoValueGenerator<char>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(ulong))
            {
                return new TdServerSequenceHiLoValueGenerator<ulong>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(uint))
            {
                return new TdServerSequenceHiLoValueGenerator<uint>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(ushort))
            {
                return new TdServerSequenceHiLoValueGenerator<ushort>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(sbyte))
            {
                return new TdServerSequenceHiLoValueGenerator<sbyte>(rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            throw new ArgumentException(
                CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(TdServerSequenceValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
