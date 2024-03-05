
namespace Lis.Battle.Tiles
{
    public class TileTroopsLimitController : TileProductionController
    {
        protected override void OnTileUnlocked(TileController tile)
        {
            base.OnTileUnlocked(tile);

            BattleManager.Hero.TroopsLimit.ApplyChange(Upgrade.GetValue());
        }
    }
}