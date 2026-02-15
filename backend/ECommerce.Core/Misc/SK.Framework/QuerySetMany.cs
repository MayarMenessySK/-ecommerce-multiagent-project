using System.Collections.Generic;

namespace SK.Framework;

/// <summary>
/// A query set expecting a list, array, or any IEnumerable<>
/// </summary>
/// <typeparam name="T"></typeparam>
public class QuerySetMany<T> : QuerySet<T>, IQuerySetMany<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:QueryMulti"/> class.
    /// </summary>
    public QuerySetMany()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:QueryMulti"/> class.
    /// </summary>
    public QuerySetMany(List<T> items)
        : base(multiple: items)
    {
    }

    List<T> IQuerySetMany<T>.Items => this.Multiple;
}
