using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Friend Ball Production")]
    public class BuildingFriendBallProduction : Building
    {
        public override void Unlocked()
        {
            base.Unlocked();
            BattleManager.Instance.HeroController.Hero.TroopsLimit.ApplyChange(2);
        }
    }
}