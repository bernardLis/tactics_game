using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleBuildingHomeCrystal : BattleBuilding
    {
        [SerializeField] Transform _GFX;

        [SerializeField] GameObject _landingEffect;

        protected override IEnumerator ShowBuilding()
        {
            yield return null;
        }

        void Awake()
        {
        }

        void Start()
        {
            // transform.localScale = Vector3.zero;
            // transform.position += Vector3.up * 15f;

            // StartCoroutine(LandingCoroutine());
        }

        IEnumerator LandingCoroutine()
        {
            yield return transform.DOMoveY(2f, 5f)
                .SetDelay(4f)
                .SetEase(Ease.OutQuad).WaitForCompletion();

            yield return new WaitForSeconds(1f);
            yield return transform.DOPunchScale(Vector3.one * 0.5f, 1f, 10, 1f)
                .SetLoops(3, LoopType.Restart);
            yield return transform.DOPunchPosition(Vector3.one * 0.5f, 1f, 10, 1f)
                .SetLoops(3, LoopType.Restart)
                .WaitForCompletion();

            yield return new WaitForSeconds(1.5f);
            Vector3 scale = transform.localScale;
            yield return transform.DOScale(scale * 2f, 0.2f)
                .SetEase(Ease.OutCubic)
                .WaitForCompletion();

            Destroy(Instantiate(_landingEffect, transform.position + Vector3.up * 3f, Quaternion.identity), 2f);
            transform.DOScale(scale * 0.5f, 1f)
                .SetEase(Ease.OutBounce);

            List<BattleEntity> enemies = new(BattleManager.GetOpponents(0));
            foreach (BattleEntity be in enemies)
                be.BaseGetHit(100, Color.red);

            yield return new WaitForSeconds(4f);

            _GFX.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart);
        }

        public override void StartCorruption(BattleBoss boss)
        {
            boss.OnCorruptionBroken += PauseCorruption;
            boss.OnStunFinished += ResumeCorruption;

            _corruptionCoroutine = CorruptionCoroutine();
            StartCoroutine(_corruptionCoroutine);
        }

        protected override void Corrupted()
        {
            base.Corrupted();
            BattleManager.LoseBattle();
        }
    }
}
