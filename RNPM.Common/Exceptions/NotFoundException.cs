﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RNPM.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base($"{message}")
    {
    }

    public NotFoundException(string name, object key)
        : base($"{name} ({key}) was not found.")
    {
    }
}
