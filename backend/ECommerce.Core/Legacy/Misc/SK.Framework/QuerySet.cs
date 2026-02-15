using System;
using System.Collections.Generic;

namespace SK.Framework;

public static class QuerySet
{
    public static IQuerySetOne<T> One<T>(T item) where T : class
        => new QuerySetOne<T>(item);

    public static IQuerySetMany<T> Many<T>(List<T> items) where T : class
        => new QuerySetMany<T>(items);

    public static QueryResults<T> Paging<T>(List<T> results, int total) where T : class
        => new QueryResults<T>(results, total);
}

/// <summary>
/// This is a class to hold query results with several nice properties
/// </summary>
/// <typeparam name="T"></typeparam>
public class QuerySet<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:QuerySet"/> class.
    /// By ignoring all the optional parameters, the query set is in status of NotFound
    /// </summary>
    /// <param name="single">Set a value here if a single value is returned</param>
    /// <param name="multiple">Set a value here if a list of value is returned</param>
    public QuerySet(T single = null, List<T> multiple = null)
    {
        if (single == null && multiple == null)
            NotFound();
        else if (single != null)
            Found(single);
        else
            Found(multiple);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:QuerySet"/> class.
    /// </summary>
    public QuerySet() => Type = QuerySetType.None;

    /// <summary>
    /// Gets or sets the type of the query set.
    /// </summary>
    /// <value>The type of the query set.</value>
    public QuerySetType Type { get; private set; }

    public void NotFound()
    {
        Type = QuerySetType.None;
    }

    private T _single;

    public T Single => _single;

    protected List<T> _multiple;

    public List<T> Multiple => _multiple;

    /// <summary>
    /// Query returns one result
    /// </summary>
    /// <param name="single"></param>
    public void Found(T single)
    {
        if (single == null)
        {
            NotFound();
        }
        else
        {
            Type = QuerySetType.Single;
            _single = single;
        }
    }

    /// <summary>
    /// Query returns one or more result
    /// </summary>
    /// <param name="multiple"></param>
    public void Found(List<T> multiple)
    {
        if (multiple == null)
        {
            NotFound();
        }
        else if (multiple.Count == 0)
        {
            _multiple = multiple;
            NotFound();
        }
        else
        {
            Type = QuerySetType.Multiple;
            _multiple = multiple;
        }
    }

    private Exception _ex;

    public Exception Exception => _ex;

    public void Error(Exception ex)
    {
        _ex = ex;
        Type = QuerySetType.QueryError;
    }

    public int Count
    {
        get
        {
            switch (Type)
            {
                case QuerySetType.None: return 0;
                case QuerySetType.Single: return 1;
                case QuerySetType.Multiple: return _multiple.Count;
                default: return 0;
            }
        }
    }

    public bool IsFound => Type != QuerySetType.None && Type != QuerySetType.QueryError;

    public bool IsNotFound => !IsFound;
}
