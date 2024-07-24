using Lis.Arena;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class HeroLeveler : BuildingController, IInteractable
    {
        int _levelUpsAvailable;
        public new string InteractionPrompt => "Level Up";

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

        protected override void OnArenaInitialized()
        {
            base.OnArenaInitialized();
            _levelUpsAvailable = 0;
            Building = FightManager.Campaign.HeroLeveler;

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
            if (HeroManager.Instance.Hero.Level.Value == 2)
            {
                Building.IsUnlocked = true;
                Unlock();
            }

            _levelUpsAvailable++;
        }
    }
}