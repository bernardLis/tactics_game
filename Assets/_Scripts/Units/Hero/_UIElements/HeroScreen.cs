using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero
{
    public class HeroScreen : UnitScreen
    {
        const string _ussRowContainer = USSClassName + "row-container";
        const string _ussSlot = USSClassName + "slot";

        readonly Hero _hero;

        public HeroScreen(Hero hero) : base(hero)
        {
            _hero = hero;
        }

        public override void Initialize()
        {
            base.Initialize();
            HandleTablets();
        }

        protected override void HandleStats()
        {
            foreach (Stat s in _hero.GetAllStats())
            {
                StatElement statElement = new(s);
                StatsContainer.Add(statElement);
            }
        }

        protected override void AddOtherBasicInfo()
        {
        }

        protected override void AddBattleData()
        {
        }

        protected override void AddAttacks()
        {
            VisualElement container = new();
            container.AddToClassList(_ussRowContainer);
            OtherContainer.Add(container);

            foreach (Ability.Ability ability in _hero.GetAllAbilities())
            {
                AbilityElement icon = new(ability, true);
                container.Add(icon);
            }

            List<VisualElement> slots = new();
            for (int i = 0; i < 5 - _hero.GetAllAbilities().Count; i++)
            {
                VisualElement slot = new();
                slot.AddToClassList(_ussSlot);
                container.Add(slot);
                slots.Add(slot);
            }

            if (_hero.AdvancedTablet == null)
                slots[^1].style.backgroundColor = Color.yellow;

            _hero.OnAbilityAdded += (a) =>
            {
                if (slots.Count > 0)
                {
                    container.Remove(slots[0]);
                    slots.RemoveAt(0);
                }

                AbilityElement icon = new(a, true);
                container.Insert(_hero.Abilities.Count - 1, icon);

                if (a.Nature is NatureAdvanced && slots.Count > 0)
                    slots[^1].style.backgroundColor = Color.white;
            };
        }

        void HandleTablets()
        {
            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            OtherContainer.Add(spacer);

            VisualElement container = new();
            container.AddToClassList(_ussRowContainer);
            OtherContainer.Add(container);

            foreach (Tablet t in _hero.Tablets)
            {
                TabletElement tabletElement = new(t, true);
                container.Add(tabletElement);
            }

            if (_hero.AdvancedTablet != null)
            {
                container.Add(new TabletElement(_hero.AdvancedTablet, true));
                return;
            }

            VisualElement slot = new();
            slot.AddToClassList(_ussSlot);
            slot.style.backgroundColor = Color.yellow;
            container.Add(slot);
            _hero.OnTabletAdvancedAdded += AdvancedTabletAdded;
            return;

            void AdvancedTabletAdded(TabletAdvanced tabletAdvanced)
            {
                container.Remove(slot);
                container.Add(new TabletElement(tabletAdvanced, true));
                _hero.OnTabletAdvancedAdded -= AdvancedTabletAdded;
            }
        }
    }
}