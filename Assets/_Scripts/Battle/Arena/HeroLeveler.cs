using System.Collections;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class HeroLeveler : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Level Up!";

        int _levelUpsAvailable;

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            _levelUpsAvailable = 0;
            Building = BattleManager.Battle.HeroLeveler;

            IsInteractionAvailable = false;
            HeroManager.Instance.OnHeroInitialized += OnHeroInitialized;
            Initialize();
        }

        void OnHeroInitialized(Hero hero)
        {
            hero.OnLevelUp += AllowInteraction;
        }

        protected override void AllowInteraction()
        {
            base.AllowInteraction();
            _levelUpsAvailable++;
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public override bool Interact(Interactor interactor)
        {
            if (!IsInteractionAvailable)
            {
                Debug.Log("No level up available!");
                return false;
            }

            LevelUpRewardScreen levelUpScreen = new();
            levelUpScreen.Initialize();

            _levelUpsAvailable--;
            if (_levelUpsAvailable == 0)
                ForbidInteraction();

            return true;
        }
    }
}