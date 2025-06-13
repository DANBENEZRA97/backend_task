using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using EventManagementSystem.Data.Entities;

public class EventCreateExampleFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Event))
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(0),
                ["name"] = new OpenApiString("string"),
                ["startDate"] = new OpenApiString("2025-06-09T17:48:16.443Z"),
                ["endDate"] = new OpenApiString("2025-06-09T17:48:16.443Z"),
                ["maxRegistrations"] = new OpenApiInteger(0),
                ["location"] = new OpenApiString("string"),
                ["eventUsers"] = new OpenApiArray() // תצוגה ריקה
            };
        }
    }
}
