// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tedd.EFCore.Teradata.TdServer.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class TdServerStringTypeMapping : StringTypeMapping
    {
        private const int UnicodeMax = 4000;
        private const int AnsiMax = 8000;

        private readonly int _maxSpecificSize;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TdServerStringTypeMapping(
            [CanBeNull] string storeType = null,
            bool unicode = false,
            int? size = null,
            bool fixedLength = false,
            StoreTypePostfix? storeTypePostfix = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType ?? GetStoreName(unicode, fixedLength),
                    storeTypePostfix ?? StoreTypePostfix.Size,
                    GetDbType(unicode, fixedLength),
                    unicode,
                    size,
                    fixedLength))
        {
        }

        private static string GetStoreName(bool unicode, bool fixedLength) => unicode
            ? fixedLength ? "char" : "varchar"
            : fixedLength
                ? "char"
                : "varchar";

        private static DbType? GetDbType(bool unicode, bool fixedLength) => unicode
            ? (fixedLength ? System.Data.DbType.String : (DbType?)null)
            : System.Data.DbType.AnsiString;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected TdServerStringTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
        }

        private static int CalculateSize(bool unicode, int? size)
            => unicode
                ? size.HasValue && size <= UnicodeMax
                    ? size.Value
                    : UnicodeMax
                : size.HasValue && size <= AnsiMax
                    ? size.Value
                    : AnsiMax;

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new TdServerStringTypeMapping(parameters);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // 8000 bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // -1 (unbounded) to avoid SQL client size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length;

            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override string GenerateNonNullSqlLiteral(object value)
            => IsUnicode
                ? $"N'{EscapeSqlLiteral((string)value)}'" // Interpolation okay; strings
                : $"'{EscapeSqlLiteral((string)value)}'";
    }
}
