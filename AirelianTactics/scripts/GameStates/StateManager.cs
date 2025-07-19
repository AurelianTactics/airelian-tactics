using System.Collections;
using System.Collections.Generic;
using System;
using AirelianTactics.Services;

/// <summary>
/// StateManager handles the transitions between different game states.
/// It maintains a reference to the current state and manages the state transitions.
/// Also manages shared services and game state that persists across states.
/// </summary>
public class StateManager
{
    /// <summary>
    /// The currently active state.
    /// </summary>
    private IState currentState;

    /// <summary>
    /// Dictionary of all registered states, mapped by their type.
    /// </summary>
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    /// <summary>
    /// Dictionary defining the flow between states.
    /// Key is the current state type, value is the next state type.
    /// </summary>
    private Dictionary<Type, Type> stateFlow = new Dictionary<Type, Type>();

    /// <summary>
    /// The shared game context that is passed between states.
    /// </summary>
    private GameContext gameContext;

    // Shared services that persist across all game states
    /// <summary>
    /// Shared unit service instance used by all states
    /// </summary>
    public UnitService UnitService { get; private set; }

    /// <summary>
    /// Shared spell service instance used by all states
    /// </summary>
    public SpellService SpellService { get; private set; }

    /// <summary>
    /// Shared status service instance used by all states
    /// </summary>
    public StatusService StatusService { get; private set; }

    /// <summary>
    /// Shared board instance used by all states
    /// </summary>
    public Board Board { get; private set; }

    /// <summary>
    /// Shared game time manager that persists queues across state transitions
    /// </summary>
    public GameTimeManager GameTimeManager { get; private set; }

    /// <summary>
    /// World tick counter for game time progression
    /// </summary>
    public int WorldTick { get; private set; } = -1;

    /// <summary>
    /// Event triggered when a state transition occurs.
    /// Provides the previous state and the new state.
    /// </summary>
    public event Action<IState, IState> OnStateChanged;

    /// <summary>
    /// Flag indicating if the current state has completed its work.
    /// </summary>
    private bool currentStateCompleted = false;

    /// <summary>
    /// Constructor initializes a new StateManager with a new GameContext and services.
    /// </summary>
    public StateManager()
    {
        gameContext = new GameContext();
        
        // Initialize shared services
        UnitService = new UnitService();
        SpellService = new SpellService();
        StatusService = new StatusService();
        Board = new Board();
        
        // Initialize game time manager with services
        GameTimeManager = new GameTimeManager(UnitService, SpellService, StatusService, Board);
    }

    /// <summary>
    /// Increment the world tick counter
    /// </summary>
    public void IncrementWorldTick()
    {
        WorldTick++;
    }

    /// <summary>
    /// Registers a state with the state manager.
    /// </summary>
    /// <param name="state">The state to register.</param>
    public void RegisterState(IState state)
    {
        Type stateType = state.GetType();
        
        // Assign the shared game context to the state
        state.GameContext = gameContext;
        
        if (states.ContainsKey(stateType))
        {
            // Replace existing state if it's already registered
            states[stateType] = state;
        }
        else
        {
            // Add new state
            states.Add(stateType, state);
        }
    }

    /// <summary>
    /// Defines the flow between states.
    /// </summary>
    /// <typeparam name="TFrom">The source state type.</typeparam>
    /// <typeparam name="TTo">The destination state type.</typeparam>
    public void DefineStateFlow<TFrom, TTo>()
        where TFrom : IState
        where TTo : IState
    {
        Type fromType = typeof(TFrom);
        Type toType = typeof(TTo);

        if (!states.ContainsKey(fromType))
        {
            throw new ArgumentException($"State of type {fromType.Name} is not registered with the StateManager.");
        }

        if (!states.ContainsKey(toType))
        {
            throw new ArgumentException($"State of type {toType.Name} is not registered with the StateManager.");
        }

        if (stateFlow.ContainsKey(fromType))
        {
            // Replace existing flow if it's already defined
            stateFlow[fromType] = toType;
        }
        else
        {
            // Add new flow
            stateFlow.Add(fromType, toType);
        }
    }

    /// <summary>
    /// Changes to a new state of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of state to change to.</typeparam>
    public void ChangeState<T>() where T : IState
    {
        ChangeState(typeof(T));
    }

    /// <summary>
    /// Changes to a new state of the specified type.
    /// </summary>
    /// <param name="stateType">The type of state to change to.</param>
    public void ChangeState(Type stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            throw new ArgumentException($"State of type {stateType.Name} is not registered with the StateManager.");
        }

        IState newState = states[stateType];
        
        // Don't change if it's the same state
        if (currentState == newState)
        {
            return;
        }

        // Exit the current state if it exists
        IState previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit();
        }

        // Set and enter the new state
        currentState = newState;
        
        // Ensure the new state has access to the shared game context
        currentState.GameContext = gameContext;
        
        currentStateCompleted = false;
        currentState.Enter();

        // Trigger the state changed event
        OnStateChanged?.Invoke(previousState, currentState);
    }

    /// <summary>
    /// Updates the current state.
    /// </summary>
    public void Update()
    {
        if (currentState != null)
        {
            // Update the current state
            currentState.Update();
            
            // Check if the state has completed its work
            if (currentState is State state && state.IsCompleted)
            {
                currentStateCompleted = true;
            }
            
            // If the current state has completed, transition to the next state
            if (currentStateCompleted)
            {
                Type currentStateType = currentState.GetType();
                if (stateFlow.ContainsKey(currentStateType))
                {
                    Type nextStateType = stateFlow[currentStateType];
                    ChangeState(nextStateType);
                }
            }
        }
    }

    /// <summary>
    /// Passes user input to the current state.
    /// </summary>
    /// <param name="input">The user input string</param>
    public void HandleInput(string input)
    {
        if (currentState != null)
        {
            currentState.HandleInput(input);
        }
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    /// <returns>The current state.</returns>
    public IState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Gets a registered state of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of state to get.</typeparam>
    /// <returns>The state of the specified type.</returns>
    public T GetState<T>() where T : IState
    {
        Type stateType = typeof(T);
        if (!states.ContainsKey(stateType))
        {
            throw new ArgumentException($"State of type {stateType.Name} is not registered with the StateManager.");
        }

        return (T)states[stateType];
    }

    /// <summary>
    /// Gets the shared game context.
    /// </summary>
    /// <returns>The game context.</returns>
    public GameContext GetGameContext()
    {
        return gameContext;
    }

    /// <summary>
    /// Marks the current state as completed.
    /// This will trigger a transition to the next state in the flow.
    /// </summary>
    public void CompleteCurrentState()
    {
        currentStateCompleted = true;
    }
} 