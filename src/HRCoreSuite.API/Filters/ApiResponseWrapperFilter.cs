using System.Net;
using HRCoreSuite.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRCoreSuite.API.Filters;

public class ApiResponseWrapperFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null) return;

        if (context.Result is NoContentResult)
        {
            return;
        }
        
        if (context.Result is ObjectResult resultFor204 && resultFor204.StatusCode == (int)HttpStatusCode.NoContent)
        {
            return;
        }
        
        if (context.Result is ObjectResult res && res.Value is ApiResponse<object>)
        {
            return;
        }

        if (context.Result is ObjectResult objectResult)
        {
            var value = objectResult.Value;
            var statusCode = objectResult.StatusCode ?? 200;

            if (statusCode >= 200 && statusCode < 300)
            {
                var valueType = value?.GetType() ?? typeof(object);
                var apiResponseType = typeof(ApiResponse<>).MakeGenericType(valueType);
                var apiResponse = apiResponseType.GetMethod("SuccessResponse")!.Invoke(null, new[] { value });
                context.Result = new ObjectResult(apiResponse) { StatusCode = statusCode };
            }
            else
            {
                var apiResponse = ApiResponse<object>.FailResponse(value);
                context.Result = new ObjectResult(apiResponse) { StatusCode = statusCode };
            }
        }

        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            var statusCode = statusCodeResult.StatusCode;
            
            if (statusCode >= 200 && statusCode < 300)
            {
                var apiResponse = typeof(ApiResponse<object>).GetMethod("SuccessResponse")!.Invoke(null, new object?[] { null });
                context.Result = new ObjectResult(apiResponse) { StatusCode = statusCode };
            }
            else
            {
                var apiResponse = ApiResponse<object>.FailResponse(null);
                context.Result = new ObjectResult(apiResponse) { StatusCode = statusCode };
            }
        }
    }
}