using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.ComponentModel;
using System.Reflection;

namespace Eclipse.WebAPI.Swagger;

public sealed class DescriptionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Description = context.MethodInfo.GetCustomAttributes<DescriptionAttribute>()
            .Select(a => $"<p>{a.Description}<p/>")
            .Join(Environment.NewLine);
    }
}
