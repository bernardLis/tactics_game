using System;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        protected GameManager GameManager;
        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;

        [SerializeField] protected GameObject InteractionAvailableEffect;

        protected Building Building;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            GameManager = GameManager.Instance;
            Initialize();
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

        protected virtual void Initialize()
        {
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
            if (_unlockedEffect != null) _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
        }
    }
}