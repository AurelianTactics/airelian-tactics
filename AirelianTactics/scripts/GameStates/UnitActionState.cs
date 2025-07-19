using System;
using System.Collections.Generic;
using AirelianTactics.Services;

/// <summary>
/// Handles user command selection, targeting, and confirmation during unit turns.
/// Triggered when CombatState returns a GameTimeObject for ActiveTurn or MidActiveTurn.
/// </summary>
public class UnitActionState : State
{
    private int actorUnitId;
    private PlayerUnit currentUnit;
    private UnitService unitService;
    private SpellService spellService;
    private StatusService statusService;
    private Board board;
    private GameTimeManager gameTimeManager;
    
    // State tracking
    private enum ActionPhase
    {
        SelectingCommand,
        Targeting,
        Confirming
    }
    
    private ActionPhase currentPhase = ActionPhase.SelectingCommand;
    private string selectedCommand = "";
    private object targetingData = null;
    private bool waitingForUserInput = false;
    
    /// <summary>
    /// Public property to check if the state is waiting for user input
    /// </summary>
    public bool IsWaitingForInput => waitingForUserInput;

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public UnitActionState(StateManager stateManager) : base(stateManager)
    {
        // Services will be initialized from GameContext in Enter()
    }

    /// <summary>
    /// Set the actor unit ID for this turn. Called by CombatState before transitioning.
    /// </summary>
    /// <param name="unitId">The ID of the unit taking their turn</param>
    public void SetActorUnit(int unitId)
    {
        actorUnitId = unitId;
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        
        // Get shared services from StateManager
        unitService = stateManager.UnitService;
        spellService = stateManager.SpellService;
        statusService = stateManager.StatusService;
        board = stateManager.Board;
        gameTimeManager = stateManager.GameTimeManager;  // Use persistent GameTimeManager
        
        Console.WriteLine("Entering Unit Action State");
        
        // Get the current unit using the actorUnitId
        if (unitService.unitDict.TryGetValue(actorUnitId, out currentUnit) == false)
        {
            currentUnit = null;
        }
        
        if (currentUnit == null)
        {
            Console.WriteLine($"Unit {actorUnitId} not found");
            CompleteState();
            return;
        }

        Console.WriteLine($"Unit {currentUnit.UnitId} (Team {currentUnit.TeamId}) is taking their turn");
        
        // Start with command selection
        currentPhase = ActionPhase.SelectingCommand;
        ShowCommandMenu();
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // In a real implementation, this would check for user input
        // For now, we'll simulate the flow
        if (!waitingForUserInput)
        {
            switch (currentPhase)
            {
                case ActionPhase.SelectingCommand:
                    HandleCommandSelection();
                    break;
                case ActionPhase.Targeting:
                    HandleTargeting();
                    break;
                case ActionPhase.Confirming:
                    HandleConfirmation();
                    break;
            }
        }
    }

    /// <summary>
    /// Show the command menu to the user
    /// </summary>
    private void ShowCommandMenu()
    {
        Console.WriteLine("=== UNIT ACTION MENU ===");
        // Add move and act when Wait is implemented
        // Console.WriteLine("1. Move");
        // Console.WriteLine("2. Act");
        Console.WriteLine("3. Wait");
        Console.WriteLine("Select your command:");
        
        waitingForUserInput = true;
        // Input will be handled by the HandleInput method when user provides it
    }

    /// <summary>
    /// Handle command selection phase
    /// </summary>
    private void HandleCommandSelection()
    {
        // This would be called when user input is received
        switch (selectedCommand)
        {
            // case "1":
            //     selectedCommand = "Move";
            //     currentPhase = ActionPhase.Targeting;
            //     ShowMoveTargeting();
            //     break;
            // case "2":
            //     selectedCommand = "Act";
            //     currentPhase = ActionPhase.Targeting;
            //     ShowActTargeting();
            //     break;
            case "3":
                selectedCommand = "Wait";
                currentPhase = ActionPhase.Confirming;
                ShowConfirmation();
                break;
            default:
                Console.WriteLine("Invalid selection. Please try again.");
                ShowCommandMenu();
                break;
        }
    }

    /// <summary>
    /// Show move targeting interface with actual valid tiles
    /// </summary>
    private void ShowMoveTargeting()
    {
        Console.WriteLine("=== MOVE TARGETING ===");
        Console.WriteLine("Select a tile to move to:");
        
        // Get the current unit's position
        Point? currentPositionNullable = board.GetUnitPosition(currentUnit.UnitId);
        if (!currentPositionNullable.HasValue)
        {
            Console.WriteLine("Error: Could not find unit position");
            currentPhase = ActionPhase.SelectingCommand;
            ShowCommandMenu();
            return;
        }
        
        Point currentPosition = currentPositionNullable.Value;
        
        // Get valid move tiles - using a default move range of 3 since StatTotalMove is private
        // In a real implementation, you'd need a public getter for move range
        List<Tile> validMoveTiles = board.GetValidMoveTiles(currentPosition, 3);
        
        if (validMoveTiles.Count == 0)
        {
            Console.WriteLine("No valid move tiles available");
            currentPhase = ActionPhase.SelectingCommand;
            ShowCommandMenu();
            return;
        }
        
        // Show up to 9 tiles with number keys
        Console.WriteLine($"Current position: {currentPosition}");
        Console.WriteLine("Available move tiles:");
        
        int displayCount = Math.Min(validMoveTiles.Count, 9);
        for (int i = 0; i < displayCount; i++)
        {
            Tile tile = validMoveTiles[i];
            Console.WriteLine($"{i + 1}. ({tile.pos.x},{tile.pos.y}) - Height: {tile.height}");
        }
        
        if (validMoveTiles.Count > 9)
        {
            Console.WriteLine($"... and {validMoveTiles.Count - 9} more tiles (not shown)");
        }
        
        Console.WriteLine("0. Cancel");
        Console.WriteLine("Enter number (0-9):");
        
        // Store the valid tiles for later reference
        targetingData = validMoveTiles;
        waitingForUserInput = true;
        // Input will be handled by the HandleInput method when user provides it
    }

    /// <summary>
    /// Show act targeting interface
    /// </summary>
    private void ShowActTargeting()
    {
        Console.WriteLine("=== ACT TARGETING ===");
        Console.WriteLine("Select an ability:");
        Console.WriteLine("1. Attack");
        
        waitingForUserInput = true;
        // Input will be handled by the HandleInput method when user provides it
    }

    /// <summary>
    /// Handle targeting phase
    /// </summary>
    private void HandleTargeting()
    {
        switch (selectedCommand)
        {
            case "Move":
                HandleMoveTargeting();
                break;
            case "Act":
                HandleActTargeting();
                break;
        }
    }

    /// <summary>
    /// Handle move targeting input
    /// </summary>
    private void HandleMoveTargeting()
    {
        // Get the user's selection
        string input = selectedCommand;
        
        // Parse the input as a number
        if (int.TryParse(input, out int selection))
        {
            if (selection == 0)
            {
                // User cancelled - go back to command menu
                currentPhase = ActionPhase.SelectingCommand;
                ShowCommandMenu();
                return;
            }
            
            // Get the valid tiles list from targetingData
            List<Tile> validTiles = targetingData as List<Tile>;
            
            if (validTiles != null && selection >= 1 && selection <= Math.Min(validTiles.Count, 9))
            {
                // Valid selection - get the selected tile
                Tile selectedTile = validTiles[selection - 1];
                targetingData = selectedTile.pos;
                
                Console.WriteLine($"Selected tile: ({selectedTile.pos.x},{selectedTile.pos.y})");
                currentPhase = ActionPhase.Confirming;
                ShowConfirmation();
            }
            else
            {
                Console.WriteLine("Invalid selection. Please try again.");
                ShowMoveTargeting();
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            ShowMoveTargeting();
        }
    }

    /// <summary>
    /// Handle act targeting input
    /// </summary>
    private void HandleActTargeting()
    {
        string abilityChoice = targetingData.ToString();
        
        if (abilityChoice == "1") // Attack
        {
            Console.WriteLine("Select target tile for attack:");
            Console.WriteLine("Available targets: (2,1), (1,2)");
            
            waitingForUserInput = true;
            // Input will be handled by the HandleInput method when user provides it
            
            // After getting target, create act targeting data
            targetingData = new ActTargetingData
            {
                AbilityName = "Attack",
                TargetPoint = new Point(2, 1),
                SpellName = new SpellName() // Default attack spell
            };
            
            currentPhase = ActionPhase.Confirming;
            ShowConfirmation();
        }
        else if (abilityChoice == "2") // Heal
        {
            Console.WriteLine("Select target for heal:");
            // Similar flow for heal
            targetingData = new ActTargetingData
            {
                AbilityName = "Heal",
                TargetPoint = new Point(1, 1),
                SpellName = new SpellName() // Default heal spell
            };
            
            currentPhase = ActionPhase.Confirming;
            ShowConfirmation();
        }
        else
        {
            Console.WriteLine("Invalid ability selection. Please try again.");
            ShowActTargeting();
        }
    }

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    private void ShowConfirmation()
    {
        Console.WriteLine("=== CONFIRMATION ===");
        Console.WriteLine($"Command: {selectedCommand}");
        
        switch (selectedCommand)
        {
            case "Move":
                Point moveTarget = (Point)targetingData;
                Console.WriteLine($"Move to: ({moveTarget.x}, {moveTarget.y})");
                break;
            case "Act":
                ActTargetingData actData = (ActTargetingData)targetingData;
                Console.WriteLine($"Ability: {actData.AbilityName}");
                Console.WriteLine($"Target: ({actData.TargetPoint.x}, {actData.TargetPoint.y})");
                break;
            case "Wait":
                Console.WriteLine("Wait and end turn");
                break;
        }
        
        Console.WriteLine("Confirm? (Y/N)");
        
        waitingForUserInput = true;
        // Input will be handled by the HandleInput method when user provides it
    }

    /// <summary>
    /// Handle confirmation phase
    /// </summary>
    private void HandleConfirmation()
    {
        string confirmation = targetingData?.ToString() ?? "Y";
        
        if (confirmation.ToUpper() == "Y")
        {
            CreateAndQueueGameTimeObject();
            CompleteState();
        }
        else
        {
            // Go back to command selection
            currentPhase = ActionPhase.SelectingCommand;
            selectedCommand = "";
            targetingData = null;
            ShowCommandMenu();
        }
    }

    /// <summary>
    /// Create a GameTimeObject based on the selected command and queue it
    /// </summary>
    private void CreateAndQueueGameTimeObject()
    {
        GameTimeObject actionGTO = null;
        
        switch (selectedCommand)
        {
            case "Wait":
                actionGTO = new GameTimeObject(
                    phase: Phases.FasterThanFastAction,
                    actorUnitId: currentUnit.UnitId,
                    spellName: CreateWaitSpell()
                );
                break;
                
            case "Move":
                Point moveTarget = (Point)targetingData;
                actionGTO = new GameTimeObject(
                    phase: Phases.FasterThanFastAction,
                    actorUnitId: currentUnit.UnitId,
                    targetUnitId: null, // Move doesn't target a unit
                    spellName: CreateMoveSpell(moveTarget)
                );
                break;
                
            case "Act":
                ActTargetingData actData = (ActTargetingData)targetingData;
                actionGTO = new GameTimeObject(
                    phase: Phases.FasterThanFastAction,
                    actorUnitId: currentUnit.UnitId,
                    targetUnitId: board.GetUnitAtPosition(actData.TargetPoint),
                    spellName: actData.SpellName
                );
                break;
        }
        
        if (actionGTO != null)
        {
            // Add to the GameTimeManager queue
            gameTimeManager.AddGameTimeObject(actionGTO);
            Console.WriteLine($"Created and queued GameTimeObject for {selectedCommand} action");
        }
    }

    /// <summary>
    /// Create a spell for the Wait command
    /// </summary>
    private SpellName CreateWaitSpell()
    {
        SpellName waitSpell = new SpellName();
        waitSpell.AbilityName = "Wait";

        return waitSpell;
    }

    /// <summary>
    /// Create a spell for the Move command
    /// </summary>
    private SpellName CreateMoveSpell(Point targetPosition)
    {
        SpellName moveSpell = new SpellName();
        moveSpell.AbilityName = "Move";
        moveSpell.TargetPoint = targetPosition;

        return moveSpell;
    }



    /// <summary>
    /// Handle user input received from the input system
    /// </summary>
    public override void HandleInput(string input)
    {
        if (waitingForUserInput)
        {
            // Process the input based on the current phase
            switch (currentPhase)
            {
                case ActionPhase.SelectingCommand:
                    selectedCommand = input;
                    waitingForUserInput = false;
                    break;
                case ActionPhase.Targeting:
                    targetingData = input;
                    waitingForUserInput = false;
                    break;
                case ActionPhase.Confirming:
                    targetingData = input;
                    waitingForUserInput = false;
                    break;
            }
        }
    }



    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Unit Action State");
        base.Exit();
    }
}

/// <summary>
/// Data structure for Act command targeting
/// </summary>
public class ActTargetingData
{
    public string AbilityName { get; set; }
    public Point TargetPoint { get; set; }
    public SpellName SpellName { get; set; }
} 