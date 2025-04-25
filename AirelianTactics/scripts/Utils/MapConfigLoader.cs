using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Loads map configurations from JSON files.
/// </summary>
public class MapConfigLoader
{
    /// <summary>
    /// Loads a MapConfig from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>The loaded MapConfig object.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public static MapConfig LoadMapConfig(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Map configuration file not found: {filePath}");
        }
        
        string jsonString = File.ReadAllText(filePath);
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            return JsonSerializer.Deserialize<MapConfig>(jsonString, options);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Error parsing map configuration from {filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves a MapConfig to a JSON file.
    /// </summary>
    /// <param name="mapConfig">The MapConfig to save.</param>
    /// <param name="filePath">The path to save the file to.</param>
    /// <exception cref="Exception">Thrown when there's an error saving the file.</exception>
    public static void SaveMapConfig(MapConfig mapConfig, string filePath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            string jsonString = JsonSerializer.Serialize(mapConfig, options);
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving map configuration to {filePath}: {ex.Message}", ex);
        }
    }
} 