using DG.Tweening;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena
{
    [RequireComponent(typeof(SphereCollider))]
    public class HeroMapTeleporter : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject _gfx;
        SphereCollider _collider;

        public string InteractionPrompt => "Exit Arena";

        void Start()
        {
            _collider = GetComponent<SphereCollider>();

            DisableSelf();

            FightManager.Instance.OnFightEnded += EnableSelf;
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

        public bool CanInteract()
        {
            return !FightManager.IsFightActive;
        }

        public bool Interact(Interactor interactor)
        {
            HeroManager.Instance.HeroController.TeleportToMap();
            return true;
        }
    }
}