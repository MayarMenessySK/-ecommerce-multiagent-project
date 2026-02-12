using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Addresses;

public class CreateAddressInput
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address line 1 is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Address line 1 must be between 5 and 200 characters")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address line 2 must not exceed 200 characters")]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "State must be between 2 and 100 characters")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Postal code must be between 3 and 20 characters")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 100 characters")]
    public string Country { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    [Required(ErrorMessage = "Address type is required")]
    [RegularExpression("^(Shipping|Billing)$", ErrorMessage = "Address type must be either 'Shipping' or 'Billing'")]
    public string AddressType { get; set; } = "Shipping";
}

public class UpdateAddressInput
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address line 1 is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Address line 1 must be between 5 and 200 characters")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address line 2 must not exceed 200 characters")]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "State must be between 2 and 100 characters")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Postal code must be between 3 and 20 characters")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 100 characters")]
    public string Country { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    [Required(ErrorMessage = "Address type is required")]
    [RegularExpression("^(Shipping|Billing)$", ErrorMessage = "Address type must be either 'Shipping' or 'Billing'")]
    public string AddressType { get; set; } = "Shipping";
}
