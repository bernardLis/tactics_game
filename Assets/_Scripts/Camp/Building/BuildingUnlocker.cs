using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BuildingUnlocker : MonoBehaviour, IInteractable
    {
        [SerializeField] Canvas _tooltipCanvas;
        [SerializeField] protected TMP_Text TooltipText;
        Building _building;

        public string InteractionPrompt => "Unlock Building";

        public void Initialize(Building building)
        {
            if (building.IsUnlocked) gameObject.SetActive(false);
            _building = building;
            TooltipText.text =
                $"Press F to Unlock {Helpers.ParseScriptableObjectName(building.name)} for {building.UnlockCost}";
        }

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
    }
}