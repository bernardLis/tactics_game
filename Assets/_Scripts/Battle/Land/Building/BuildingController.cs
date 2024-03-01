using System.Collections;
using DG.Tweening;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Land.Building
{
    public class BuildingController : MonoBehaviour
    {
        protected BattleManager BattleManager;

        [SerializeField] GameObject _gfx;
        [SerializeField] protected Canvas Canvas;

        protected Building Building;
        Vector3 _originalScale;

        bool _corruptionPaused;

        public virtual void Initialize(Vector3 pos, Building building)
        {
            BattleManager = BattleManager.Instance;

            Building = building;

            Transform t = transform;
            t.localPosition = pos;
            _originalScale = t.localScale;

            Canvas.gameObject.SetActive(false);

            GetComponentInChildren<PlayerUnitTracker>().OnEntityEnter += OnEntityEnter;
            GetComponentInChildren<PlayerUnitTracker>().OnEntityExit += OnEntityExit;
        }

        public void ShowBuilding()
        {
            Building.Unlocked();
            StartCoroutine(ShowBuildingCoroutine());
        }

        protected virtual IEnumerator ShowBuildingCoroutine()
        {
            _gfx.SetActive(true);

            Transform t = transform;
            t.localScale = Vector3.zero;
            transform.LookAt(BattleManager.GetComponent<HeroManager>().HeroController.transform.position);

            transform.DOScale(_originalScale, 1f)
                .SetEase(Ease.OutBack);
            yield return null;
        }


        void OnEntityEnter(UnitController be)
        {
            if (!be.TryGetComponent(out HeroController _)) return;
            DisplayTooltip();
        }

        void OnEntityExit(UnitController be)
        {
            if (!be.TryGetComponent(out HeroController _)) return;
            HideTooltip();
        }

        public virtual void DisplayTooltip()
        {
            Canvas.gameObject.SetActive(true);
        }

        public virtual void HideTooltip()
        {
            Canvas.gameObject.SetActive(false);
        }
    }
}