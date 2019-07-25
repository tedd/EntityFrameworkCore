// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Relational.Query.Pipeline;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tedd.EFCore.Teradata.TdServer.Query.Pipeline
{
    public class TdServerQuerySqlGeneratorFactory : QuerySqlGeneratorFactory
    {
        private readonly IRelationalCommandBuilderFactory _commandBuilderFactory;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public TdServerQuerySqlGeneratorFactory(
            IRelationalCommandBuilderFactory commandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper)
            : base(commandBuilderFactory, sqlGenerationHelper)
        {
            _commandBuilderFactory = commandBuilderFactory;
            _sqlGenerationHelper = sqlGenerationHelper;
        }

        public override QuerySqlGenerator Create()
            => new TdServerQuerySqlGenerator(_commandBuilderFactory, _sqlGenerationHelper);
    }
}
