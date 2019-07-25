// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerQuerySqlGenerator : QuerySqlGenerator
    {
        public TdServerQuerySqlGenerator(
            IRelationalCommandBuilderFactory relationalCommandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper)
            : base(relationalCommandBuilderFactory, sqlGenerationHelper)
        {
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
            if (selectExpression.Limit != null
                && selectExpression.Offset == null)
            {
                Sql.Append("TOP(");

                Visit(selectExpression.Limit);

                Sql.Append(") ");
            }
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            // Note: For Limit without Offset, TdServer generates TOP()
            if (selectExpression.Offset != null)
            {
                Sql.AppendLine()
                    .Append("OFFSET ");

                Visit(selectExpression.Offset);

                Sql.Append(" ROWS");

                if (selectExpression.Limit != null)
                {
                    Sql.Append(" FETCH NEXT ");

                    Visit(selectExpression.Limit);

                    Sql.Append(" ROWS ONLY");
                }
            }
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (!sqlFunctionExpression.IsBuiltIn
                && string.IsNullOrEmpty(sqlFunctionExpression.Schema))
            {
                sqlFunctionExpression = new SqlFunctionExpression(
                    schema: "dbo",
                    sqlFunctionExpression.FunctionName,
                    sqlFunctionExpression.Arguments,
                    sqlFunctionExpression.Type,
                    sqlFunctionExpression.TypeMapping);
            }


            return base.VisitSqlFunction(sqlFunctionExpression);
        }
    }
}
