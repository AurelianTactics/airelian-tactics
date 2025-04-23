using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

/// <summary>
/// Loads game configurations from JSON files.
/// </summary>
public class GameConfigLoader
{
    /// <summary>
    /// Loads a GameConfig from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>The loaded GameConfig object.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public static GameConfig LoadGameConfig(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Game configuration file not found: {filePath}");
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
            
            return JsonSerializer.Deserialize<GameConfig>(jsonString, options);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Error parsing game configuration from {filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves a GameConfig to a JSON file.
    /// </summary>
    /// <param name="gameConfig">The GameConfig to save.</param>
    /// <param name="filePath">The path to save the file to.</param>
    /// <exception cref="Exception">Thrown when there's an error saving the file.</exception>
    public static void SaveGameConfig(GameConfig gameConfig, string filePath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            string jsonString = JsonSerializer.Serialize(gameConfig, options);
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving game configuration to {filePath}: {ex.Message}", ex);
        }
    }

   
} 