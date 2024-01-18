

namespace Lis
{
    public class EntityMovementScreen : EntityScreen
    {
        readonly EntityMovement _entityMovement;
        public EntityMovementScreen(EntityMovement entity) : base(entity)
        {
            _entityMovement = entity;
        }

        protected override void AddStats()
        {
            base.AddStats();
        
            StatElement speed = new(_entityMovement.Speed);
            _statsContainer.Add(speed);
        }
    }
}
