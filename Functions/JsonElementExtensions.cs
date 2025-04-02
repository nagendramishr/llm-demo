using System;
using System.Text.Json;

namespace Functions
{
    public static class JsonElementExtensions
    {
        public static string GetPropertyOrDefault(this JsonElement element, string propertyName, string defaultValue)
        {
            return element.TryGetProperty(propertyName, out JsonElement propertyElement) && propertyElement.ValueKind == JsonValueKind.String
                ? propertyElement.GetString() ?? defaultValue
                : defaultValue;
        }

        public static bool GetPropertyOrDefault(this JsonElement element, string propertyName, bool defaultValue)
        {
            return element.TryGetProperty(propertyName, out JsonElement propertyElement) && propertyElement.ValueKind == JsonValueKind.True || propertyElement.ValueKind == JsonValueKind.False
                ? propertyElement.GetBoolean()
                : defaultValue;
        }

        public static DateTime GetPropertyOrDefault(this JsonElement element, string propertyName, DateTime defaultValue)
        {
            return element.TryGetProperty(propertyName, out JsonElement propertyElement) && propertyElement.TryGetDateTime(out DateTime dateTimeValue)
                ? dateTimeValue
                : defaultValue;
        }

        public static Guid GetGuidPropertyOrDefault(this JsonElement element, string propertyName, Guid defaultValue)
        {
            return element.TryGetProperty(propertyName, out JsonElement propertyElement) && propertyElement.TryGetGuid(out Guid guidValue)
                ? guidValue
                : defaultValue;
        }
    }
}