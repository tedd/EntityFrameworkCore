// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tedd.EFCore.Teradata.TdServer.Diagnostics.Internal;
using Tedd.EFCore.Teradata.TdServer.Internal;
using Tedd.EFCore.Teradata.TdServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Teradata.Client.Provider;
using Tedd.EFCore.Teradata;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TdServerTestHelpers : TestHelpers
    {
        protected TdServerTestHelpers()
        {
        }

        public static TdServerTestHelpers Instance { get; } = new TdServerTestHelpers();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
            => services.AddEntityFrameworkTdServer();

        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseTdServer(new TdConnection("Database=DummyDatabase"));

        public override LoggingDefinitions LoggingDefinitions { get; } = new TdServerLoggingDefinitions();
    }
}
