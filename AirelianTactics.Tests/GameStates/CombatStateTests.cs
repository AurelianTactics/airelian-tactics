using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using AirelianTactics.Services;

namespace AirelianTactics.Tests.GameStates
{
    [TestClass]
    public class CombatStateTests
    {
        private Mock<StateManager> mockStateManager = null!;
        private CombatState combatState = null!;

        [TestInitialize]
        public void Setup()
        {
            // Arrange - Create mock state manager for each test
            mockStateManager = new Mock<StateManager>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up after each test
            combatState = null!;
            mockStateManager = null!;
        }

        [TestMethod]
        public void Constructor_InitializesServicesAndInheritsCorrectly()
        {
            // Act - Create the CombatState
            combatState = new CombatState(mockStateManager.Object);

            // Assert - Verify the constructor properly initializes everything
            
            // 1. Verify it inherits from State correctly
            Assert.IsInstanceOfType(combatState, typeof(State), "CombatState should inherit from State");
            Assert.IsInstanceOfType(combatState, typeof(IState), "CombatState should implement IState interface");

            // 2. Verify initial state flags
            Assert.IsFalse(combatState.IsCompleted, "IsCompleted should be false initially");

            // 3. Verify private fields are initialized correctly using reflection
            // Check hasCompletedCombat field
            var hasCompletedCombatField = typeof(CombatState).GetField("hasCompletedCombat", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(hasCompletedCombatField, "hasCompletedCombat field should exist");
            var hasCompletedCombatValue = (bool)hasCompletedCombatField!.GetValue(combatState)!;
            Assert.IsFalse(hasCompletedCombatValue, "hasCompletedCombat should be false initially");

            // Check unitService field is initialized
            var unitServiceField = typeof(CombatState).GetField("unitService", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(unitServiceField, "unitService field should exist");
            var unitServiceValue = unitServiceField!.GetValue(combatState);
            Assert.IsNotNull(unitServiceValue, "unitService should be initialized");
            Assert.IsInstanceOfType(unitServiceValue, typeof(UnitService), "unitService should be of type UnitService");

            // Check combatTeamManager field is initialized
            var combatTeamManagerField = typeof(CombatState).GetField("combatTeamManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(combatTeamManagerField, "combatTeamManager field should exist");
            var combatTeamManagerValue = combatTeamManagerField!.GetValue(combatState);
            Assert.IsNotNull(combatTeamManagerValue, "combatTeamManager should be initialized");
            Assert.IsInstanceOfType(combatTeamManagerValue, typeof(CombatTeamManager), "combatTeamManager should be of type CombatTeamManager");

            // 4. Verify other fields are not prematurely initialized (should be null until Enter() is called)
            var victoryConditionField = typeof(CombatState).GetField("victoryCondition", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(victoryConditionField, "victoryCondition field should exist");
            var victoryConditionValue = victoryConditionField!.GetValue(combatState);
            Assert.IsNull(victoryConditionValue, "victoryCondition should be null until Enter() is called");

            var allianceManagerField = typeof(CombatState).GetField("allianceManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(allianceManagerField, "allianceManager field should exist");
            var allianceManagerValue = allianceManagerField!.GetValue(combatState);
            Assert.IsNull(allianceManagerValue, "allianceManager should be null until Enter() is called");

            // 5. Verify the state manager reference is stored correctly
            var stateManagerField = typeof(State).GetField("stateManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(stateManagerField, "stateManager field should exist in base State class");
            var stateManagerValue = stateManagerField!.GetValue(combatState);
            Assert.AreSame(mockStateManager.Object, stateManagerValue, "StateManager reference should be stored correctly");
        }
    }
} 