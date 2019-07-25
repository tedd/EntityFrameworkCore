// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline.SqlExpressions;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerNewGuidTranslator : IMethodCallTranslator
    {
        private static MethodInfo _methodInfo = typeof(Guid).GetRuntimeMethod(nameof(Guid.NewGuid), Array.Empty<Type>());
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public TdServerNewGuidTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IList<SqlExpression> arguments)
        {
            return _methodInfo.Equals(method)
                ? _sqlExpressionFactory.Function(
                    "NEWID",
                    Array.Empty<SqlExpression>(),
                    method.ReturnType)
                : null;
        }
    }
}
