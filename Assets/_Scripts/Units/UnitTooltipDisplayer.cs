using Lis.Battle;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Units
{
    public class UnitTooltipDisplayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        TooltipManager _tooltipManager;
        UnitCardFactory _unitCardFactory;
        UnitController _unitController;

        void Start()
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

        void OnDeath(UnitController a, Attack.Attack b)
        {
            if (this == null) return;
            if (_tooltipManager.CurrentEntityInfo == _unitController)
                _tooltipManager.HideEntityInfo();

            if (_tooltipManager.CurrentTooltipDisplayer == gameObject)
                _tooltipManager.HideTooltip();
        }

        void OnRevival()
        {
            _tooltipManager.HideTooltip();
        }

        bool CanDisplayTooltip()
        {
            if (_tooltipManager == null) return false;
            if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return false;
            if (_unitController == null) return false;

            return true;
        }
    }
}