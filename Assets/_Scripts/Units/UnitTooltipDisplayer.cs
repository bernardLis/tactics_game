using Lis.Battle;
using Lis.Units.Pawn;
using Lis.Units.Peasant;
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
            _unitController = GetComponentInParent<UnitController>();
            _tooltipManager = TooltipManager.Instance;
            _unitController.OnDeath += OnDeath;
            _unitController.Unit.OnRevival += OnRevival;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            VisualElement el = new UnitCard(_unitController.Unit);

            if (_unitController.Unit is Pawn.Pawn pawn)
                el = new PawnCard(pawn);
            if (_unitController.Unit is Peasant.Peasant peasant)
                el = new PeasantCard(peasant);

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