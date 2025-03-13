namespace AirelianTactics.Core.Logic
{
    // Core game logic
    public class CombatEngine
    {
        private readonly CombatObject _combatObject;
        
        // Services injected via constructor
        private readonly IStatusService _statusService;
        private readonly ISpellService _spellService;
        private readonly IUnitService _unitService;
        
        public CombatEngine(
            CombatObject combatObject,
            IStatusService statusService,
            ISpellService spellService,
            IUnitService unitService)
        {
            _combatObject = combatObject;
            _statusService = statusService;
            _spellService = spellService;
            _unitService = unitService;
        }
        
        // Core game loop logic
        public void ProcessGameLoop()
        {
            switch (_combatObject.CurrentPhase)
            {
                case Phases.StatusTick:
                    ProcessStatusTick();
                    break;
                case Phases.SlowActionTick:
                    ProcessSlowActionTick();
                    break;
                case Phases.SlowAction:
                    ProcessSlowAction();
                    break;  
                case Phases.CTIncrement:
                    ProcessCTIncrement();
                    break;
                case Phases.ActiveTurn:
                    ProcessActiveTurn();
                    break;      
                case Phases.EndActiveTurn:
                    ProcessEndActiveTurn();
                    break;
                case Phases.Mime:
                    ProcessMime();
                    break;
                case Phases.Reaction:
                    ProcessReaction();
                    break;
                case Phases.Quick:
                    ProcessQuick();
                    break;
                // to do: multiplayer, RL, WA phases
            }
        }
        
        private void ProcessStatusTick()
        {
            // Process status effects for all units
            _statusService.ProcessStatusEffects(_playerManager);

            // Transition to next phase
            _combatObject.CurrentPhase = Phases.SlowActionTick;
        }
        
        private void ProcessSlowActionTick()
        {
            // Process slow actions tick
            _spellService.ProcessSlowActionTick();

            // Transition to next phase
            _combatObject.CurrentPhase = Phases.SlowAction;
        }

        private void ProcessSlowAction()
        {
            _combatObject.CurrentPhase = _combatObject.getPotentialReactionMimeQuickPhase(_combatObject.CurrentPhase);

            if (_combatObject.CurrentPhase == Phases.SlowAction){
                // Process next slow action if it exists
                wasSlowActionProcessed = _spellService.ProcessNextSlowAction();

                if (wasSlowActionProcessed){
                    // return here to see if there are more slow actions or if a flag interupts before the next slow action
                    // ie reaction goes next
                    _combatObject.CurrentPhase = Phases.SlowAction;
                }
                else{
                    // no slow action to process, so CTIncrement
                    // Transition to next phase
                    _combatObject.CurrentPhase = Phases.CTIncrement;
                }
            }
        }

        private void ProcessCTIncrement()
        {
            // Process CT increment for all units
            _unitService.ProcessCTIncrement();

            // Transition to next phase
            _combatObject.CurrentPhase = Phases.ActiveTurn; 
        }

        private void ProcessActiveTurn()
        {
            // can be here due to beginning of an active turn
            // can be here mid turn due to a reaction or mime (maybe a slow action but unlikely)

            // check if a flag interrupts before active turn goes
            _combatObject.CurrentPhase = _combatObject.getPotentialReactionMimeQuickPhase(_combatObject.CurrentPhase);

            if (_combatObject.CurrentPhase == Phases.ActiveTurn){

                bool isActiveTurn = _unitManager.IsActiveTurn();

                if (isActiveTurn){
                    // to do: handle the active turn. Possibly a call to the next state?
                    Console.WriteLine("To do: handle active turn"); 
                }
                else {
                    _combatObject.CurrentPhase = Phases.StatusTick;
                }
                // Process next slow action if it exists
                wasSlowActionProcessed = _spellService.ProcessNextSlowAction();

            }

            
        }       

        // not sure if needed here. can probably handle with flags or after a unit ends their turn
        // private void ProcessEndActiveTurn()
        // {

        // }

        // to do: I think i can handle with flags
        // private void ProcessMime()
        // {

        // }

        // to do: I think i can handle with flags
        // private void ProcessReaction()
        // {

        // }

        // to do: I think i can handle with flags
        // private void ProcessQuick()
        // {   

        // }
    }

}