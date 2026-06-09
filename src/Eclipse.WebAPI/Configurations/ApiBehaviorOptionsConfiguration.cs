using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

internal sealed class ApiBehaviorOptionsConfiguration : IConfigureOptions<ApiBehaviorOptions>
{
    public void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Do not expose model state validtion errors per owasp recommendation: https://owasp.org/www-community/vulnerabilities/Information_Exposure_Through_Error_Message
            var problemDetails = new ValidationProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Detail = "The request containts invalid payload format. Please refer to documentation for more information.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    }
}
