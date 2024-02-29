using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleBreakableVase : MonoBehaviour
    {
        BattlePickupManager _battlePickupManager;

        bool _isBroken;
        [SerializeField] GameObject _breakParticles;
        [SerializeField] Transform _originalVase;

        Collider _collider;
        Rigidbody _rigidbody;

        public event Action<BattleBreakableVase> OnBroken;

        void Awake()
        {
            _battlePickupManager = BattleManager.Instance.GetComponent<BattlePickupManager>();

            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(Vector3 position)
        {
            if (_breakParticles != null)
                _breakParticles.SetActive(false);

            Transform t = transform;
            t.position = position;
            t.localScale = Vector3.zero;
            _isBroken = false;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;

            gameObject.SetActive(true);
            _originalVase.gameObject.SetActive(true);

            transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        void OnMouseDown()
        {
            TriggerBreak();
        }

        public void TriggerBreak()
        {
            StartCoroutine(BreakObject());
        }

        IEnumerator BreakObject()
        {
            // TODO: play audio

            if (_isBroken) yield break;
            _isBroken = true;

            _collider.enabled = false;
            _rigidbody.isKinematic = true;

            if (_breakParticles != null)
                _breakParticles.SetActive(true);

            _originalVase.gameObject.SetActive(false);
            _battlePickupManager.SpawnPickup(transform.position);

            OnBroken?.Invoke(this);

            yield return new WaitForSeconds(5f);
            gameObject.SetActive(false);
        }
    }
}