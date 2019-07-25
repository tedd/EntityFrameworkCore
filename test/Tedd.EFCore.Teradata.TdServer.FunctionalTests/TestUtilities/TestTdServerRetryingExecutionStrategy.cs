// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Storage;
using Tedd.EFCore.Teradata;
using Teradata.Client.Provider;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestTdServerRetryingExecutionStrategy : TdServerRetryingExecutionStrategy
    {
        private const bool ErrorNumberDebugMode = false;

        private static readonly int[] _additionalErrorNumbers =
        {
            -1, // Physical connection is not usable
            -2, // Timeout
            1807, // Could not obtain exclusive lock on database 'model'
            42008, // Mirroring (Only when a database is deleted and another one is created in fast succession)
            42019 // CREATE DATABASE operation failed
        };

        public TestTdServerRetryingExecutionStrategy()
            : base(
                new DbContext(
                    new DbContextOptionsBuilder()
                        .EnableServiceProviderCaching(false)
                        .UseTdServer(TestEnvironment.DefaultConnection).Options),
                DefaultMaxRetryCount, DefaultMaxDelay, _additionalErrorNumbers)
        {
        }

        public TestTdServerRetryingExecutionStrategy(DbContext context)
            : base(context, DefaultMaxRetryCount, DefaultMaxDelay, _additionalErrorNumbers)
        {
        }

        public TestTdServerRetryingExecutionStrategy(DbContext context, TimeSpan maxDelay)
            : base(context, DefaultMaxRetryCount, maxDelay, _additionalErrorNumbers)
        {
        }

        public TestTdServerRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies)
            : base(dependencies, DefaultMaxRetryCount, DefaultMaxDelay, _additionalErrorNumbers)
        {
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            if (base.ShouldRetryOn(exception))
            {
                return true;
            }

            if (ErrorNumberDebugMode
                && exception is TdException sqlException)
            {
                var message = "Didn't retry on";
                foreach (TdError err in sqlException.Errors)
                {
                    message += " " + err.Number;
                }

                message += Environment.NewLine;
                throw new InvalidOperationException(message + exception, exception);
            }

            return exception is InvalidOperationException invalidOperationException
                   && invalidOperationException.Message == "Internal .Net Framework Data Provider error 6."
                ? true
                : false;
        }

        public new virtual TimeSpan? GetNextDelay(Exception lastException)
        {
            ExceptionsEncountered.Add(lastException);
            return base.GetNextDelay(lastException);
        }

        public static new bool Suspended
        {
            get => ExecutionStrategy.Suspended;
            set => ExecutionStrategy.Suspended = value;
        }
    }
}
