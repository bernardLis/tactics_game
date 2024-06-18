using Lis.Battle.Fight;
using UnityEngine;
using DG.Tweening;
using Lis.Core;
using Lis.Units.Hero;

namespace Lis.Battle.Arena
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerBaseTeleporter : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject _gfx;
        SphereCollider _collider;

        public string InteractionPrompt => "Teleport To Base";

        HeroController _heroController;

        void Start()
        {
            _collider = GetComponent<SphereCollider>();

            DisableSelf();

            FightManager.Instance.OnFightEnded += EnableSelf;
            FightManager.Instance.OnFightStarted += DisableSelf;

            BattleManager.Instance.GetComponent<BattleInitializer>().OnBattleInitialized += OnBattleInitialized;
        }

        void OnBattleInitialized()
        {
            _heroController = HeroManager.Instance.HeroController;
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
            _heroController.TeleportToBase();
            return true;
        }
    }
}