using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    protected ActionResult<ApiResponse<T>> CreateSuccessResponse<T>(T data, string message = "Success", int statusCode = 200)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };

        return statusCode switch
        {
            201 => CreatedAtAction(null, response),
            _ => Ok(response)
        };
    }

    protected IActionResult CreateErrorResponse(string code, string message, object? details = null, int statusCode = 500)
    {
        var errorResponse = new ApiErrorResponse
        {
            Error = new ErrorDetail
            {
                Code = code,
                Message = message,
                Details = details
            }
        };

        return statusCode switch
        {
            400 => BadRequest(errorResponse),
            401 => Unauthorized(errorResponse),
            403 => StatusCode(403, errorResponse),
            404 => NotFound(errorResponse),
            _ => StatusCode(statusCode, errorResponse)
        };
    }
    
    protected IActionResult CreateValidationErrorResponse(Dictionary<string, string[]> errors)
    {
        return CreateErrorResponse("VALIDATION_ERROR", "Validation failed", errors, 400);
    }
}
