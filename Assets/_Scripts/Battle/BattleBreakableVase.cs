using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBreakableVase : MonoBehaviour
{
    bool _isBroken;
    [SerializeField] GameObject _breakParticles;
    [SerializeField] Transform _originalVase;
    [SerializeField] Coin _coin;

    Collider _collider;
    Rigidbody _rigidbody;

    void Start()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        TriggerBreak();
    }

    void TriggerBreak()
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

        SpawnCoin();
    }

    void SpawnCoin()
    {
        if (_coin == null) return;
        Coin instance = Instantiate(_coin);
        BattlePickup coin = Instantiate(_coin.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        coin.Initialize(instance);

        Destroy(gameObject, 7f);
    }
}
