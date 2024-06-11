using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Tablets
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet Advanced")]
    public class TabletAdvanced : Tablet
    {
        [Header("Advanced Tablet")]
        public Ability.Ability Ability;

        public bool IsMadeOfNatures(Nature firstNature, Nature secondNature)
        {
            NatureAdvanced nature = (NatureAdvanced)Nature;
            return (nature.FirstNature == firstNature && nature.SecondNature == secondNature) ||
                   (nature.FirstNature == secondNature && nature.SecondNature == firstNature);
        }
    }
}