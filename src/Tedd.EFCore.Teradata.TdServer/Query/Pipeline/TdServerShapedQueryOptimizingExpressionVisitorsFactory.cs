// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerShapedQueryOptimizerFactory : RelationalShapedQueryOptimizerFactory
    {
        public TdServerShapedQueryOptimizerFactory(ISqlExpressionFactory sqlExpressionFactory)
            : base(sqlExpressionFactory)
        {
        }

        public override ShapedQueryOptimizer Create(QueryCompilationContext queryCompilationContext)
        {
            return new TdServerShapedQueryOptimizer(queryCompilationContext, SqlExpressionFactory);
        }
    }
}
