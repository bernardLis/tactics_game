using DG.Tweening;
using UnityEngine;

namespace Lis.Map
{
    public class NodeControllerFight : NodeController
    {
        [SerializeField] Transform _swords;
        [SerializeField] Transform _leftSword;
        [SerializeField] Transform _rightSword;

        Sequence _swordFightSequence;

        protected override void ResolveNode()
        {
            base.ResolveNode();
        }

        public override void SetAvailable()
        {
            base.SetAvailable();
            _swords.DOLocalRotate(new(0, 360, 0), 4f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.InOutSine);
            _swords.DOLocalMoveY(Random.Range(0.9f, 1.2f), Random.Range(4f, 5f))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            _leftSword.DOLocalRotate(new(0, 0, Random.Range(135, 150)), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            _rightSword.DOLocalRotate(new(0, 0, Random.Range(-135, -150)), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            _leftSword.DOLocalMove(new(Random.Range(-0.4f, -0.2f), Random.Range(0.3f, 0.4f), 0), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            _rightSword.DOLocalMove(new(Random.Range(0.2f, 0.4f), Random.Range(0.3f, 0.4f), 0), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
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