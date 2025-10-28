using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore;

[DebuggerStepThrough]
internal static class Check
{
    [ContractAnnotation("value:null => halt")]
    public static T NotNull<T>([NoEnumeration] T value, [InvokerParameterName] string parameterName)
    {
        if (ReferenceEquals(value, null))
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    [ContractAnnotation("value:null => halt")]
    public static T NotNull<T>(
        [NoEnumeration] T value,
        [InvokerParameterName] string parameterName,
        string propertyName)
    {
        if (ReferenceEquals(value, null))
        {
            NotEmpty(parameterName, nameof(parameterName));
            NotEmpty(propertyName, nameof(propertyName));
            throw new ArgumentException($"Property '{propertyName}' on '{parameterName}' cannot be null.");
        }

        return value;
    }

    [ContractAnnotation("value:null => halt")]
    public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, [InvokerParameterName] string parameterName)
    {
        NotNull(value, parameterName);

        if (value.Count == 0)
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentException("Collection argument is empty.", parameterName);
        }

        return value;
    }

    [ContractAnnotation("value:null => halt")]
    public static string NotEmpty(string? value, [InvokerParameterName] string parameterName)
    {
        if (value is null)
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentNullException(parameterName);
        }

        if (value.Trim().Length == 0)
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentException("Argument is empty.", parameterName);
        }

        return value;
    }

    public static string? NullButNotEmpty(string? value, [InvokerParameterName] string parameterName)
    {
        if (value is not null && value.Length == 0)
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentException("Argument is empty.", parameterName);
        }

        return value;
    }

    public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, [InvokerParameterName] string parameterName)
        where T : class
    {
        NotNull(value, parameterName);

        if (value.Any(e => e == null))
        {
            NotEmpty(parameterName, nameof(parameterName));
            throw new ArgumentException($"Collection '{parameterName}' contains null elements.");
        }

        return value;
    }
}
