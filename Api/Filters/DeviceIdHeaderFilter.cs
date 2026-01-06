using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class DeviceIdHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Проверяем, есть ли параметр deviceId в методе (из [FromHeader])
        var hasDeviceIdParam = context.MethodInfo.GetParameters()
            .Any(p => p.Name?.Equals("deviceId", StringComparison.OrdinalIgnoreCase) == true);

        if (!hasDeviceIdParam) return;

        // Находим header-параметр в OpenAPI
        var parameter = operation.Parameters.FirstOrDefault(p => 
            p.Name.Equals("deviceId", StringComparison.OrdinalIgnoreCase) && 
            p.In == ParameterLocation.Header);

        if (parameter != null)
        {
            parameter.Description = "Уникальный идентификатор устройства (рекомендуется GUID). " +
                                    "Для тестов в Swagger сгенерируйте на https://guidgenerator.com/ или используйте пример ниже.";
            
            // Фиксированный example (Swagger не умеет динамический Guid при каждом запросе)
            parameter.Example = new OpenApiString("550e8400-e29b-41d4-a716-446655440000");
            
            parameter.Required = true; // Если обязательно (как у тебя)
        }
        else
        {
            // Если параметр не найден автоматически — добавляем вручную (на всякий случай)
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "deviceId",
                In = ParameterLocation.Header,
                Description = "Уникальный идентификатор устройства (GUID). Пример: 550e8400-e29b-41d4-a716-446655440000",
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Example = new OpenApiString("550e8400-e29b-41d4-a716-446655440000") }
            });
        }
    }
}