using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class HeroLeveler : BuildingController, IInteractable
    {
        private int _levelUpsAvailable;
        public new string InteractionPrompt => "Press F To Level Up!";

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

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            _levelUpsAvailable = 0;
            Building = BattleManager.Battle.HeroLeveler;

            IsInteractionAvailable = false;
            HeroManager.Instance.OnHeroInitialized += OnHeroInitialized;
            Initialize();
        }

        private void OnHeroInitialized(Hero hero)
        {
            hero.OnLevelUp += AllowInteraction;
        }

        protected override void AllowInteraction()
        {
            base.AllowInteraction();
            if (HeroManager.Instance.Hero.Level.Value == 2)
            {
                Building.IsUnlocked = true;
                Unlock();
            }

            _levelUpsAvailable++;
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }
    }
}