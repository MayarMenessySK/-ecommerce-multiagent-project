using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Reviews;

public class CreateReviewInput
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}

public class UpdateReviewInput
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}
