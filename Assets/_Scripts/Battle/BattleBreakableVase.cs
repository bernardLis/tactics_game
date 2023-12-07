using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleBreakableVase : MonoBehaviour
{
    bool _isBroken;
    [SerializeField] GameObject _breakParticles;
    [SerializeField] Transform _originalVase;
    [SerializeField] Coin _coin;
    [SerializeField] Hammer _hammer;
    [SerializeField] Horseshoe _horseshoe;

    Collider _collider;
    Rigidbody _rigidbody;

    public event Action OnBroken;
    void Start()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
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
        SpawnPickup();

        OnBroken?.Invoke();
    }

    void SpawnPickup()
    {
        // TODO: lower chance of spawning hammer and horseshoe
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                SpawnCoin();
                break;
            case 1:
                SpawnHammer();
                break;
            case 2:
                SpawnHorseshoe();
                break;
        }
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

    void SpawnHammer()
    {
        if (_hammer == null) return;
        Hammer instance = Instantiate(_hammer);
        BattlePickup hammer = Instantiate(_hammer.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        hammer.Initialize(instance);

        Destroy(gameObject, 7f);
    }

    void SpawnHorseshoe()
    {
        if (_horseshoe == null) return;
        Horseshoe instance = Instantiate(_horseshoe);
        BattlePickup horseshoe = Instantiate(_horseshoe.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        horseshoe.Initialize(instance);

        Destroy(gameObject, 7f);
    }
}
