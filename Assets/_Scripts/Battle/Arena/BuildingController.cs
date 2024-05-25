using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        protected BattleManager BattleManager;
        protected FightManager FightManager;
        BuildingUnlocker _buildingUnlocker;

        protected Building Building;

        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;

        [SerializeField] Canvas _tooltipCanvas;
        [SerializeField] protected TMP_Text TooltipText;
        [SerializeField] protected GameObject InteractionAvailableEffect;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            FightManager = FightManager.Instance;
            FightManager.OnFightEnded += OnFightEnded;
            FightManager.OnFightStarted += OnFightStarted;

            BattleManager = BattleManager.Instance;
            BattleManager.OnBattleInitialized += OnBattleInitialized;
            if (BattleManager.IsTimerOn) OnBattleInitialized();

            SetTooltipText();
        }

        protected virtual void OnBattleInitialized()
        {
        }

        protected virtual void OnFightEnded()
        {
        }

        protected virtual void OnFightStarted()
        {
        }

        protected virtual void Initialize()
        {
            _unlockedGfx.SetActive(Building.IsUnlocked);

            _buildingUnlocker = GetComponentInChildren<BuildingUnlocker>();
            if (_buildingUnlocker == null) return;
            Building.OnUnlocked += Unlock;
            _buildingUnlocker.Initialize(Building);
        }

        protected virtual void AllowInteraction()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected virtual void ForbidInteraction()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
        }

        protected virtual void Unlock()
        {
            _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
        }

        protected virtual void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public string InteractionPrompt => "Press F To Something";

        public virtual bool CanInteract() => IsInteractionAvailable;

        public virtual void DisplayTooltip()
        {
            if (CanInteract())
                _tooltipCanvas.gameObject.SetActive(true);
        }

        public virtual void HideTooltip()
        {
            _tooltipCanvas.gameObject.SetActive(false);
        }

        public virtual bool Interact(Interactor interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}