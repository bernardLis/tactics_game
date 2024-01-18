

using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet Advanced")]
    public class TabletAdvanced : Tablet
    {
        [Header("Advanced Tablet")]
        public Element FirstElement;
        public Element SecondElement;

        public Ability Ability;

        public override void Initialize(Hero hero)
        {
            base.Initialize(hero);
        }

        public override void LevelUp()
        {
            base.LevelUp();
        }

        public bool IsMadeOfElements(Element firstElement, Element secondElement)
        {
            return (FirstElement == firstElement && SecondElement == secondElement) ||
                   (FirstElement == secondElement && SecondElement == firstElement);
        }
    }
}
