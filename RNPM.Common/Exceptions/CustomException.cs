using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace RNPM.Common.Exceptions;

/// <summary>
/// Exception type for app exceptions
/// </summary>
public class CustomException : Exception
{
    public CustomException()
    { }

    public CustomException(string message)
        : base(message)
    { }

    public CustomException(string message, Exception innerException, List<IdentityError> errors)
        : base(message, innerException)
    {

    }

    public CustomException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

/// <summary>  
/// Different types of exceptions.  
/// </summary>  
public enum Exceptions
{
    NullReferenceException = 1,
    FileNotFoundException = 2,
    OverflowException = 3,
    OutOfMemoryException = 4,
    InvalidCastException = 5,
    ObjectDisposedException = 6,
    UnauthorizedAccessException = 7,
    NotImplementedException = 8,
    NotSupportedException = 9,
    InvalidOperationException = 10,
    TimeoutException = 11,
    ArgumentException = 12,
    FormatException = 13,
    StackOverflowException = 14,
    SqlException = 15,
    IndexOutOfRangeException = 16,
    IOException = 17,
    NotFoundException = 18
}
