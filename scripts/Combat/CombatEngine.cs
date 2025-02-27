namespace AirelianTactics.Core.Logic
{
    // Core game logic
    public class CombatEngine
    {
        private readonly CombatObject _combatObject;
        
        // Services injected via constructor
        private readonly IStatusService _statusService;
        private readonly ISpellService _spellService;
        private readonly IPlayerManager _playerManager;
        
        public CombatEngine(
            CombatObject combatObject,
            IStatusService statusService,
            ISpellService spellService,
            IPlayerManager playerManager)
        {
            _combatObject = combatObject;
            _statusService = statusService;
            _spellService = spellService;
            _playerManager = playerManager;
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

        // private void ProcessCTIncrement()
        // {
        //     // Process CT increment for all units
        //     _spellService.ProcessCTIncrement();

        //     // Transition to next phase
        //     _combatObject.CurrentPhase = Phases.ActiveTurn; 
        // }

        // private void ProcessActiveTurn()
        // {
        //     // Process active turn for all units
        //     _spellService.ProcessActiveTurn();  

        //     // Transition to next phase
        //     _combatObject.CurrentPhase = Phases.EndActiveTurn;
        // }       

        // private void ProcessEndActiveTurn()
        // {
        //     // Process end active turn for all units
        //     _spellService.ProcessEndActiveTurn();   

        //     // Transition to next phase
        //     _combatObject.CurrentPhase = Phases.Mime;
        // }

        // private void ProcessMime()
        // {
        //     // Process mime for all units
        //     _spellService.ProcessMime();

        //     // Transition to next phase
        //     _combatObject.CurrentPhase = Phases.Reaction;
        // }

        // private void ProcessReaction()
        // {
        //     // quick and slow actions can boot out of reaction so can't stay here until all are done
        //     // Process reaction for all units
        //     _spellService.ProcessReaction();

        //     // Transition to next phase
        //     _combatObject.CurrentPhase = Phases.Quick;
        // }

        // private void ProcessQuick()
        // {   
        //     // Process quick for all units
        //     _spellService.ProcessQuick();

        //     // Transition to next phase
        //     //_combatObject.CurrentPhase = Phases.Standby;    
        // }
    }
    
    // Service interfaces
    public interface IStatusService
    {
        void ProcessStatusEffects(PlayerUnit unit);
        bool IsTurnActable(int unitId);
    }
    
    public interface ISpellService
    {
        void ProcessSlowActions();
        SpellSlow GetNextSlowAction();
    }
}