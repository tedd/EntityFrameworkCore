// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline.SqlExpressions;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerDateDiffFunctionsTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDateDiffMapping
            = new Dictionary<MethodInfo, string>
            {
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "YEAR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "YEAR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "YEAR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "YEAR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MONTH"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MONTH"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MONTH"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MONTH"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "DAY"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "DAY"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "DAY"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "DAY"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "HOUR"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "MINUTE"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "SECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "MILLISECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "MICROSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "NANOSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "NANOSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "NANOSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "NANOSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan), typeof(TimeSpan) }),
                    "NANOSECOND"
                },
                {
                    typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(TdServerDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(TimeSpan?), typeof(TimeSpan?) }),
                    "NANOSECOND"
                }
            };
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public TdServerDateDiffFunctionsTranslator(
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IList<SqlExpression> arguments)
        {
            if (_methodInfoDateDiffMapping.TryGetValue(method, out var datePart))
            {
                var startDate = arguments[1];
                var endDate = arguments[2];
                var typeMapping = ExpressionExtensions.InferTypeMapping(startDate, endDate);

                startDate = _sqlExpressionFactory.ApplyTypeMapping(startDate, typeMapping);
                endDate = _sqlExpressionFactory.ApplyTypeMapping(endDate, typeMapping);

                return _sqlExpressionFactory.Function(
                    "DATEDIFF",
                    new[]
                    {
                        _sqlExpressionFactory.Fragment(datePart),
                        startDate,
                        endDate
                    },
                    typeof(int));
            }

            return null;
        }
    }
}
