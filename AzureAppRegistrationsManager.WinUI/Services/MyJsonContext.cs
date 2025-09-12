using System.Text.Json.Serialization;
using GraphApplication = Microsoft.Graph.Models.Application;

namespace AzureAppRegistrationsManager.WinUI.Services;

/// <summary>
/// Trim analysis warning IL2026: ...: Using member 'System.Text.Json.JsonSerializer.Deserialize{TValue}(String, JsonSerializerOptions)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code.
/// JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.
/// </summary>
[JsonSerializable(typeof(GraphApplication))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, WriteIndented = true)]
internal partial class MyJsonContext : JsonSerializerContext;