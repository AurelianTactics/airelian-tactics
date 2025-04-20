using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Loads team configurations from JSON files.
/// </summary>
public class TeamConfigLoader
{
    /// <summary>
    /// Loads a TeamConfig from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>The loaded TeamConfig object.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public static TeamConfig LoadTeamConfig(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Team configuration file not found: {filePath}");
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
            
            // This line deserializes (converts) the JSON string into a TeamConfig object
            // JsonSerializer.Deserialize<T> is a method that takes a JSON string and converts it to an object of type T
            // In this case, we're converting the JSON string to a TeamConfig object
            // The options parameter configures how the deserialization works (case insensitivity, etc.)
            return JsonSerializer.Deserialize<TeamConfig>(jsonString, options);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Error parsing team configuration from {filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves a TeamConfig to a JSON file.
    /// </summary>
    /// <param name="teamConfig">The TeamConfig to save.</param>
    /// <param name="filePath">The path to save the file to.</param>
    /// <exception cref="Exception">Thrown when there's an error saving the file.</exception>
    public static void SaveTeamConfig(TeamConfig teamConfig, string filePath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            string jsonString = JsonSerializer.Serialize(teamConfig, options);
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving team configuration to {filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a sample TeamConfig for testing purposes.
    /// </summary>
    /// <returns>A sample TeamConfig.</returns>
    public static TeamConfig CreateSampleTeamConfig()
    {
        return new TeamConfig
        {
            TeamId = 1,
            TeamName = "Player Team",
            Units = new System.Collections.Generic.List<UnitConfig>
            {
                new UnitConfig
                {
                    UnitId = 1,
                    Name = "Warrior",
                    HP = 100,
                    Speed = 10,
                    PA = 50,
                    Move = 4,
                    Jump = 2
                },
                new UnitConfig
                {
                    UnitId = 2,
                    Name = "Archer",
                    HP = 80,
                    Speed = 12,
                    PA = 40,
                    Move = 3,
                    Jump = 1
                }
            }
        };
    }
} 