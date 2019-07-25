// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TdServerNorthwindTestStoreFactory : TdServerTestStoreFactory
    {
        public const string Name = "Northwind";
        public static readonly string NorthwindConnectionString = TdServerTestStore.CreateConnectionString(Name);
        public static new TdServerNorthwindTestStoreFactory Instance { get; } = new TdServerNorthwindTestStoreFactory();

        protected TdServerNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => TdServerTestStore.GetOrCreate(Name, "Northwind.tdsql");
    }
}
