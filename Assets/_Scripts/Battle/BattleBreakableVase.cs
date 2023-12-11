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
    [SerializeField] Bag _bag;
    [SerializeField] Skull _skull;


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
        // 1% chance of spawning hammer
        // 1% chance of spawning horseshoe
        // 98% chance of spawning coin
        int random = Random.Range(0, 100);

        if (random == 0)
            SpawnHammer();
        else if (random == 1)
            SpawnHorseshoe();
        else if (random == 2)
            SpawnBag();
        else if (random == 3)
            SpawnSkull();
        else
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

    void SpawnBag()
    {
        if (_bag == null) return;
        Bag instance = Instantiate(_bag);
        BattlePickup bag = Instantiate(_bag.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        bag.Initialize(instance);

        Destroy(gameObject, 7f);
    }

    void SpawnSkull()
    {
        if (_skull == null) return;
        Skull instance = Instantiate(_skull);
        BattlePickup skull = Instantiate(_skull.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        skull.Initialize(instance);

        Destroy(gameObject, 7f);
    }
}
