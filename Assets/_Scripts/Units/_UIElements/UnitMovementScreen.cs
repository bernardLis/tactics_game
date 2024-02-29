

namespace Lis.Units
{
    public class UnitMovementScreen : UnitScreen
    {
        readonly UnitMovement _unitMovement;
        public UnitMovementScreen(UnitMovement unit) : base(unit)
        {
            _unitMovement = unit;
        }

        protected override void AddStats()
        {
            base.AddStats();
        
            StatElement speed = new(_unitMovement.Speed);
            StatsContainer.Add(speed);
        }
    }
}
