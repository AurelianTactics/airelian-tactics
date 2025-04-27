using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace AirelianTactics.Tests.GameStates
{
    [TestClass]
    public class GameContextTests
    {
        private StateManager stateManager = null!;
        private GameInitState initState = null!;
        private MapSetupState mapSetupState = null!;
        private string testGameConfigPath = null!;
        private List<string> testTeamConfigPaths = new List<string>();

        [TestInitialize]
        public void Setup()
        {
            // Create test config files
            CreateTestConfigs();

            // Setup state manager and states
            stateManager = new StateManager();
            initState = new TestGameInitState(stateManager, testGameConfigPath);
            mapSetupState = new MapSetupState(stateManager);

            // Register the states
            stateManager.RegisterState(initState);
            stateManager.RegisterState(mapSetupState);

            // Define the flow - use the exact types that were registered
            stateManager.DefineStateFlow<TestGameInitState, MapSetupState>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete test config files if they exist
            if (File.Exists(testGameConfigPath))
            {
                File.Delete(testGameConfigPath);
            }
            
            foreach (var path in testTeamConfigPaths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void GameContext_ShouldBeCreatedByStateManager()
        {
            // Assert
            Assert.IsNotNull(stateManager.GetGameContext());
            Assert.IsInstanceOfType(stateManager.GetGameContext(), typeof(GameContext));
        }

        [TestMethod]
        public void GameContext_ShouldBeSharedBetweenStates()
        {
            // Act
            stateManager.ChangeState<TestGameInitState>();
            
            // Assert - both states should reference the same GameContext instance
            Assert.IsNotNull(initState.GameContext);
            Assert.IsNotNull(mapSetupState.GameContext);
            Assert.AreSame(initState.GameContext, mapSetupState.GameContext);
        }

        [TestMethod]
        public void GameInitState_ShouldLoadGameConfigIntoGameContext()
        {
            // Act
            stateManager.ChangeState<TestGameInitState>();
            stateManager.Update(); // This will trigger the config loading
            
            // Assert
            Assert.IsNotNull(initState.GameContext.GameConfig);
            Assert.AreEqual("LastTeamStanding", initState.GameContext.GameConfig.General.VictoryCondition);
            Assert.AreEqual(2, initState.GameContext.GameConfig.Teams.Count);
            Assert.AreEqual(1, initState.GameContext.GameConfig.General.Alliances.Count);
        }

        [TestMethod]
        public void GameInitState_ShouldLoadMultipleTeamsIntoGameContext()
        {
            // Act
            stateManager.ChangeState<TestGameInitState>();
            stateManager.Update(); // This will trigger the team config loading
            
            // Assert
            Assert.IsNotNull(initState.GameContext.Teams);

            // to do: when team structure is finalized maybe some more detailed tests
        }

        [TestMethod]
        public void MapSetupState_ShouldHaveAccessToAllTeams()
        {
            // Arrange
            stateManager.ChangeState<TestGameInitState>();
            stateManager.Update(); // Load team config in GameInitState
            
            // Act
            stateManager.ChangeState<MapSetupState>();
            
            // Assert
            Assert.IsNotNull(mapSetupState.GameContext.Teams);
            Assert.AreEqual(2, mapSetupState.GameContext.Teams.Count);
            
            // Check first team
            var team1 = mapSetupState.GameContext.Teams[0];
            Assert.AreEqual("Test Team 1", team1.TeamName);
            
            // Check second team
            var team2 = mapSetupState.GameContext.Teams[1];
            Assert.AreEqual("Test Team 2", team2.TeamName);
            
            // Verify backwards compatibility
            Assert.AreEqual(team1, mapSetupState.GameContext.TeamConfig);
        }

        private void CreateTestConfigs()
        {
            // Create a temporary directory for test files if it doesn't exist
            string testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            Directory.CreateDirectory(testDir);
            
            // Create the first test team config file
            string testTeamPath1 = Path.Combine(testDir, "test_team1.json");
            testTeamConfigPaths.Add(testTeamPath1);
            
            string testTeamJson1 = @"{
  ""teamId"": ""0"",
  ""teamName"": ""Test Team 1"",
  ""units"": [
    {
      ""unitId"": 1,
      ""name"": ""Test Unit 1"",
      ""hp"": 100,
      ""speed"": 10,
      ""pa"": 5,
      ""move"": 4,
      ""jump"": 2,
      ""initialCT"": 0
    },
    {
      ""unitId"": 2,
      ""name"": ""Test Unit 2"",
      ""hp"": 80,
      ""speed"": 12,
      ""pa"": 4,
      ""move"": 3,
      ""jump"": 1,
      ""initialCT"": 0
    }
  ]
}";
            File.WriteAllText(testTeamPath1, testTeamJson1);
            
            // Create the second test team config file
            string testTeamPath2 = Path.Combine(testDir, "test_team2.json");
            testTeamConfigPaths.Add(testTeamPath2);
            
            string testTeamJson2 = @"{
  ""teamId"": ""1"",
  ""teamName"": ""Test Team 2"",
  ""units"": [
    {
      ""unitId"": 3,
      ""name"": ""Test Unit 3"",
      ""hp"": 110,
      ""speed"": 9,
      ""pa"": 6,
      ""move"": 3,
      ""jump"": 2,
      ""initialCT"": 0
    },
    {
      ""unitId"": 4,
      ""name"": ""Test Unit 4"",
      ""hp"": 75,
      ""speed"": 14,
      ""pa"": 3,
      ""move"": 5,
      ""jump"": 1,
      ""initialCT"": 0
    }
  ]
}";
            File.WriteAllText(testTeamPath2, testTeamJson2);
            
            // Create the test game config file
            testGameConfigPath = Path.Combine(testDir, "test_game_config.json");
            
            string testGameConfigJson = @"{
  ""general"": {
    ""victoryCondition"": ""LastTeamStanding"",
    ""alliances"": [
      {
        ""0"": {""1"":""Enemy""},
        ""1"": {""0"":""Enemy""}
      }
    ]
  },
  ""teams"": [
    """ + testTeamPath1.Replace("\\", "\\\\") + @""",
    """ + testTeamPath2.Replace("\\", "\\\\") + @"""
  ],
  ""map"": {
    ""mapFile"": ""TestFiles/test_map.json""
  }
}";
            File.WriteAllText(testGameConfigPath, testGameConfigJson);
        }

        // Test implementation of GameInitState that uses a custom config path
        private class TestGameInitState : GameInitState
        {
            private readonly string gameConfigPath;

            public TestGameInitState(StateManager stateManager, string gameConfigPath) : base(stateManager)
            {
                this.gameConfigPath = gameConfigPath;
            }

            // Override the LoadGameConfiguration method to use our test config
            protected override void LoadGameConfiguration()
            {
                // Check if the file exists
                if (!File.Exists(gameConfigPath))
                {
                    throw new FileNotFoundException($"Test game configuration file not found at path: {gameConfigPath}");
                }
                
                // Use the GameConfigLoader to load the game configuration
                GameConfig gameConfig = GameConfigLoader.LoadGameConfig(gameConfigPath);
                
                // Store the game configuration in the GameContext
                GameContext.GameConfig = gameConfig;
            }
        }
    }
} 