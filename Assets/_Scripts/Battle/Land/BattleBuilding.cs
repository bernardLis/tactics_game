using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Lis
{
    public class BattleBuilding : MonoBehaviour, IInteractable
    {
        protected GameManager GameManager;
        protected BattleManager BattleManager;
        protected BattleTooltipManager TooltipManager;

        [SerializeField] GameObject _gfx;
        [SerializeField] protected Canvas Canvas;

        Building _building;
        Vector3 _originalScale;

        bool _corruptionPaused;

        public virtual void Initialize(Vector3 pos, Building building)
        {
            GameManager = GameManager.Instance;
            BattleManager = BattleManager.Instance;
            TooltipManager = BattleTooltipManager.Instance;

            _building = building;

            Transform t = transform;
            t.localPosition = pos;
            _originalScale = t.localScale;

            Canvas.gameObject.SetActive(false);
        }

        public void ShowBuilding()
        {
            _building.Unlocked();
            StartCoroutine(ShowBuildingCoroutine());
        }

        protected virtual IEnumerator ShowBuildingCoroutine()
        {
            _gfx.SetActive(true);

            Transform t = transform;
            t.localScale = Vector3.zero;
            transform.LookAt(BattleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);

            transform.DOScale(_originalScale, 1f)
                .SetEase(Ease.OutBack);
            yield return null;
        }

        /* INTERACTION */
        public virtual bool CanInteract(BattleInteractor interactor)
        {
            return true;
        }

        public virtual void DisplayTooltip()
        {
            Canvas.gameObject.SetActive(true);
        }

        public virtual void HideTooltip()
        {
            Canvas.gameObject.SetActive(false);
        }

        public virtual bool Interact(BattleInteractor interactor)
        {
            return true;
        }
    }
}