using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Turret")]
    public class Turret : EntityFight
    {

        [Header("Turret")]
        public int UpgradeCost;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);

            UpgradeCost = 200 * Level.Value;
        }

        public void PurchaseUpgrade()
        {
            LevelUp();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            UpgradeCost = 200 * Level.Value;
        }

        public void PurchaseSpecialUpgrade()
        {
            //HERE: turret special upgrade
        }
    }
}


