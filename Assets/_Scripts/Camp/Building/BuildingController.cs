using System;
using Lis.Arena;
using Lis.Arena.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;

        [SerializeField] protected GameObject InteractionAvailableEffect;
        TooltipManager _tooltipManager;

        protected Building Building;
        protected FightManager FightManager;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            FightManager = FightManager.Instance;
            FightManager.OnFightEnded += OnFightEnded;
            FightManager.OnFightStarted += OnFightStarted;

            _tooltipManager = TooltipManager.Instance;

            ArenaInitializer.Instance.OnArenaInitialized += OnArenaInitialized;
            if (FightManager.IsTimerOn) OnArenaInitialized();
        }

        public string InteractionPrompt => "Something";

        public virtual bool CanInteract()
        {
            return IsInteractionAvailable;
        }

        public virtual bool Interact(Interactor interactor)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnArenaInitialized()
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

            Building.OnUnlocked += Unlock;
        }

        protected virtual void AllowInteraction()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected void ForbidInteraction()
        {
            InteractionAvailableEffect.SetActive(false);
            IsInteractionAvailable = false;
        }

        protected virtual void Unlock()
        {
            if (this == null) return;
            _tooltipManager.DisplayGameInfo(
                new Label($" {Helpers.ParseScriptableObjectName(Building.name)} unlocked!"));
            _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
        }
    }
}