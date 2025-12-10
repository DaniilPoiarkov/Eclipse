using Microsoft.OpenApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eclipse.WebAPI.Swagger;

public sealed class ContentLanguageHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Content-Language",
            In = ParameterLocation.Header,
            Description = "Specifies language of content. Default is [en], which will use english localization.",
            Required = false,
        });
    }
}
