using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleBreakableVase : MonoBehaviour
{
    BattlePickupManager _battlePickupManager;

    bool _isBroken;
    [SerializeField] GameObject _breakParticles;
    [SerializeField] Transform _originalVase;

    Collider _collider;
    Rigidbody _rigidbody;

    public event Action OnBroken;
    void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();

        _battlePickupManager = BattleManager.Instance.GetComponent<BattlePickupManager>();
    }

    public void Initialize(Vector3 position)
    {
        if (_breakParticles != null)
            _breakParticles.SetActive(false);

        transform.position = position;
        transform.localScale = Vector3.zero;
        _isBroken = false;
        _collider.enabled = true;
        _rigidbody.isKinematic = false;

        gameObject.SetActive(true);
        _originalVase.gameObject.SetActive(true);

        transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);
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

        OnBroken?.Invoke();

        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
