using System;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Pickup
{
    public class PickupController : MonoBehaviour
    {
        AudioManager _audioManager;
        BattleManager _battleManager;
        MMF_Player _feelPlayer;

        [SerializeField] Transform _gfx;

        SphereCollider _sphereCollider;
        Hero _hero;

        public Pickup Pickup;

        public event Action<PickupController> OnCollected;

        public void Awake()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;

            _feelPlayer = GetComponent<MMF_Player>();

            _hero = _battleManager.GetComponent<HeroManager>().HeroController.Hero;
            _sphereCollider = GetComponent<SphereCollider>();

            _hero.Pull.OnValueChanged += SetPickUpRadius;
        }

        public void Initialize(Pickup pickup, Vector3 position)
        {
            if (_gfx.childCount > 0)
                Destroy(_gfx.GetChild(0).gameObject);
            Instantiate(pickup.Gfx, _gfx);

            Transform t = transform;
            t.position = position;
            gameObject.SetActive(true);
            
            t.localScale = Vector3.zero;
            t.DOScale(1, 1f).SetEase(Ease.OutBack);
            
            Pickup = pickup;
            Pickup.Initialize();
            _audioManager.PlaySfx(pickup.DropSound, t.position);

            SetPickUpRadius(_hero.Pull.GetValue());
            _sphereCollider.enabled = true;

            if (Pickup is ExperienceStone) return;
            t.DORotate(new(0, 360, 0), 15f, RotateMode.FastBeyond360)
                .SetLoops(-1).SetEase(Ease.InOutSine);

            t.DOLocalMoveY(0.8f, 0.5f).SetEase(Ease.OutBack);
        }

        void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent(out HeroController hero)) return;

            PickUp(hero);
        }

        void PickUp(HeroController heroController)
        {
            if (Pickup == null) return;

            _sphereCollider.enabled = false;
            transform.DOKill();
            DisplayText(Pickup.GetCollectedText(), Pickup.Color.Primary);

            Vector3 position = transform.position;
            _audioManager.PlaySfx(Pickup.CollectSound, position);
            Destroy(Instantiate(Pickup.CollectEffect, position, Quaternion.identity), 1f);

            float punchDuration = 0.5f;
            transform.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1);

            Vector3 heroPosition = heroController.transform.position;
            Vector3 jumpPos = new(
                heroPosition.x,
                heroPosition.y + 2f,
                heroPosition.z
            );
            float jumpDuration = 0.6f;

            transform.DOScale(0, jumpDuration)
                .SetDelay(punchDuration);

            transform.DOJump(jumpPos, 5f, 1, jumpDuration)
                .SetDelay(punchDuration)
                .OnComplete(() =>
                {
                    Pickup.Collected(heroController.Hero);
                    gameObject.SetActive(false);
                });

            OnCollected?.Invoke(this);
        }

        void OnDestroy()
        {
            _hero.Pull.OnValueChanged -= SetPickUpRadius;
        }

        void SetPickUpRadius(float i)
        {
            _sphereCollider.radius = i;
        }

        public void GetToHero()
        {
            if (!gameObject.activeSelf) return;

            transform.DOMove(_battleManager.HeroController.transform.position + Vector3.up, Random.Range(0.5f, 2f))
                .OnComplete(() => { PickUp(_battleManager.HeroController); });
        }

        void DisplayText(string text, Color color)
        {
            MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = text;
            floatingText.ForceColor = true;
            floatingText.AnimateColorGradient = Helpers.GetGradient(color);
            Transform t = transform;
            Vector3 pos = t.position + new Vector3(0, t.localScale.y * 0.8f, 0);
            _feelPlayer.PlayFeedbacks(pos);
        }
    }
}