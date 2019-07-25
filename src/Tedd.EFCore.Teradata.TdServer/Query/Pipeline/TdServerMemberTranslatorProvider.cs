// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public TdServerMemberTranslatorProvider(
            ISqlExpressionFactory sqlExpressionFactory,
            IEnumerable<IMemberTranslatorPlugin> plugins)
            : base(sqlExpressionFactory, plugins)
        {
            AddTranslators(
                new IMemberTranslator[] {
                    new TdServerDateTimeMemberTranslator(sqlExpressionFactory),
                    new TdServerStringMemberTranslator(sqlExpressionFactory)
                });
        }
    }
}
