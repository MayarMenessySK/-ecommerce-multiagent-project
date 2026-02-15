using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace ECommerce.Data.Linq;

public partial class LinqMetaData : ILinqMetaData
{
    private readonly IDataAccessAdapter _adapter;

    public LinqMetaData(IDataAccessAdapter adapter)
    {
        _adapter = adapter;
    }

    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.UserEntity> User => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.ProductEntity> Product => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.CategoryEntity> Category => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.CartEntity> Cart => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.CartItemEntity> CartItem => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.OrderEntity> Order => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.OrderItemEntity> OrderItem => throw new NotImplementedException("Run LLBLGen Pro");
    public ILLBLGenProQuery<ECommerce.Data.EntityClasses.ReviewEntity> Review => throw new NotImplementedException("Run LLBLGen Pro");

    public IDataAccessAdapter GetDataAccessAdapter() => _adapter;
}
