// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Tedd.EFCore.Teradata.TdServer.Update.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Tedd.EFCore.Teradata.Update
{
    public class TdServerModificationCommandBatchTest
    {
        [ConditionalFact]
        public void AddCommand_returns_false_when_max_batch_size_is_reached()
        {
            var typeMapper = new TdServerTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var logger = new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>();

            var batch = new TdServerModificationCommandBatch(
                new ModificationCommandBatchFactoryDependencies(
                    new RelationalCommandBuilderFactory(
                        new RelationalCommandBuilderDependencies(
                            typeMapper)),
                    new TdServerSqlGenerationHelper(
                        new RelationalSqlGenerationHelperDependencies()),
                    new TdServerUpdateSqlGenerator(
                        new UpdateSqlGeneratorDependencies(
                            new TdServerSqlGenerationHelper(
                                new RelationalSqlGenerationHelperDependencies()),
                            typeMapper)),
                    new TypedRelationalValueBufferFactoryFactory(
                        new RelationalValueBufferFactoryDependencies(
                            typeMapper, new CoreSingletonOptions())),
                    new CurrentDbContext(new FakeDbContext()),
                    logger),
                1);

            Assert.True(
                batch.AddCommand(
                    new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.False(
                batch.AddCommand(
                    new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }

        private class FakeDbContext : DbContext
        {
        }
    }
}
