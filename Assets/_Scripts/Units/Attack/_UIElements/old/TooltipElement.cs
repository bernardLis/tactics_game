using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Creature.Ability
{
    public class TooltipElement : VisualElement
    {
        /*
        const string _ussClassName = "creature-ability-tooltip-element__";
        const string _ussMain = _ussClassName + "main";


        public TooltipElement(Ability ability)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CreatureAbilityTooltipStyles);
            if (ss != null)
                styleSheets.Add(ss);

            AddToClassList(_ussMain);

            Label nameLabel = new($"<b>{Helpers.ParseScriptableObjectName(ability.name)}</b>");
            Add(nameLabel);

            Label description = new(ability.Description);
            description.style.whiteSpace = WhiteSpace.Normal;
            Add(description);

            Label cooldown = new($"Cooldown: {ability.Attack.Cooldown}");
            Add(cooldown);
        }
        */
    }
}