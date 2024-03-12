using System;
using System.Collections;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    public class BreakableVaseController : MonoBehaviour
    {
        AudioManager _audioManager;
        PickupManager _pickupManager;

        bool _isBroken;
        [SerializeField] GameObject _breakParticles;
        [SerializeField] Transform _originalVase;
        [SerializeField] Sound _breakSound;

        Collider _collider;
        Rigidbody _rigidbody;

        public event Action<BreakableVaseController> OnBroken;

        void Awake()
        {
            _audioManager = AudioManager.Instance;
            _pickupManager = BattleManager.Instance.GetComponent<PickupManager>();

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
            if (_isBroken) yield break;
            _isBroken = true;

            _audioManager.PlaySfx(_breakSound, transform.position);

            _collider.enabled = false;
            _rigidbody.isKinematic = true;

            if (_breakParticles != null)
                _breakParticles.SetActive(true);

            _originalVase.gameObject.SetActive(false);
            _pickupManager.SpawnPickup(transform.position);

            OnBroken?.Invoke(this);

            yield return new WaitForSeconds(5f);
            gameObject.SetActive(false);
        }
    }
}