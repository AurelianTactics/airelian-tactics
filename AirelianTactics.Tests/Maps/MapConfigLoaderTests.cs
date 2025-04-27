using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;

namespace AirelianTactics.Tests.Maps
{
    [TestClass]
    public class MapConfigLoaderTests
    {
        private string testMapConfigPath = null!;
        private string testSaveMapConfigPath = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create test map config file
            CreateTestMapConfig();
            
            // Initialize save path but don't create the file yet
            string testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            testSaveMapConfigPath = Path.Combine(testDir, "test_save_map.json");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete test config files if they exist
            if (File.Exists(testMapConfigPath))
            {
                File.Delete(testMapConfigPath);
            }
            
            if (File.Exists(testSaveMapConfigPath))
            {
                File.Delete(testSaveMapConfigPath);
            }
        }

        [TestMethod]
        public void LoadMapConfig_ValidFile_ReturnsMapConfig()
        {
            // Act
            var mapConfig = MapConfigLoader.LoadMapConfig(testMapConfigPath);
            
            // Assert
            Assert.IsNotNull(mapConfig);
            Assert.AreEqual("Test Map", mapConfig.General.MapName);
            Assert.AreEqual(2, mapConfig.Tiles.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LoadMapConfig_FileNotFound_ThrowsException()
        {
            // Arrange
            string nonExistentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "non_existent_file.json");
            
            // Act - This should throw a FileNotFoundException
            MapConfigLoader.LoadMapConfig(nonExistentFilePath);
            
            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        public void SaveMapConfig_ValidMapConfig_SavesFile()
        {
            // Arrange
            var mapConfig = new MapConfig
            {
                General = new MapGeneralConfig { MapName = "Saved Test Map" },
                Tiles = new System.Collections.Generic.List<TileConfig>
                {
                    new TileConfig
                    {
                        TileId = 1,
                        X = 0,
                        Y = 0,
                        Z = 0,
                        Standable = true,
                        Traversable = true,
                        Terrain = "grass",
                        CanPlayerStart = true
                    }
                }
            };
            
            // Act
            MapConfigLoader.SaveMapConfig(mapConfig, testSaveMapConfigPath);
            
            // Assert
            Assert.IsTrue(File.Exists(testSaveMapConfigPath));
            
            // Verify the saved content by loading it back
            var loadedConfig = MapConfigLoader.LoadMapConfig(testSaveMapConfigPath);
            Assert.AreEqual("Saved Test Map", loadedConfig.General.MapName);
            Assert.AreEqual(1, loadedConfig.Tiles.Count);
            Assert.AreEqual(1, loadedConfig.Tiles[0].TileId);
            Assert.AreEqual("grass", loadedConfig.Tiles[0].Terrain);
        }

        private void CreateTestMapConfig()
        {
            // Create a temporary directory for test files if it doesn't exist
            string testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            Directory.CreateDirectory(testDir);
            
            // Create the test map config file
            testMapConfigPath = Path.Combine(testDir, "test_map.json");
            
            string testMapConfigJson = @"{
  ""general"": {
    ""mapName"": ""Test Map""
  },
  ""tiles"": [
    {
      ""tileId"": 1,
      ""x"": 0,
      ""y"": 0,
      ""z"": 0,
      ""standable"": true,
      ""traversable"": true,
      ""terrain"": ""grass"",
      ""canPlayerStart"": true
    },
    {
      ""tileId"": 2,
      ""x"": 1,
      ""y"": 0,
      ""z"": 0,
      ""standable"": true,
      ""traversable"": true,
      ""terrain"": ""grass"",
      ""canPlayerStart"": false
    }
  ]
}";
            File.WriteAllText(testMapConfigPath, testMapConfigJson);
        }
    }
} 