using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Troops Limit")]
    public class BuildingTroopsLimit : Building
    {
        public override void Unlocked()
        {
            base.Unlocked();
            BattleManager.Instance.HeroController.Hero.TroopsLimit.ApplyChange(2);
        }
    }
}