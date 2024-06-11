using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Hammer")]
    public class Hammer : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<BreakableVaseManager>().BreakAllVases();
        }
    }
}