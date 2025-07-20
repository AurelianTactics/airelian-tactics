using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace AirelianTactics.Tests.GameStates
{
    [TestClass]
    public class StateManagerTests
    {
        private StateManager stateManager = null!;
        private Mock<IState> mockState1 = null!;
        private Mock<IState> mockState2 = null!;

        [TestInitialize]
        public void Setup()
        {
            // Arrange - Set up before each test
            stateManager = new StateManager();
            mockState1 = new Mock<IState>();
            mockState2 = new Mock<IState>();
            
            // We can't mock GetType(), so we'll use concrete classes for tests that need type info
        }

        [TestMethod]
        public void RegisterState_StateIsRegistered()
        {
            // Create a concrete test state instead of using a mock
            var concreteState = new TestState1();
            
            // Act
            stateManager.RegisterState(concreteState);
            
            // Assert
            Assert.AreEqual(concreteState, stateManager.GetState<TestState1>());
        }

        [TestMethod]
        public void ChangeState_StateChanges_EnterAndExitMethodsCalled()
        {
            // Use concrete state objects instead of mocks
            var concreteState1 = new TestState1();
            var concreteState2 = new TestState2();
            
            // Setup entry and exit tracking
            bool state1EnterCalled = false;
            bool state1ExitCalled = false;
            bool state2EnterCalled = false;
            
            // Modify the test states to track method calls
            concreteState1.OnEnter = () => state1EnterCalled = true;
            concreteState1.OnExit = () => state1ExitCalled = true;
            concreteState2.OnEnter = () => state2EnterCalled = true;
            
            // Act
            stateManager.RegisterState(concreteState1);
            stateManager.RegisterState(concreteState2);
            stateManager.ChangeState<TestState1>();
            stateManager.ChangeState<TestState2>();
            
            // Assert
            Assert.IsTrue(state1EnterCalled, "State 1 Enter method should have been called");
            Assert.IsTrue(state1ExitCalled, "State 1 Exit method should have been called");
            Assert.IsTrue(state2EnterCalled, "State 2 Enter method should have been called");
        }

        [TestMethod]
        public void DefineStateFlow_StateFlowDefined_TransitionsAutomatically()
        {
            // Arrange
            var completableState = new CompletableState(stateManager);
            var nextState = new TestState2();
            stateManager.RegisterState(completableState);
            stateManager.RegisterState(nextState);
            stateManager.DefineStateFlow<CompletableState, TestState2>();
            
            bool nextStateEnterCalled = false;
            nextState.OnEnter = () => nextStateEnterCalled = true;
            
            // Act
            stateManager.ChangeState<CompletableState>();
            completableState.Complete(); // Mark the state as completed
            stateManager.Update(); // Trigger state transition
            
            // Assert
            Assert.AreEqual(typeof(TestState2), stateManager.GetCurrentState().GetType());
            Assert.IsTrue(nextStateEnterCalled, "Next state Enter method should have been called");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetState_UnregisteredState_ThrowsException()
        {
            // Act - should throw ArgumentException
            stateManager.GetState<TestState1>();
        }

        [TestMethod]
        public void Update_StateIsUpdated()
        {
            // Arrange
            var testState = new TestState1();
            bool updateCalled = false;
            testState.OnUpdate = () => updateCalled = true;
            
            stateManager.RegisterState(testState);
            stateManager.ChangeState<TestState1>();
            
            // Act
            stateManager.Update();
            
            // Assert
            Assert.IsTrue(updateCalled, "State Update method should have been called");
        }

        // Helper classes for testing
        private class TestState1 : IState
        {
            public Action? OnEnter { get; set; }
            public Action? OnExit { get; set; }
            public Action? OnUpdate { get; set; }
            public Action<string>? OnHandleInput { get; set; }
            
            // Implement the GameContext property
            public GameContext GameContext { get; set; } = new GameContext();

            public void Enter() => OnEnter?.Invoke();
            public void Exit() => OnExit?.Invoke();
            public void Update() => OnUpdate?.Invoke();
            public void HandleInput(string input) => OnHandleInput?.Invoke(input);
        }

        private class TestState2 : IState
        {
            public Action? OnEnter { get; set; }
            public Action? OnExit { get; set; }
            public Action? OnUpdate { get; set; }
            public Action<string>? OnHandleInput { get; set; }
            
            // Implement the GameContext property
            public GameContext GameContext { get; set; } = new GameContext();

            public void Enter() => OnEnter?.Invoke();
            public void Exit() => OnExit?.Invoke();
            public void Update() => OnUpdate?.Invoke();
            public void HandleInput(string input) => OnHandleInput?.Invoke(input);
        }

        private class CompletableState : State
        {
            public CompletableState(StateManager stateManager) : base(stateManager) { }
            
            public void Complete()
            {
                CompleteState();
            }
        }
    }
} 