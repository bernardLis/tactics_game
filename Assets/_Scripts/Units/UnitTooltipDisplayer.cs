using Lis.Battle;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Units
{
    public class UnitTooltipDisplayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        private TooltipManager _tooltipManager;
        private UnitCardFactory _unitCardFactory;
        private UnitController _unitController;

        private void Start()
        {
            _unitController = GetComponentInParent<UnitController>();
            _tooltipManager = TooltipManager.Instance;
            _unitCardFactory = UnitCardFactory.Instance;

            _unitController.OnDeath += OnDeath;
            if (_unitController.Unit == null) return;
            _unitController.Unit.OnRevival += OnRevival;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;

            _tooltipManager.ShowTooltip(_unitCardFactory.CreateUnitCard(_unitController.Unit), gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            _tooltipManager.ShowEntityInfo(_unitController);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            _tooltipManager.HideEntityInfo();
        }

        private void OnDeath(UnitController a, Attack.Attack b)
        {
            if (this == null) return;
            if (_tooltipManager.CurrentEntityInfo == _unitController)
                _tooltipManager.HideEntityInfo();

            if (_tooltipManager.CurrentTooltipDisplayer == gameObject)
                _tooltipManager.HideTooltip();

            if (_unitController.Team == 1)
                GetComponent<BoxCollider>().enabled = false;
        }

        private void OnRevival()
        {
            _tooltipManager.HideTooltip();
        }

        private bool CanDisplayTooltip()
        {
            if (_tooltipManager == null) return false;
            if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return false;
            if (_unitController == null) return false;

            return true;
        }
    }
}