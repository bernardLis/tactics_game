﻿using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BuildingUnlocker : MonoBehaviour, IInteractable
    {
        GameManager _gameManager;

        [SerializeField] TMP_Text _nameText;
        [SerializeField] TMP_Text _priceText;

        Building _building;

        public string InteractionPrompt => "Unlock Building";

        public void Initialize(Building building)
        {
            if (building.IsUnlocked)
            {
                gameObject.SetActive(false);
                return;
            }
            _gameManager = GameManager.Instance;

            _building = building;

            _nameText.text = Helpers.ParseScriptableObjectName(building.name);
            _priceText.text += building.UnlockCost.ToString();
        }

        public bool CanInteract()
        {
            return _gameManager.Gold >= _building.UnlockCost;
        }

        public bool Interact(Interactor interactor)
        {
            Debug.Log("Interacting with BuildingUnlocker");
            if (_gameManager.Gold < _building.UnlockCost)
                return false;

            _gameManager.ChangeGoldValue(-_building.UnlockCost);
            _building.Unlock();
            gameObject.SetActive(false);
            return true;
        }
    }
}