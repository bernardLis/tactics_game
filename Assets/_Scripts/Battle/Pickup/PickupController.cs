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
        [SerializeField] private Transform _gfx;

        public Pickup Pickup;
        private AudioManager _audioManager;
        private BattleManager _battleManager;
        private MMF_Player _feelPlayer;
        private Hero _hero;
        private HeroController _heroController;
        private Rigidbody _rigidbody;

        private SphereCollider _sphereCollider;

        public void Awake()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;

            _feelPlayer = GetComponent<MMF_Player>();

            _heroController = _battleManager.GetComponent<HeroManager>().HeroController;
            _hero = _heroController.Hero;

            _sphereCollider = GetComponent<SphereCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _hero.Pull.OnValueChanged += SetPickUpRadius;
        }

        private void OnDestroy()
        {
            _hero.Pull.OnValueChanged -= SetPickUpRadius;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent(out HeroController hero)) return;

            PickUp(hero);
        }

        public event Action<PickupController> OnCollected;

        public void Initialize(Pickup pickup, Vector3 position)
        {
            if (_gfx.childCount > 0)
                Destroy(_gfx.GetChild(0).gameObject);

            Pickup = pickup;
            Pickup.Initialize();

            Transform t = transform;
            if (Pickup is not ExperienceStone)
            {
                _rigidbody.isKinematic = true;
                position.y = 0;
            }

            t.position = position;
            t.rotation = Quaternion.identity;
            gameObject.SetActive(true);

            _gfx.localPosition = Vector3.zero;
            _gfx.localRotation = Quaternion.identity;

            Instantiate(pickup.Gfx, _gfx);

            t.localScale = Vector3.zero;
            t.DOScale(1, 1f).SetEase(Ease.OutBack);

            _audioManager.PlaySfx(pickup.DropSound, t.position);

            SetPickUpRadius(_hero.Pull.GetValue());
            _sphereCollider.enabled = true;

            if (Pickup is ExperienceStone)
            {
                Fall();
                return;
            }

            _gfx.DORotate(new(0, 360, 0), 15f, RotateMode.FastBeyond360)
                .SetLoops(-1).SetEase(Ease.InOutSine);

            _gfx.DOLocalMoveY(0.8f, 0.5f).SetEase(Ease.OutBack);
        }

        private void Fall()
        {
            Vector3 position = transform.position;
            position.y = 0;
            transform.DOMove(position, 1f)
                .SetEase(Ease.OutBounce);
        }

        private void PickUp(HeroController heroController)
        {
            if (Pickup == null) return;

            _sphereCollider.enabled = false;
            Transform t = transform;
            _gfx.DOKill();
            t.DOKill();

            Pickup.HandleHeroBonuses(heroController.Hero);
            DisplayText(Pickup.GetCollectedText(), Pickup.Color.Primary);

            Vector3 position = t.position;
            _audioManager.PlaySfx(Pickup.CollectSound, position);
            // Destroy(Instantiate(Pickup.CollectEffect, position, Quaternion.identity), 1f);

            float punchDuration = 0.5f;
            t.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1);

            Vector3 jumpPos = transform.position + Vector3.up * 3;
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

        private void SetPickUpRadius(float i)
        {
            _sphereCollider.radius = i;
        }

        public void GetToHero()
        {
            if (!gameObject.activeSelf) return;

            transform.DOMove(_heroController.transform.position + Vector3.up, Random.Range(0.5f, 2f))
                .OnComplete(() => { PickUp(_heroController); });
        }

        private void DisplayText(string text, Color color)
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