using UnityEngine;

namespace Lis.Battle.Arena
{
    public class HeroLeveler : ArenaInteractable, IInteractable
    {
        BattleManager _battleManager;

        public new string InteractionPrompt => "Press F To Level Up!";

        int _levelUpsAvailable;

        protected override void Start()
        {
            base.Start();
            IsInteractionAvailable = false;

            _battleManager = BattleManager.Instance;
            _battleManager.GetComponent<BattleInitializer>().OnBattleInitialized += Initialize;
        }

        void Initialize()
        {
            _battleManager.GetComponent<HeroManager>().Hero.OnLevelUp += () =>
            {
                _levelUpsAvailable++;
                InteractionAvailableEffect.SetActive(true);
                IsInteractionAvailable = true;
            };
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
            {
                InteractionAvailableEffect.SetActive(false);
                IsInteractionAvailable = false;
            }

            return true;
        }
    }
}