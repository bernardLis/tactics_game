


using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis
{
    public class CreatureAbilityTooltipElement : VisualElement
    {
        const string _ussClassName = "creature-ability-tooltip-element__";
        const string _ussMain = _ussClassName + "main";

        readonly GameManager _gameManager;

        readonly CreatureAbility _ability;

        public CreatureAbilityTooltipElement(CreatureAbility ability)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureAbilityTooltipStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _ability = ability;

            AddToClassList(_ussMain);

            Label name = new($"<b>{Helpers.ParseScriptableObjectName(_ability.name)}</b>");
            Add(name);

            Label description = new(_ability.Description);
            description.style.whiteSpace = WhiteSpace.Normal;
            Add(description);

            Label cooldown = new($"Cooldown: {_ability.Cooldown}");
            Add(cooldown);
        }
    }
}
