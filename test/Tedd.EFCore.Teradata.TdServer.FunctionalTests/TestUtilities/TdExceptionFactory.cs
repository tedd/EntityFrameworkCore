// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Teradata.Client.Provider;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public static class TdExceptionFactory
    {
        public static TdException CreateTdException(int number, Guid? connectionId = null)
        {
            var errorCtors = typeof(TdError)
                .GetTypeInfo()
                .DeclaredConstructors;


            var con1 = errorCtors.First(
                c => c.GetParameters().Length == 3 && c.GetParameters()[2].ParameterType == typeof(string));
            var error = (TdError)con1
                .Invoke(new object[] { 1, number, "ErrorMessage" });
            var errors = (TdErrorCollection)typeof(TdErrorCollection)
                .GetTypeInfo()
                .DeclaredConstructors
                .First(c => c.GetParameters().Length == 0)
                .Invoke(null);

            typeof(TdErrorCollection).GetRuntimeMethods().Single(m => m.Name == "Add").Invoke(errors, new object[] { error });

            var exceptionCtors = typeof(TdException)
                .GetTypeInfo()
                .DeclaredConstructors;

            return (TdException)exceptionCtors.First(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(TdErrorCollection))
                .Invoke(new object[] { errors });
        }
    }
}
