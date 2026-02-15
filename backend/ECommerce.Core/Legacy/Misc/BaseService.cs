namespace ECommerce.Core.Misc;

public abstract class BaseService
{
    protected void ValidateEntity<T>(T entity) where T : class
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
    }

    protected void ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Invalid ID provided", nameof(id));
        }
    }

    protected void ThrowNotFound(string entityName, Guid id)
    {
        throw new KeyNotFoundException($"{entityName} with ID {id} not found");
    }

    protected void ThrowNotFound(string message)
    {
        throw new KeyNotFoundException(message);
    }

    protected void ThrowBadRequest(string message)
    {
        throw new InvalidOperationException(message);
    }

    protected void ThrowUnauthorized(string message = "Unauthorized access")
    {
        throw new UnauthorizedAccessException(message);
    }
}
