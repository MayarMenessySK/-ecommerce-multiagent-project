using System;

namespace SK.Framework;

public interface IQuerySetOne<out T>
{
    bool IsFound { get; }

    bool IsNotFound { get; }

    T Item { get; }

    Exception Exception { get; }
}
