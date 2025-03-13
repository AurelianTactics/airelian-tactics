using System;
using System.Threading;

/// <summary>
/// GameRunner is the entry point for the game.
/// It initializes the game and starts the game loop.
/// </summary>
public class GameRunner
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public static void Main()
    {
        Console.WriteLine("Starting game...");
        
        // Create and run an automated combat demo
        RunAutomatedCombatDemo();
        
        Console.WriteLine("Game completed.");
    }
    
    /// <summary>
    /// Runs an automated combat demo that shows state transitions without user input.
    /// </summary>
    private static void RunAutomatedCombatDemo()
    {
        // Create the combat data with some initial setup
        CombatData combatData = new CombatData();
        
        // Add some units to the combat
        combatData.PlayerUnits.Add(new Unit { Name = "Hero", Health = 100 });
        combatData.EnemyUnits.Add(new Unit { Name = "Enemy", Health = 50 });
        
        // Create the combat state manager
        CombatStateManager combatManager = new CombatStateManager(combatData);
        
        // Initialize the combat (this will set the initial state to InitCombat)
        combatManager.Initialize();
        
        // Run the automated combat loop
        bool isCombatComplete = false;
        
        while (!isCombatComplete)
        {
            // Update the combat state manager (this will process state transitions)
            combatManager.Update();
            
            // Simulate game logic based on the current state
            SimulateCombatLogic(combatData);
            
            // Check if combat is complete (victory or defeat)
            IState currentState = combatManager.GetCurrentState();
            if (currentState is VictoryState || currentState is DefeatState)
            {
                isCombatComplete = true;
            }
            
            // Simulate a frame delay
            Thread.Sleep(500); // Half-second delay to see the state changes
        }
    }
    
    /// <summary>
    /// Simulates combat logic by setting appropriate flags in the combat data.
    /// This would normally be driven by user input or AI, but for this demo,
    /// we're automating it to show the state transitions.
    /// </summary>
    private static void SimulateCombatLogic(CombatData combatData)
    {
        // If we're in the player input state and no action is selected yet
        if (!combatData.IsActionSelected && combatData.IsPlayerTurn)
        {
            // Simulate player selecting an attack action
            Console.WriteLine("Player selects Attack action");
            combatData.SelectedAction = new Action { Name = "Attack", Damage = 20 };
            combatData.TargetUnit = combatData.EnemyUnits[0];
            combatData.IsActionSelected = true;
        }
        // If we're in the enemy turn state and no action is selected yet
        else if (!combatData.IsActionSelected && !combatData.IsPlayerTurn)
        {
            // Simulate enemy selecting an attack action
            Console.WriteLine("Enemy selects Attack action");
            combatData.SelectedAction = new Action { Name = "Attack", Damage = 10 };
            combatData.TargetUnit = combatData.PlayerUnits[0];
            combatData.IsActionSelected = true;
        }
        // If an action is selected but not executed
        else if (combatData.IsActionSelected && !combatData.IsActionExecuted)
        {
            // Simulate executing the action
            Console.WriteLine($"{(combatData.IsPlayerTurn ? "Player" : "Enemy")} executes {combatData.SelectedAction.Name} on {combatData.TargetUnit.Name}");
            combatData.SelectedAction.Execute(combatData.TargetUnit);
            combatData.IsActionExecuted = true;
        }
        // If action is executed but effects aren't applied
        else if (combatData.IsActionExecuted && !combatData.AreEffectsApplied)
        {
            // Simulate applying effects
            Console.WriteLine($"{combatData.TargetUnit.Name} takes {combatData.SelectedAction.Damage} damage");
            combatData.TargetUnit.Health -= combatData.SelectedAction.Damage;
            Console.WriteLine($"{combatData.TargetUnit.Name}'s health is now {combatData.TargetUnit.Health}");
            combatData.AreEffectsApplied = true;
        }
        // If effects are applied but victory check isn't complete
        else if (combatData.AreEffectsApplied && !combatData.IsVictoryCheckComplete)
        {
            // Simulate checking for victory/defeat
            if (combatData.EnemyUnits[0].Health <= 0)
            {
                Console.WriteLine("Enemy has been defeated!");
                combatData.IsPlayerVictorious = true;
            }
            else if (combatData.PlayerUnits[0].Health <= 0)
            {
                Console.WriteLine("Player has been defeated!");
                combatData.IsPlayerDefeated = true;
            }
            combatData.IsVictoryCheckComplete = true;
        }
        // If victory check is complete but turn isn't ended (and we're not in victory/defeat state)
        else if (combatData.IsVictoryCheckComplete && !combatData.IsTurnEnded && 
                !combatData.IsPlayerVictorious && !combatData.IsPlayerDefeated)
        {
            // Simulate ending the turn
            Console.WriteLine("Turn ended");
            combatData.IsTurnEnded = true;
        }
    }
}

// Extend the Unit class with more properties
public partial class Unit
{
    public string Name { get; set; }
    public int Health { get; set; }
}

// Extend the Action class with more properties
public partial class Action
{
    public string Name { get; set; }
    public int Damage { get; set; }
    
    public void Execute(Unit target)
    {
        // The actual damage calculation would happen here
        // For this demo, we'll just use the damage value directly
    }
} 