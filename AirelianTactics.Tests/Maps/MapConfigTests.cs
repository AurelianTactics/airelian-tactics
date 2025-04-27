using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AirelianTactics.Tests.Maps
{
    [TestClass]
    public class MapConfigTests
    {
        [TestMethod]
        public void MapConfig_DefaultValues_ShouldBeInitialized()
        {
            // Act
            var mapConfig = new MapConfig();
            
            // Assert
            Assert.IsNotNull(mapConfig.General);
            Assert.IsNotNull(mapConfig.Tiles);
            Assert.AreEqual(string.Empty, mapConfig.MapFile);
            Assert.AreEqual(0, mapConfig.Tiles.Count);
        }
        
        [TestMethod]
        public void MapConfig_AddTile_ShouldAddToCollection()
        {
            // Arrange
            var mapConfig = new MapConfig();
            var tile = new TileConfig
            {
                TileId = 1,
                X = 0,
                Y = 0,
                Z = 0,
                Standable = true,
                Traversable = true,
                Terrain = "grass",
                CanPlayerStart = true
            };
            
            // Act
            mapConfig.Tiles.Add(tile);
            
            // Assert
            Assert.AreEqual(1, mapConfig.Tiles.Count);
            Assert.AreEqual(1, mapConfig.Tiles[0].TileId);
            Assert.AreEqual("grass", mapConfig.Tiles[0].Terrain);
        }
        
        [TestMethod]
        public void MapGeneralConfig_DefaultValues_ShouldBeInitialized()
        {
            // Act
            var mapGeneralConfig = new MapGeneralConfig();
            
            // Assert
            Assert.AreEqual(string.Empty, mapGeneralConfig.MapName);
        }
        
        [TestMethod]
        public void TileConfig_DefaultValues_ShouldBeInitialized()
        {
            // Act
            var tileConfig = new TileConfig();
            
            // Assert
            Assert.AreEqual(0, tileConfig.TileId);
            Assert.AreEqual(0, tileConfig.X);
            Assert.AreEqual(0, tileConfig.Y);
            Assert.AreEqual(0, tileConfig.Z);
            Assert.AreEqual(false, tileConfig.Standable);
            Assert.AreEqual(false, tileConfig.Traversable);
            Assert.AreEqual(string.Empty, tileConfig.Terrain);
            Assert.AreEqual(false, tileConfig.CanPlayerStart);
        }
    }
} 