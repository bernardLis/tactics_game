using UnityEngine.UIElements;

namespace Lis.Units.Attack
{
    public class AttackElement : VisualElement
    {
        public AttackElement(Attack attack)
        {
            style.width = 50;
            style.height = 50;

            style.backgroundImage = new(attack.Icon);
        }
    }
}