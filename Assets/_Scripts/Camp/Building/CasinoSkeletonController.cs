using DG.Tweening;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class CasinoSkeletonController : MonoBehaviour
    {
        CasinoController _casinoController;
        Animator _animator;

        static readonly int Spin = Animator.StringToHash("Spin");
        static readonly int Win = Animator.StringToHash("Won");
        static readonly int Lose = Animator.StringToHash("Lost");

        public void Initialize(CasinoController casinoController)
        {
            _casinoController = casinoController;
            _animator = GetComponent<Animator>();

            _casinoController.OnSpin += OnSpin;
            _casinoController.OnSpinCompleted += OnSpinCompleted;
        }

        void OnDestroy()
        {
            _casinoController.OnSpin -= OnSpin;
            _casinoController.OnSpinCompleted -= OnSpinCompleted;
        }

        void OnSpin()
        {
            if (this == null) return;

            transform.DOLocalRotate(new(0, 270, 0), 0.3f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => { _animator.SetTrigger(Spin); });

            transform.DOLocalRotate(new(0, 180, 0), 0.5f)
                .SetEase(Ease.InOutSine)
                .SetDelay(1.5f);
        }

        void OnSpinCompleted(bool hasWon)
        {
            if (this == null) return;

            _animator.SetTrigger(hasWon ? Win : Lose);
        }
    }
}