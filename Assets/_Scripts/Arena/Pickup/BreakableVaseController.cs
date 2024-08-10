using System;
using System.Collections;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    public class BreakableVaseController : MonoBehaviour
    {
        [SerializeField] GameObject _breakParticles;
        [SerializeField] Transform _originalVase;
        [SerializeField] Sound _breakSound;
        AudioManager _audioManager;

        Collider _collider;

        public bool IsBroken { get; private set; }
        PickupManager _pickupManager;
        Rigidbody _rigidbody;

        void Awake()
        {
            _audioManager = AudioManager.Instance;
            _pickupManager = FightManager.Instance.GetComponent<PickupManager>();

            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void OnMouseDown()
        {
            TriggerBreak();
        }

        public event Action<BreakableVaseController> OnBroken;

        public void Initialize(Vector3 position)
        {
            if (_breakParticles != null)
                _breakParticles.SetActive(false);

            Transform t = transform;
            t.position = position;
            t.localScale = Vector3.zero;
            IsBroken = false;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;

            gameObject.SetActive(true);
            _originalVase.gameObject.SetActive(true);

            transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        public void TriggerBreak()
        {
            StartCoroutine(BreakObject());
        }

        IEnumerator BreakObject()
        {
            if (IsBroken) yield break;
            IsBroken = true;

            _audioManager.CreateSound()
                .WithSound(_breakSound)
                .WithPosition(transform.position)
                .Play();

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