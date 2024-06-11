using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using UnityEngine.UIElements;

namespace Lis.HeroSelection._UIElements
{
    public class HeroInfoElement : VisualElement
    {
        private const string _ussClassName = "hero-selector-info__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussNameContainer = _ussClassName + "name-container";

        private const string _ussNameLabel = _ussClassName + "name";

        private readonly Hero _hero;

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

            Add(new AbilityElement(hero.StartingAbility));
            Add(new HorizontalSpacerElement());
            Add(new Label($"Times picked: {hero.TimesPicked}"));
        }

        private void AddNameAndElement()
        {
            Label nameLabel = new($"{_hero.UnitName}");
            nameLabel.AddToClassList(_ussNameLabel);
            Add(nameLabel);

            Add(new HorizontalSpacerElement());

            Add(new NatureElement(_hero.Nature));
        }

        private void AddStatsContainer()
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