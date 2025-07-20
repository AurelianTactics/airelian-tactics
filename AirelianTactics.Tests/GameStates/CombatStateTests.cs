using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using AirelianTactics.Services;

namespace AirelianTactics.Tests.GameStates
{
    [TestClass]
    public class CombatStateTests
    {
        private StateManager stateManager = null!;
        private CombatState combatState = null!;

        [TestInitialize]
        public void Setup()
        {
            // Arrange - Create real state manager for each test
            stateManager = new StateManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up after each test
            combatState = null!;
            stateManager = null!;
        }

        [TestMethod]
        public void Constructor_InitializesServicesAndInheritsCorrectly()
        {
            // Act - Create the CombatState
            combatState = new CombatState(stateManager);

            // Assert - Verify the constructor properly initializes everything
            
            // 1. Verify it inherits from State correctly
            Assert.IsInstanceOfType(combatState, typeof(State), "CombatState should inherit from State");
            Assert.IsInstanceOfType(combatState, typeof(IState), "CombatState should implement IState interface");

            // 2. Verify initial state flags
            Assert.IsFalse(combatState.IsCompleted, "IsCompleted should be false initially");

            // 3. Verify services are null before Enter() is called (correct design)
            var unitServiceField = typeof(CombatState).GetField("unitService", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(unitServiceField, "unitService field should exist");
            var unitServiceValue = unitServiceField!.GetValue(combatState);
            Assert.IsNull(unitServiceValue, "unitService should be null until Enter() is called");

            // 4. Call Enter() to initialize services (this will get services from real StateManager)
            combatState.Enter();

            // 5. Verify services are now initialized after Enter()
            unitServiceValue = unitServiceField!.GetValue(combatState);
            Assert.IsNotNull(unitServiceValue, "unitService should be initialized after Enter()");
            Assert.AreSame(stateManager.UnitService, unitServiceValue, "unitService should be the same instance from StateManager");

            // Check other basic services are initialized
            var spellServiceField = typeof(CombatState).GetField("spellService", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(spellServiceField, "spellService field should exist");
            var spellServiceValue = spellServiceField!.GetValue(combatState);
            Assert.IsNotNull(spellServiceValue, "spellService should be initialized after Enter()");
            Assert.AreSame(stateManager.SpellService, spellServiceValue, "spellService should be the same instance from StateManager");

            // 6. Verify the state manager reference is stored correctly
            var stateManagerField = typeof(State).GetField("stateManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(stateManagerField, "stateManager field should exist in base State class");
            var stateManagerValue = stateManagerField!.GetValue(combatState);
            Assert.AreSame(stateManager, stateManagerValue, "StateManager reference should be stored correctly");
        }
    }
} 