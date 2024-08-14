using System;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BuildingController : MonoBehaviour, IInteractable
    {
        protected GameManager GameManager;
        protected CampManager CampManager;
        protected CampConsoleManager CampConsoleManager;
        Hero _hero;

        [SerializeField] GameObject _unlockedGfx;
        [SerializeField] GameObject _unlockedEffect;
        [SerializeField] GameObject _buildingUnlocker;
        [SerializeField] TMP_Text _nameText;

        [HideInInspector] public Building Building;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            GameManager = GameManager.Instance;
            CampManager = CampManager.Instance;
            CampConsoleManager = CampConsoleManager.Instance;

            _hero = GameManager.Campaign.Hero;
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
            _nameText.text = Helpers.ParseScriptableObjectName(Building.name);

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

        protected virtual void Unlock()
        {
            if (this == null) return;
            if (_unlockedEffect != null) _unlockedEffect.SetActive(true);
            CampConsoleManager.ShowMessage($"{Helpers.ParseScriptableObjectName(Building.name)} unlocked.");
            _unlockedGfx.SetActive(true);
            AllowInteraction();
        }

        public virtual void AssignUnit(UnitCampController ucc)
        {
            ucc.OnGrabbed += ReleaseUnit;
        }

        void ReleaseUnit(UnitCampController ucc)
        {
            CampConsoleManager.ShowMessage($"Releasing unit from Gold Mine.");
            _hero.AddArmy(ucc.Unit);

            Building.ReleaseUnit(ucc.Unit);
        }
    }
}