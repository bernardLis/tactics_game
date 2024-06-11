using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class FightSelectScreen : FullScreenElement
    {
        private readonly FightManager _fightManager;

        private readonly List<FightOptionElement> _fightOptionElements = new();
        private MyButton _chooseRandomButton;

        public FightSelectScreen()
        {
            _fightManager = FightManager.Instance;
            Content.style.alignItems = Align.Stretch;
            Content.style.flexWrap = Wrap.Wrap;
        }

        public void Initialize()
        {
            VisualElement container = new();
            container.style.height = Length.Percent(70);
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.SpaceAround;
            Content.Add(container);

            foreach (FightOption option in _fightManager.CurrentFight.Options)
            {
                FightOptionElement el = new(option);
                _fightOptionElements.Add(el);
                container.Add(el);

                if (_fightManager.CurrentFight.ChosenOption != null)
                {
                    el.DisableSelf();
                    continue;
                }

                option.OnChosen += _ => DisableOptions();
            }

            SetTitle("Choose next fight:");
            AddRandomButton();
            AddContinueButton();
        }

        private void AddRandomButton()
        {
            if (_fightManager.CurrentFight.ChosenOption != null) return;
            _chooseRandomButton = new($"Random +{FightManager.FightNumber * 10} gold", USSCommonButton, ChooseRandom);
            UtilityContainer.Add(_chooseRandomButton);
        }

        private void ChooseRandom()
        {
            _fightManager.CurrentFight.ChooseRandomOption();
        }

        private void DisableOptions()
        {
            _chooseRandomButton.SetEnabled(false);
            foreach (FightOptionElement el in _fightOptionElements) el.DisableSelf();
        }
    }
}