using SD.LLBLGen.Pro.ORMSupportClasses;

namespace ECommerce.Data.EntityClasses;

/// <summary>
/// Entity class for Cart table
/// TODO: This is a placeholder - run LLBLGen Pro to generate the actual entities
/// </summary>
public partial class CartEntity : EntityBase2
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
