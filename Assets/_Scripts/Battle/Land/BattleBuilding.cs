using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleBuilding : MonoBehaviour, IInteractable
    {
        protected GameManager GameManager;
        protected BattleManager BattleManager;
        protected BattleTooltipManager TooltipManager;

        [SerializeField] GameObject _corruptionEffectPrefab;
        GameObject _buildingCorruptionEffect;

        protected Building Building;

        protected ProgressBarHandler ProgressBarHandler;

        protected IEnumerator _corruptionCoroutine;
        bool _corruptionPaused;

        public event Action OnBuildingCorrupted;

        public virtual void Initialize(Vector3 pos, Building building)
        {
            GameManager = GameManager.Instance;
            BattleManager = BattleManager.Instance;
            TooltipManager = BattleTooltipManager.Instance;

            Building = building;

            transform.localPosition = pos;

            ProgressBarHandler = GetComponentInChildren<ProgressBarHandler>();
            ProgressBarHandler.Initialize();
            ProgressBarHandler.HideProgressBar();

            StartCoroutine(SecuredCoroutine());
        }

        protected virtual IEnumerator SecuredCoroutine()
        {
            yield return ShowBuilding();

            Building.Secure();
        }

        protected virtual IEnumerator ShowBuilding()
        {
            Transform t = transform;
            Vector3 scale = t.localScale;
            t.localScale = Vector3.zero;
            transform.LookAt(BattleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);

            transform.DOScale(scale, 1f)
                .SetEase(Ease.OutBack);
            yield return transform.DOLocalMoveY(transform.localPosition.y + scale.x * 0.5f, 1f)
                .SetEase(Ease.OutBack)
                .WaitForCompletion();
        }

        public virtual void StartCorruption(BattleBoss boss)
        {
            boss.OnCorruptionBroken += BreakCorruption;
            _corruptionCoroutine = CorruptionCoroutine();
            StartCoroutine(_corruptionCoroutine);
        }

        protected void PauseCorruption()
        {
            _corruptionPaused = true;
            DOTween.Pause("CorruptionEffectRotation");
        }

        protected void ResumeCorruption()
        {
            _corruptionPaused = false;
            DOTween.Play("CorruptionEffectRotation");
        }

        protected IEnumerator CorruptionCoroutine()
        {
            yield return DisplayCorruptionEffect();
            Color c = GameManager.GameDatabase.GetColorByName("Corruption").Primary;
            ProgressBarHandler.SetFillColor(c);
            ProgressBarHandler.SetBorderColor(Color.black);
            ProgressBarHandler.SetProgress(0);
            ProgressBarHandler.ShowProgressBar();

            int totalSecondsToCorrupt = Building.SecondsToCorrupt +
                                        GameManager.UpgradeBoard.GetUpgradeByName("Corruption Duration").GetValue();

            for (int i = 0; i <= totalSecondsToCorrupt; i++)
            {
                if (_corruptionPaused) yield return new WaitUntil(() => !_corruptionPaused);
                yield return new WaitForSeconds(1);
                ProgressBarHandler.SetProgress((float)i / totalSecondsToCorrupt);
            }

            yield return HideCorruptionEffect();
            Corrupted();
        }

        protected virtual void BreakCorruption()
        {
            if (_corruptionCoroutine != null)
                StopCoroutine(_corruptionCoroutine);
            _corruptionCoroutine = null;

            ProgressBarHandler.HideProgressBar();
            StartCoroutine(HideCorruptionEffect());
        }

        IEnumerator DisplayCorruptionEffect()
        {
            Vector3 pos = transform.position;
            pos.y = 0.02f;
            _buildingCorruptionEffect = Instantiate(_corruptionEffectPrefab, pos, Quaternion.identity);
            _buildingCorruptionEffect.transform.localScale = Vector3.zero;
            float scale = transform.localScale.x;
            yield return _buildingCorruptionEffect.transform.DOScale(scale, 0.5f)
                .SetEase(Ease.OutBack)
                .WaitForCompletion();

            _buildingCorruptionEffect.transform.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.InOutSine)
                .SetId("CorruptionEffectRotation");
        }

        IEnumerator HideCorruptionEffect()
        {
            if (_buildingCorruptionEffect == null) yield break;
            _buildingCorruptionEffect.transform.DOKill();
            yield return _buildingCorruptionEffect.transform.DOScale(0, 1f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(_buildingCorruptionEffect))
                .WaitForCompletion();
        }

        protected virtual void Corrupted()
        {
            Building.Corrupted();
            OnBuildingCorrupted?.Invoke();
        }

        /* INTERACTION */
        public virtual bool CanInteract(BattleInteractor interactor)
        {
            if (_corruptionCoroutine != null) return false;
            return Building.IsSecured;
        }

        public virtual void DisplayTooltip()
        {
            if (TooltipManager == null) return;
            TooltipManager.ShowTooltip(new BuildingCard(Building), gameObject);
        }

        public void HideTooltip()
        {
            if (TooltipManager == null) return;
            TooltipManager.HideTooltip();
        }

        public virtual bool Interact(BattleInteractor interactor)
        {
            return true;
        }
    }
}