using Lis.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Hero.Tablets
{
    using Ability;

    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet Advanced")]
    public class TabletAdvanced : Tablet
    {
        [FormerlySerializedAs("FirstElement")] [Header("Advanced Tablet")]
        public Nature FirstNature;

        [FormerlySerializedAs("SecondElement")] public Nature SecondNature;

        public Ability Ability;

        public bool IsMadeOfElements(Nature firstNature, Nature secondNature)
        {
            return (FirstNature == firstNature && SecondNature == secondNature) ||
                   (FirstNature == secondNature && SecondNature == firstNature);
        }
    }
}