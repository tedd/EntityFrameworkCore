// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Tedd.EFCore.Teradata;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Tedd.EFCore.Teradata.TdServer.Update.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Tedd.EFCore.Teradata.Update
{
    public class TdServerModificationCommandBatchFactoryTest
    {
        [ConditionalFact]
        public void Uses_MaxBatchSize_specified_in_TdServerOptionsExtension()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseTdServer("Database=Crunchie", b => b.MaxBatchSize(1));

            var typeMapper = new TdServerTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var logger = new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>();

            var factory = new TdServerModificationCommandBatchFactory(
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
                optionsBuilder.Options);

            var batch = factory.Create();

            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.False(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }

        [ConditionalFact]
        public void MaxBatchSize_is_optional()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseTdServer("Database=Crunchie");

            var typeMapper = new TdServerTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var logger = new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>();

            var factory = new TdServerModificationCommandBatchFactory(
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
                optionsBuilder.Options);

            var batch = factory.Create();

            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }

        private class FakeDbContext : DbContext
        {
        }
    }
}
