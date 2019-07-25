// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Tedd.EFCore.Teradata.Migrations.Operations
{
    /// <summary>
    ///     A SQL Server-specific <see cref="MigrationOperation" /> to drop a database.
    /// </summary>
    public class TdServerDropDatabaseOperation : MigrationOperation
    {
        /// <summary>
        ///     The name of the database.
        /// </summary>
        public virtual string Name { get; [param: NotNull] set; }
    }
}
