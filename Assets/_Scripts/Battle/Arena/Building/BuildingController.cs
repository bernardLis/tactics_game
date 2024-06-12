using System;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena.Building
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;

        [SerializeField] Canvas _tooltipCanvas;
        [SerializeField] protected TMP_Text TooltipText;
        [SerializeField] protected GameObject InteractionAvailableEffect;
        BuildingUnlocker _buildingUnlocker;
        TooltipManager _tooltipManager;
        protected BattleManager BattleManager;

        protected Building Building;
        protected FightManager FightManager;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            FightManager = FightManager.Instance;
            FightManager.OnFightEnded += OnFightEnded;
            FightManager.OnFightStarted += OnFightStarted;

            BattleManager = BattleManager.Instance;
            BattleManager.GetComponent<BattleInitializer>().OnBattleInitialized += OnBattleInitialized;
            if (BattleManager.IsTimerOn) OnBattleInitialized();

            _tooltipManager = TooltipManager.Instance;

            SetTooltipText();
        }

        public string InteractionPrompt => "Press F To Something";

        public virtual bool CanInteract()
        {
            return IsInteractionAvailable;
        }

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
            throw new NotImplementedException();
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

        protected void Initialize()
        {
            _unlockedGfx.SetActive(Building.IsUnlocked);

            _buildingUnlocker = GetComponentInChildren<BuildingUnlocker>();
            Building.OnUnlocked += Unlock;
            if (_buildingUnlocker == null) return;
            _buildingUnlocker.Initialize(Building);
        }

        protected virtual void AllowInteraction()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected void ForbidInteraction()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
        }

        protected void Unlock()
        {
            _tooltipManager.DisplayGameInfo(
                new Label($" {Helpers.ParseScriptableObjectName(Building.name)} unlocked!"));
            _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
        }

        protected virtual void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }
    }
}