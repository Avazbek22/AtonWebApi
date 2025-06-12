using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Swagger;

public class AddCurrentUserHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Current-User",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Логин текущего пользователя",
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}