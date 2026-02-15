using System;
using System.Collections.Generic;

namespace SK.Framework;

public interface IQuerySetMany<T>
{
    int Count { get; }

    bool IsFound { get; }

    bool IsNotFound { get; }

    List<T> Items { get; }

    Exception Exception { get; }
}
