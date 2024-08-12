using System;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        protected GameManager GameManager;
        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;

        public Building Building;

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
            if (Building.IsUnlocked) return;

            Building.OnUnlocked += Unlock;
            _unlockedGfx.SetActive(false);
            ForbidInteraction();
            BuildingUnlocker unlocker = GetComponentInChildren<BuildingUnlocker>();
            unlocker.gameObject.SetActive(true);
            unlocker.Initialize(Building);
        }

        protected virtual void AllowInteraction()
        {
            IsInteractionAvailable = true;
        }

        protected void ForbidInteraction()
        {
            IsInteractionAvailable = false;
        }

        protected virtual void Unlock()
        {
            Debug.Log("Building controller unlocked");
            if (this == null) return;
            if (_unlockedEffect != null) _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
        }
    }
}