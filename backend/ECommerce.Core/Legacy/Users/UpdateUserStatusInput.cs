using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Users;

public class UpdateUserStatusInput
{
    [Required]
    public bool IsActive { get; set; }
}
