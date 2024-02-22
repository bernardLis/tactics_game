using UnityEngine.UIElements;

namespace Lis
{
    public class HeroSelectorInfoElement : VisualElement
    {
        const string _ussClassName = "hero-selector-info__";
        const string _ussMain = _ussClassName + "main";
        const string _ussNameContainer = _ussClassName + "name-container";

        const string _ussNameLabel = _ussClassName + "name";


        Hero _hero;

        public HeroSelectorInfoElement(Hero hero)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.HeroSelectorInfoStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);
            AddToClassList("common__text-primary");

            _hero = hero;
            AddNameAndElement();

            Add(new AbilityElement(hero.StartingAbility));

            VisualElement statContainer = new();
            statContainer.style.flexDirection = FlexDirection.Row;
            // statContainer.AddToClassList(_ussStatContainer);
            Add(statContainer);

            foreach (Stat s in hero.GetAllStats())
            {
                StatElement statElement = new(s);
                statContainer.Add(statElement);
            }
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
    }
}