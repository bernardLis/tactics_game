using Lis.Battle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitTooltipDisplayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        UnitController _unitController;
        TooltipManager _tooltipManager;

        void Start()
        {
            _unitController = GetComponent<UnitController>();
            _tooltipManager = TooltipManager.Instance;
            _unitController.OnDeath += OnDeath;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            VisualElement el = new UnitCard(_unitController.Unit);

            _tooltipManager.ShowTooltip(el, gameObject);
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
            if (_tooltipManager.CurrentEntityInfo == _unitController)
                _tooltipManager.HideEntityInfo();

            if (_tooltipManager.CurrentTooltipDisplayer == _unitController.gameObject)
                _tooltipManager.HideTooltip();
        }

        bool CanDisplayTooltip()
        {
            if (_tooltipManager == null) return false;
            if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return false;
            if (_unitController == null) return false;
            if (_unitController.IsDead) return false;

            return true;
        }
    }
}