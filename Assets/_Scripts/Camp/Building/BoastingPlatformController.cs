using Lis.Units.Hero;

namespace Lis.Camp.Building
{
    public class BoastingPlatformController : BuildingController
    {
        BoastingPlatform _boastingPlatform;

        protected override void Initialize()
        {
            Building = GameManager.Campaign.BoastingPlatform;
            _boastingPlatform = (BoastingPlatform)Building;
            base.Initialize();
        }

        public override bool Interact(Interactor interactor)
        {
            BoastingScreen screen = new();
            screen.InitializeBoastingPlatform(_boastingPlatform);

            return true;
        }

    }
}