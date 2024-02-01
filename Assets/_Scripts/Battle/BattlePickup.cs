using System;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattlePickup : MonoBehaviour
    {
        AudioManager _audioManager;
        BattleManager _battleManager;
        MMF_Player _feelPlayer;

        [SerializeField] Transform _gfx;

        SphereCollider _sphereCollider;
        Hero _hero;

        public Pickup Pickup;

        public event Action<BattlePickup> OnCollected;

        public void Awake()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;

            _feelPlayer = GetComponent<MMF_Player>();

            _hero = _battleManager.GetComponent<BattleHeroManager>().BattleHero.Hero;
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
            _audioManager.PlaySFX(pickup.DropSound, t.position);

            SetPickUpRadius(_hero.Pull.GetValue());
            _sphereCollider.enabled = true;

            if (Pickup is ExperienceOrb) return;
            t.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
                .SetLoops(-1).SetEase(Ease.InOutSine);

            t.DOLocalMoveY(0.8f, 0.5f).SetEase(Ease.OutBack);
        }

        void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent(out BattleHero hero)) return;

            PickUp(hero);
        }

        protected virtual void PickUp(BattleHero hero)
        {
            if (Pickup == null) return;

            _sphereCollider.enabled = false;
            transform.DOKill();
            DisplayText(Pickup.GetCollectedText(), Pickup.Color.Primary);

            Vector3 position = transform.position;
            _audioManager.PlaySFX(Pickup.CollectSound, position);
            Destroy(Instantiate(Pickup.CollectEffect, position, Quaternion.identity), 1f);

            float punchDuration = 0.5f;
            transform.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1);

            Vector3 heroPosition = hero.transform.position;
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
                    Pickup.Collected(hero.Hero);
                    gameObject.SetActive(false);
                });

            OnCollected?.Invoke(this);
        }

        void OnDestroy()
        {
            _hero.Pull.OnValueChanged -= SetPickUpRadius;
        }

        void SetPickUpRadius(int i)
        {
            _sphereCollider.radius = i;
        }

        public void GetToHero()
        {
            if (!gameObject.activeSelf) return;

            transform.DOMove(_battleManager.BattleHero.transform.position + Vector3.up, Random.Range(0.5f, 2f))
                .OnComplete(() => { PickUp(_battleManager.BattleHero); });
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