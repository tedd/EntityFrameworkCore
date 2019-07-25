// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline.SqlExpressions;
using Tedd.EFCore.Teradata.TdServer.Internal;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerFullTextSearchFunctionsTranslator : IMethodCallTranslator
    {
        private const string FreeTextFunctionName = "FREETEXT";
        private const string ContainsFunctionName = "CONTAINS";

        private static readonly MethodInfo _freeTextMethodInfo
            = typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                nameof(TdServerDbFunctionsExtensions.FreeText),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly MethodInfo _freeTextMethodInfoWithLanguage
            = typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                nameof(TdServerDbFunctionsExtensions.FreeText),
                new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int) });

        private static readonly MethodInfo _containsMethodInfo
            = typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                nameof(TdServerDbFunctionsExtensions.Contains),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly MethodInfo _containsMethodInfoWithLanguage
            = typeof(TdServerDbFunctionsExtensions).GetRuntimeMethod(
                nameof(TdServerDbFunctionsExtensions.Contains),
                new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int) });

        private static IDictionary<MethodInfo, string> _functionMapping
            = new Dictionary<MethodInfo, string>
            {
                {_freeTextMethodInfo, FreeTextFunctionName },
                {_freeTextMethodInfoWithLanguage, FreeTextFunctionName },
                {_containsMethodInfo, ContainsFunctionName },
                {_containsMethodInfoWithLanguage, ContainsFunctionName },
            };
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public TdServerFullTextSearchFunctionsTranslator(
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IList<SqlExpression> arguments)
        {
            if (_functionMapping.TryGetValue(method, out var functionName))
            {
                var propertyReference = arguments[1];
                if (!(propertyReference is ColumnExpression))
                {
                    throw new InvalidOperationException(TdServerStrings.InvalidColumnNameForFreeText);
                }

                var typeMapping = propertyReference.TypeMapping;
                var freeText = _sqlExpressionFactory.ApplyTypeMapping(arguments[2], typeMapping);

                var functionArguments = new List<SqlExpression>
                {
                    propertyReference,
                    freeText
                };

                if (arguments.Count == 4)
                {
                    functionArguments.Add(
                        _sqlExpressionFactory.Fragment($"LANGUAGE {((SqlConstantExpression)arguments[3]).Value}"));
                }

                return _sqlExpressionFactory.Function(
                    functionName,
                    functionArguments,
                    typeof(bool));
            }

            return null;
        }
    }
}
