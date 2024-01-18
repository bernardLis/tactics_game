


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Lis
{
    public class BattleEntityTooltipDisplayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        BattleEntity _battleEntity;
        BattleTooltipManager _tooltipManager;

        void Start()
        {
            _battleEntity = GetComponent<BattleEntity>();
            _tooltipManager = BattleTooltipManager.Instance;
            _battleEntity.OnDeath += OnDeath;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            VisualElement el = new EntityCard(_battleEntity.Entity);

            _tooltipManager.ShowTooltip(el, gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            _tooltipManager.ShowEntityInfo(_battleEntity);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!CanDisplayTooltip()) return;
            _tooltipManager.HideEntityInfo();
        }

        void OnDeath(BattleEntity a, EntityFight b)
        {
            if (_tooltipManager.CurrentEntityInfo == _battleEntity)
                _tooltipManager.HideEntityInfo();

            if (_tooltipManager.CurrentTooltipDisplayer == _battleEntity.gameObject)
                _tooltipManager.HideTooltip();

            _battleEntity.OnDeath -= OnDeath;
        }

        bool CanDisplayTooltip()
        {

            if (_tooltipManager == null) return false;
            if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return false;
            if (_battleEntity.IsDead) return false;

            return true;
        }

    }
}

