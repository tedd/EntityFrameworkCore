// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TdServerTestStoreFactory : RelationalTestStoreFactory
    {
        public static TdServerTestStoreFactory Instance { get; } = new TdServerTestStoreFactory();

        protected TdServerTestStoreFactory()
        {
        }

        public override TestStore Create(string storeName)
            => TdServerTestStore.Create(storeName);

        public override TestStore GetOrCreate(string storeName)
            => TdServerTestStore.GetOrCreate(storeName);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkTdServer();
    }
}
