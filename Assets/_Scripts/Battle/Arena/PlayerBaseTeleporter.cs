using Lis.Battle.Fight;
using UnityEngine;
using DG.Tweening;
using Lis.Units.Hero;

namespace Lis.Battle.Arena
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerBaseTeleporter : MonoBehaviour
    {
        [SerializeField] GameObject _gfx;
        SphereCollider _collider;

        void Start()
        {
            _collider = GetComponent<SphereCollider>();

            DisableSelf();

            FightManager.Instance.OnFightEnded += EnableSelf;
            FightManager.Instance.OnFightStarted += DisableSelf;
        }

        void DisableSelf()
        {
            transform.DOScale(0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _gfx.SetActive(false);
                    _collider.enabled = false;
                });
        }

        void EnableSelf()
        {
            _gfx.SetActive(true);

            transform.DOScale(1, 2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => { _collider.enabled = true; });
        }

        void OnTriggerEnter(Collider other)
        {
            if (FightManager.IsFightActive) return;

            if (other.TryGetComponent(out HeroController hero))
                hero.TeleportToBase();
        }
    }
}