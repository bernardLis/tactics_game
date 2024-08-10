using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Hammer")]
    public class Hammer : Pickup
    {
        public override void Collected(Hero hero)
        {
            FightManager.Instance.GetComponent<BreakableVaseManager>().BreakAllVases();
        }
    }
}