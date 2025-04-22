using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AirelianTactics.Tests.GameStates
{
    [TestClass]
    public class IStateTests
    {
        [TestMethod]
        public void IState_Interface_ImplementsExpectedMethods()
        {
            // Arrange
            var mockState = new Mock<IState>();
            bool enterCalled = false;
            bool exitCalled = false;
            bool updateCalled = false;
            
            // Setup mock to track method calls
            mockState.Setup(s => s.Enter()).Callback(() => enterCalled = true);
            mockState.Setup(s => s.Exit()).Callback(() => exitCalled = true);
            mockState.Setup(s => s.Update()).Callback(() => updateCalled = true);
            
            // Act
            var state = mockState.Object;
            state.Enter();
            state.Update();
            state.Exit();
            
            // Assert
            Assert.IsTrue(enterCalled, "Enter method should have been called");
            Assert.IsTrue(exitCalled, "Exit method should have been called");
            Assert.IsTrue(updateCalled, "Update method should have been called");
        }
        
        [TestMethod]
        public void IState_Implementation_CanBeUsedInStateManager()
        {
            // Arrange
            var stateManager = new StateManager();
            // Use a concrete implementation instead of a mock
            var concreteState = new TestState();
            bool enterCalled = false;
            concreteState.OnEnter = () => enterCalled = true;
            
            // Act
            stateManager.RegisterState(concreteState);
            stateManager.ChangeState<TestState>();
            
            // Assert
            Assert.IsTrue(enterCalled, "Enter method should have been called");
            Assert.AreEqual(concreteState, stateManager.GetCurrentState());
        }
        
        // Concrete test state implementation for testing
        private class TestState : IState
        {
            public System.Action? OnEnter { get; set; }
            public System.Action? OnExit { get; set; }
            public System.Action? OnUpdate { get; set; }
            
            public void Enter() => OnEnter?.Invoke();
            public void Exit() => OnExit?.Invoke();
            public void Update() => OnUpdate?.Invoke();
        }
    }
} 