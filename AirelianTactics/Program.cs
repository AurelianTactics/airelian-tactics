﻿using System;

namespace AirelianTactics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("    Airelian Tactics Game");
            Console.WriteLine("=================================");
            
            // Create the state manager
            StateManager stateManager = new StateManager();
            
            // Subscribe to state change events
            stateManager.OnStateChanged += (previousState, newState) => 
            {
                // this ocures after the enter and initialization so the timing might be weird in the terminal
                Console.WriteLine($"---State changed from {(previousState != null ? previousState.GetType().Name : "null")} to {newState.GetType().Name}");
            };
            
            // Create and register all game states
            GameInitState initState = new GameInitState(stateManager);
            CombatState combatState = new CombatState(stateManager);
            UnitActionState unitActionStateInstance = new UnitActionState(stateManager);
            AIActionState aiActionStateInstance = new AIActionState(stateManager);
            GameEndState endState = new GameEndState(stateManager);
            
            stateManager.RegisterState(initState);
            stateManager.RegisterState(combatState);
            stateManager.RegisterState(unitActionStateInstance);
            stateManager.RegisterState(aiActionStateInstance);
            stateManager.RegisterState(endState);
            
            // Define the state flow
            stateManager.DefineStateFlow<GameInitState, CombatState>();
            // CombatState to UnitActionState/AIActionState is event driven. This is inconsistent with the other states.
            // may or may not want to change this
            //stateManager.DefineStateFlow<CombatState, UnitActionState>();
            stateManager.DefineStateFlow<UnitActionState, CombatState>();
            stateManager.DefineStateFlow<AIActionState, CombatState>();
            stateManager.DefineStateFlow<CombatState, GameEndState>();
            
            // Start with the game initialization state
            stateManager.ChangeState<GameInitState>();
            
            // Run the game loop
            bool isRunning = true;
            while (isRunning)
            {
                // Update the current state
                stateManager.Update();
                
                // Check if we've reached the end state
                if (stateManager.GetCurrentState() is GameEndState)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                    isRunning = false;
                }
                else
                {
                    // Handle input for states that need it (like UnitActionState)
                    IState currentState = stateManager.GetCurrentState();
                    if (currentState is UnitActionState unitActionState && unitActionState.IsWaitingForInput)
                    {
                        // Get user input and pass it to the state
                        string input = Console.ReadLine();
                        stateManager.HandleInput(input);
                    }
                    else if( currentState is CombatState combatStateInstance )
                    {
                        Console.WriteLine("Continuing to update combat state...");
                    }
                    else
                    {
                        // Simple way to exit the game loop for other states
                        Console.WriteLine("Press 'Q' to quit or any other key to continue...");
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            isRunning = false;
                        }
                    }
                }
            }
            
            Console.WriteLine("Game has ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
