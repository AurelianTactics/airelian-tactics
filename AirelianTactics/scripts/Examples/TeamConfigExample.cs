using System;
using System.IO;

/// <summary>
/// Example of using the TeamConfigLoader to load a team configuration from a JSON file.
/// </summary>
public class TeamConfigExample
{
    /// <summary>
    /// Demonstrates loading a team configuration from a JSON file.
    /// </summary>
    public static void DemoLoadTeamConfig()
    {
        string filePath = "Configs/team_sample.json";
        
        try
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Load the team configuration from the file
                TeamConfig teamConfig = TeamConfigLoader.LoadTeamConfig(filePath);
                
                // Display the loaded team information
                Console.WriteLine($"Loaded team: {teamConfig.TeamName} (ID: {teamConfig.TeamId})");
                Console.WriteLine($"Number of units: {teamConfig.Units.Count}");
                
                // Display information about each unit
                foreach (UnitConfig unit in teamConfig.Units)
                {
                    Console.WriteLine($"  - {unit.Name} (ID: {unit.UnitId})");
                    Console.WriteLine($"    HP: {unit.HP}, Speed: {unit.Speed}, PA: {unit.PA}");
                    Console.WriteLine($"    Move: {unit.Move}, Jump: {unit.Jump}, CT: {unit.InitialCT}");
                }
            }
            else
            {
                // File doesn't exist, create a sample configuration
                Console.WriteLine($"Configuration file not found at {filePath}");
                Console.WriteLine("Creating a sample configuration...");
                
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                
                // Create a sample team configuration
                TeamConfig sampleConfig = TeamConfigLoader.CreateSampleTeamConfig();
                
                // Save the sample configuration
                TeamConfigLoader.SaveTeamConfig(sampleConfig, filePath);
                
                Console.WriteLine($"Sample team configuration saved to {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
} 