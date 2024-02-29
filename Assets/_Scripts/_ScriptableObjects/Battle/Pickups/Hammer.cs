
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Hammer")]

    public class Hammer : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<BattleVaseManager>().BreakAllVases();
        }
    }
}
