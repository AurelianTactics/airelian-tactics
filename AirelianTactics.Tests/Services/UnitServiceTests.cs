using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirelianTactics.Services;

namespace AirelianTactics.Tests.Services
{
    [TestClass]
    public class UnitServiceTests
    {
        private UnitService unitService = null!;

        [TestInitialize]
        public void Setup()
        {
            unitService = new UnitService();
        }

        [TestMethod]
        public void GetNextActiveTurnPlayerUnit_MidTurnUnitExists_ReturnsMidTurnUnit()
        {
            // Arrange
            var regularUnit = new PlayerUnit(ct: 120, speed: 10, pa: 5, hp: 100, move: 4, jump: 2, unitId: 2, teamId: 0);
            var midTurnUnit = new PlayerUnit(ct: 80, speed: 8, pa: 4, hp: 90, move: 3, jump: 1, unitId: 1, teamId: 0);
            
            // Set the second unit as mid-turn (even though it has lower CT)
            midTurnUnit.IsMidActiveTurn = true;
            
            unitService.AddUnitWithId(regularUnit, 2);
            unitService.AddUnitWithId(midTurnUnit, 1);
            
            // Act
            var nextUnit = unitService.GetNextActiveTurnPlayerUnit();
            
            // Assert
            Assert.AreEqual(midTurnUnit, nextUnit, "Mid-turn unit should be prioritized over regular units regardless of CT");
            Assert.AreEqual(1, nextUnit.UnitId, "Should return the mid-turn unit with ID 1");
        }

        [TestMethod]
        public void IsTeamDefeated_AllUnitsIncapacitated_ReturnsTrue()
        {
            // Arrange
            var unit1 = new PlayerUnit(ct: 0, speed: 10, pa: 5, hp: 0, move: 4, jump: 2, unitId: 1, teamId: 1);
            var unit2 = new PlayerUnit(ct: 0, speed: 8, pa: 4, hp: 0, move: 3, jump: 1, unitId: 2, teamId: 1);
            
            // Incapacitate both units (AlterHP sets IsIncapacitated when HP <= 0)
            unit1.AlterHP(-100);
            unit2.AlterHP(-100);
            
            unitService.AddUnitWithId(unit1, 1);
            unitService.AddUnitWithId(unit2, 2);
            
            // Act
            bool isDefeated = unitService.IsTeamDefeated(1);
            
            // Assert
            Assert.IsTrue(isDefeated, "Team should be defeated when all units are incapacitated");
        }

        [TestMethod]
        public void PlayerUnit_AlterHP_ToZero_SetsIncapacitatedAndClampsHP()
        {
            // Arrange
            var unit = new PlayerUnit(ct: 50, speed: 10, pa: 5, hp: 100, move: 4, jump: 2, unitId: 1, teamId: 0);
            
            // Act - deal enough damage to drop HP below 0
            unit.AlterHP(-120);
            
            // Assert
            Assert.IsTrue(unit.IsIncapacitated, "Unit should be incapacitated when HP drops to 0 or below");
            Assert.AreEqual(0, unit.StatTotalHP, "HP should be clamped to 0, not negative");
        }

        [TestMethod]
        public void PlayerUnit_IsEligibleForActiveTurn_HighCTNotIncapacitated_ReturnsTrue()
        {
            // Arrange
            var unit = new PlayerUnit(ct: 100, speed: 10, pa: 5, hp: 100, move: 4, jump: 2, unitId: 1, teamId: 0);
            
            // Act
            bool isEligible = unit.IsEligibleForActiveTurn();
            
            // Assert
            Assert.IsTrue(isEligible, "Unit with CT >= 100 and not incapacitated should be eligible for active turn");
        }

        [TestMethod]
        public void PlayerUnit_IsEligibleForActiveTurn_Incapacitated_ReturnsFalse()
        {
            // Arrange
            var unit = new PlayerUnit(ct: 150, speed: 10, pa: 5, hp: 100, move: 4, jump: 2, unitId: 1, teamId: 0);
            unit.AlterHP(-200); // Incapacitate the unit
            
            // Act
            bool isEligible = unit.IsEligibleForActiveTurn();
            
            // Assert
            Assert.IsFalse(isEligible, "Incapacitated unit should not be eligible for active turn regardless of CT");
        }
    }
} 