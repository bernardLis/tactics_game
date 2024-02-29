using UnityEngine;

namespace Lis.Units.Hero.Tablets
{
    using Ability;
    using Element = Core.Element;

    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet Advanced")]
    public class TabletAdvanced : Tablet
    {
        [Header("Advanced Tablet")]
        public Element FirstElement;

        public Element SecondElement;

        public Ability Ability;

        public bool IsMadeOfElements(Element firstElement, Element secondElement)
        {
            return (FirstElement == firstElement && SecondElement == secondElement) ||
                   (FirstElement == secondElement && SecondElement == firstElement);
        }
    }
}