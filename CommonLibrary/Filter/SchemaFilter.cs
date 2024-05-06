using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Filter
{
    public class SchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                schema.Description =
                  (schema.Description ?? string.Empty) +
                  string.Join(",", EnumExtensions.ToEnums(type).Select(x => $"{x}={Convert.ToInt16(x)}"));
            }
        }
    }

    public static class EnumExtensions
    {
        public static IEnumerable<Enum> ToEnums(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException();
            return Enum.GetValues(t).Cast<Enum>();
        }
    }
}
