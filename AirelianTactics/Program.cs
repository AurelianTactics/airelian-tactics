using System;

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
                Console.WriteLine($"---State changed from {(previousState != null ? previousState.GetType().Name : "null")} to {newState.GetType().Name}");
            };
            
            // Create and register all game states
            GameInitState initState = new GameInitState(stateManager);
            MapSetupState mapSetupState = new MapSetupState(stateManager);
            CombatState combatState = new CombatState(stateManager);
            GameEndState endState = new GameEndState(stateManager);
            
            stateManager.RegisterState(initState);
            stateManager.RegisterState(mapSetupState);
            stateManager.RegisterState(combatState);
            stateManager.RegisterState(endState);
            
            // Define the state flow
            stateManager.DefineStateFlow<GameInitState, MapSetupState>();
            stateManager.DefineStateFlow<MapSetupState, CombatState>();
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
                    // Simple way to exit the game loop
                    Console.WriteLine("Press 'Q' to quit or any other key to continue...");
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        isRunning = false;
                    }
                }
            }
            
            Console.WriteLine("Game has ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
