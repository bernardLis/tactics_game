using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Attack
{
    public class AttackElement : ElementWithTooltip
    {
        private const string _ussCommonTextPrimary = "common__text-primary";
        private const string _ussCommonButtonBasic = "common__button-basic";

        private const string _ussClassName = "attack-element__";
        private const string _ussMain = _ussClassName + "main";

        private readonly Attack _attack;

        public AttackElement(Attack attack)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.AttackElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussCommonButtonBasic);

            _attack = attack;

            style.backgroundImage = new(attack.Icon);
        }

        protected override void DisplayTooltip()
        {
            AttackTooltipElement tt = new(_attack);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}