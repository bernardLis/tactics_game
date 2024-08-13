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
        [SerializeField] GameObject _buildingUnlocker;

        [HideInInspector] public Building Building;

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
            if (Building.IsUnlocked)
            {
                AllowInteraction();
                return;
            }

            Building.OnUnlocked += Unlock;
            _unlockedGfx.SetActive(false);
            ForbidInteraction();
            _buildingUnlocker.SetActive(true);
            BuildingUnlocker unlocker = _buildingUnlocker.GetComponent<BuildingUnlocker>();
            unlocker.Initialize(Building);
        }

        protected void AllowInteraction()
        {
            IsInteractionAvailable = true;
        }

        protected void ForbidInteraction()
        {
            IsInteractionAvailable = false;
        }

        void Unlock()
        {
            Debug.Log("Building controller unlocked");
            if (this == null) return;
            if (_unlockedEffect != null) _unlockedEffect.SetActive(true);
            _unlockedGfx.SetActive(true);
            AllowInteraction();
        }
    }
}