﻿using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class BuildingUnlocker : MonoBehaviour, IInteractable
    {
        [SerializeField] private Canvas _tooltipCanvas;
        [SerializeField] protected TMP_Text TooltipText;
        private Building _building;

        public string InteractionPrompt => "Press F To Unlock Building";

        public bool CanInteract()
        {
            return true;
        }

        public void DisplayTooltip()
        {
            if (CanInteract())
                _tooltipCanvas.gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            _tooltipCanvas.gameObject.SetActive(false);
        }

        public bool Interact(Interactor interactor)
        {
            if (GameManager.Instance.Gold < _building.UnlockCost)
                return false;

            GameManager.Instance.ChangeGoldValue(-_building.UnlockCost);
            _building.Unlock();
            gameObject.SetActive(false);
            return true;
        }

        public void Initialize(Building building)
        {
            if (building.IsUnlocked) gameObject.SetActive(false);
            _building = building;
            TooltipText.text =
                $"Press F to Unlock {Helpers.ParseScriptableObjectName(building.name)} for {building.UnlockCost}";
        }
    }
}