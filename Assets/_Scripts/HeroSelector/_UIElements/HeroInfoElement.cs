using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using UnityEngine.UIElements;
using Element = Lis.Units.Hero.Ability.Element;

namespace Lis.HeroSelector
{
    public class HeroInfoElement : VisualElement
    {
        const string _ussClassName = "hero-selector-info__";
        const string _ussMain = _ussClassName + "main";
        const string _ussNameContainer = _ussClassName + "name-container";

        const string _ussNameLabel = _ussClassName + "name";

        readonly Hero _hero;

        public HeroInfoElement(Hero hero)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.HeroSelectorInfoStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);
            AddToClassList("common__text-primary");

            _hero = hero;
            AddNameAndElement();
            AddStatsContainer();

            Add(new Element(hero.StartingAbility));
            Add(new Label($"Times picked: {hero.TimesPicked}"));
        }

        void AddNameAndElement()
        {
            VisualElement container = new();
            container.AddToClassList(_ussNameContainer);
            Add(container);

            Label nameLabel = new($"{_hero.EntityName}");
            nameLabel.AddToClassList(_ussNameLabel);
            container.Add(nameLabel);
            container.Add(new ElementalElement(_hero.Element));
        }

        void AddStatsContainer()
        {
            VisualElement statContainer = new();
            statContainer.style.flexDirection = FlexDirection.Row;
            Add(statContainer);
            foreach (Stat s in _hero.GetAllStats())
            {
                StatElement statElement = new(s);
                statContainer.Add(statElement);
            }
        }
    }
}