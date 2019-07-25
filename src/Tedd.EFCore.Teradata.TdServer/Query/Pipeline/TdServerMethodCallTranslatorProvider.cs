// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public TdServerMethodCallTranslatorProvider(
            ISqlExpressionFactory sqlExpressionFactory,
            IEnumerable<IMethodCallTranslatorPlugin> plugins)
            : base(sqlExpressionFactory, plugins)
        {
            AddTranslators(new IMethodCallTranslator[]
            {
                new TdServerMathTranslator(sqlExpressionFactory),
                new TdServerNewGuidTranslator(sqlExpressionFactory),
                new TdServerStringMethodTranslator(sqlExpressionFactory),
                new TdServerDateTimeMethodTranslator(sqlExpressionFactory),
                new TdServerDateDiffFunctionsTranslator(sqlExpressionFactory),
                new TdServerConvertTranslator(sqlExpressionFactory),
                new TdServerObjectToStringTranslator(sqlExpressionFactory),
                new TdServerFullTextSearchFunctionsTranslator(sqlExpressionFactory),
            });
        }
    }
}
