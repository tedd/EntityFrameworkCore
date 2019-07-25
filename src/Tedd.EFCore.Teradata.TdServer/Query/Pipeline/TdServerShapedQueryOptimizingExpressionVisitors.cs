﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Pipeline;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerShapedQueryOptimizer : RelationalShapedQueryOptimizer
    {
        public TdServerShapedQueryOptimizer(
            QueryCompilationContext queryCompilationContext,
            ISqlExpressionFactory sqlExpressionFactory)
            : base(queryCompilationContext, sqlExpressionFactory)
        {
        }

        public override Expression Visit(Expression query)
        {
            query = base.Visit(query);
            query = new SearchConditionConvertingExpressionVisitor(SqlExpressionFactory).Visit(query);
            query = new SqlExpressionOptimizingVisitor(SqlExpressionFactory, UseRelationalNulls).Visit(query);

            return query;
        }
    }
}
