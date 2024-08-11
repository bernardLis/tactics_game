using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Map
{
    public class NodeControllerFight : NodeController
    {
        [SerializeField] Transform _swords;
        [SerializeField] Transform _leftSword;
        [SerializeField] Transform _rightSword;
        [SerializeField] GameObject _hitEffect;


        protected override void ResolveNode()
        {
            base.ResolveNode();
            if (!Node.IsVisited) StartCoroutine(AnimateSwords());
        }

        IEnumerator AnimateSwords()
        {
            _swords.DOKill();
            _leftSword.DOKill();
            _rightSword.DOKill();

            _swords.DOLocalMoveY(0, 0.5f).SetEase(Ease.InOutSine);
            _leftSword.DOLocalRotate(new(0, 0, 180), 0.5f).SetEase(Ease.InOutSine);
            _rightSword.DOLocalRotate(new(0, 0, -180), 0.5f).SetEase(Ease.InOutSine);
            _leftSword.DOLocalMoveX(0.5f, 0.5f).SetEase(Ease.InOutSine);
            _rightSword.DOLocalMoveX(-0.5f, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);
            // rotate and move up everything
            _swords.DOLocalMoveY(1, 0.5f).SetEase(Ease.InOutSine);
            _swords.DOLocalRotate(new(0, 360, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.5f);

            // swords clash
            _leftSword.DOLocalRotate(new(0, 0, -135), 0.5f).SetEase(Ease.OutBack);
            _rightSword.DOLocalRotate(new(0, 0, 135), 0.5f).SetEase(Ease.OutBack);
            _leftSword.DOLocalMoveX(0.2f, 0.5f).SetEase(Ease.OutBack);
            _rightSword.DOLocalMoveX(-0.2f, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.15f);

            _hitEffect.SetActive(true);
        }


        public override void SetAvailable()
        {
            base.SetAvailable();
            _swords.DOLocalRotate(new(0, 360, 0), 4f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.InOutSine);
            // _swords.DOLocalMoveY(Random.Range(0.9f, 1.2f), Random.Range(4f, 5f))
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine);
            //
            // _leftSword.DOLocalRotate(new(0, 0, Random.Range(135, 150)), 1f)
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine);
            // _rightSword.DOLocalRotate(new(0, 0, Random.Range(-135, -150)), 1f)
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine);
            //
            // _leftSword.DOLocalMove(new(Random.Range(-0.4f, -0.2f), Random.Range(0.3f, 0.4f), 0), 1f)
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine);
            // _rightSword.DOLocalMove(new(Random.Range(0.2f, 0.4f), Random.Range(0.3f, 0.4f), 0), 1f)
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine);
        }

        public override void SetUnavailable()
        {
            base.SetUnavailable();
            if (this == PlayerController.CurrentNode) return;

            _swords.DOKill();
            _leftSword.DOKill();
            _rightSword.DOKill();

            Icon.gameObject.SetActive(true);
            Gfx.DOScale(0f, 0.5f).SetEase(Ease.InOutBack);
        }
    }
}