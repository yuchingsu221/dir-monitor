using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace WebService.Filters.Swagger
{
    public class ApiHeaderManagerFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            var controller = context.ApiDescription.ActionDescriptor.RouteValues["controller"].ToString();
            var action = context.ApiDescription.ActionDescriptor.RouteValues["action"].ToString();

            if (controller.Equals("Global", StringComparison.OrdinalIgnoreCase) &&
                action.Equals("Key", StringComparison.OrdinalIgnoreCase))
            { return; }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "CRYPTO-ID",
                Required = false,
                In = ParameterLocation.Header,
                Description = "加密ID",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                },

            });
        }
    }
}
