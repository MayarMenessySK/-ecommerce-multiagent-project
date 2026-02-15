//using Serilog;
//using SK.Data.ORM;
//using Sk.Framework;
//using System;
//using System.Data;
//using System.Security.Authentication;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using Nrea.Data.DatabaseSpecific;
//using Nrea.Data.Linq;

using Serilog;

namespace SK.Framework;

public static class CodeTemplate
{
    //public static async Task<Result<T>> SafeImpl<T>(IDisposable disposable1, IDisposable disposable2, Func<Task<Result<T>>> variantImpl)
    //{
    //    try
    //    {
    //        return await variantImpl();
    //    }
    //    catch (Exception exp)
    //    {
    //        HandleException(exp);
    //        return Result<T>.False(exp);
    //    }
    //    finally
    //    {
    //        disposable1?.Dispose();
    //        disposable2?.Dispose();
    //    }
    //}

    //public static async Task<Result<T>> SafeImpl<T>(IDisposable disposable1, Func<Task<Result<T>>> variantImpl)
    //{
    //    try
    //    {
    //        return await variantImpl();
    //    }
    //    catch (Exception exp)
    //    {
    //        HandleException(exp);
    //        return Result<T>.False(exp);
    //    }
    //    finally
    //    {
    //        disposable1?.Dispose();
    //    }
    //}

    //public static async Task<Result<T>> SafeImpl<T>(Func<Task<Result<T>>> variantImpl)
    //{
    //    try
    //    {
    //        return await variantImpl();
    //    }
    //    catch (Exception exp)
    //    {
    //        HandleException(exp);
    //        return Result<T>.False(exp);
    //    }
    //}

    //public static Result<T> SafeImpl<T>(Func<Result<T>> variantImpl)
    //{
    //    try
    //    {
    //        return variantImpl();
    //    }
    //    catch (Exception exp)
    //    {
    //        HandleException(exp);
    //        return Result<T>.False(exp);
    //    }
    //}

    //public static void SafeImpl(Action variantImpl)
    //{
    //    try
    //    {
    //        variantImpl();
    //    }
    //    catch (Exception exp)
    //    {
    //        HandleException(exp);
    //    }
    //}

    //public async static Task<Result<T>> SafeDbTransactionImpl<T>(IsolationLevel isolationLevelToUse, string transactionName, Func<DataAccessAdapter, Task<Result<T>>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            adapter.StartTransaction(isolationLevelToUse, transactionName);

    //            var result = await variantImpl(adapter);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            adapter.Rollback();

    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public async static Task<Result<T>> SafeSimpleDbWriteImpl<T>(Func<DataAccessAdapter, Task<Result<T>>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var result = await variantImpl(adapter);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            adapter.Rollback();

    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public static Result<T> SafeSimpleDbWriteImpl<T>(Func<DataAccessAdapter, Result<T>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var result = variantImpl(adapter);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            adapter.Rollback();

    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public async static Task SafeSimpleDbWriteImpl(Func<DataAccessAdapter, Task> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            await variantImpl(adapter);
    //        }
    //        catch (Exception exp)
    //        {
    //            adapter.Rollback();

    //            HandleException(exp);
    //        }
    //    }
    //}

    //public static async Task<Result<T>> SafeSimpleDbReadImpl<T>(Func<LinqMetaData, Task<Result<T>>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var meta = new LinqMetaData(adapter);

    //            var result = await variantImpl(meta);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public static Result<T> SafeSimpleDbReadImpl<T>(Func<LinqMetaData, Result<T>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var meta = new LinqMetaData(adapter);

    //            var result = variantImpl(meta);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public static async Task<Result<T>> SafeSimpleDbReadImpl<T>(Func<DataAccessAdapter, Task<Result<T>>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var result = await variantImpl(adapter);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    //public static Result<T> SafeSimpleDbReadImpl<T>(Func<DataAccessAdapter, Result<T>> variantImpl)
    //{
    //    using (var adapter = Adapter.Create())
    //    {
    //        try
    //        {
    //            var result = variantImpl(adapter);
    //            return result;
    //        }
    //        catch (Exception exp)
    //        {
    //            HandleException(exp);
    //            return Result<T>.False(exp);
    //        }
    //    }
    //}

    public static void HandleException(Exception exp)
    {
        Log.Error(exp, string.Empty);
    }
}
