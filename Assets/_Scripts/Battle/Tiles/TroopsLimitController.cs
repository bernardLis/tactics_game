
namespace Lis.Battle.Tiles
{
    public class TroopsLimitController : ProductionController
    {
        protected override void OnTileUnlocked(Controller tile)
        {
            base.OnTileUnlocked(tile);

            BattleManager.Hero.TroopsLimit.ApplyChange(Upgrade.GetValue());
        }
    }
}