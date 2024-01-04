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

    [SerializeField] Coin _coin;
    [SerializeField] Hammer _hammer;
    [SerializeField] Horseshoe _horseshoe;
    [SerializeField] Bag _bag;
    [SerializeField] Skull _skull;


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
        SpawnPickup();

        OnBroken?.Invoke();

        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    void SpawnPickup()
    {
        // 1% chance of spawning hammer
        // 1% chance of spawning horseshoe
        // 98% chance of spawning coin
        int random = Random.Range(0, 100);
        Pickup p = Instantiate(_coin);

        if (random == 0)
            p = Instantiate(_hammer);
        else if (random == 1)
            p = Instantiate(_horseshoe);
        else if (random == 2)
            p = Instantiate(_bag);
        else if (random == 3)
            p = Instantiate(_skull);

        /*
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
        */
        _battlePickupManager.SpawnPickup(p, transform.position);
    }

    void SpawnHammer()
    {
        if (_hammer == null) return;
        Hammer instance = Instantiate(_hammer);
        BattlePickup hammer = Instantiate(_hammer.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        // hammer.Initialize(instance);

        // Destroy(gameObject, 7f);
    }

    void SpawnHorseshoe()
    {
        if (_horseshoe == null) return;
        Horseshoe instance = Instantiate(_horseshoe);
        BattlePickup horseshoe = Instantiate(_horseshoe.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        // horseshoe.Initialize(instance);

        // Destroy(gameObject, 7f);
    }

    void SpawnBag()
    {
        if (_bag == null) return;
        Bag instance = Instantiate(_bag);
        BattlePickup bag = Instantiate(_bag.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        // bag.Initialize(instance);

        // Destroy(gameObject, 7f);
    }

    void SpawnSkull()
    {
        if (_skull == null) return;
        Skull instance = Instantiate(_skull);
        BattlePickup skull = Instantiate(_skull.Prefab, transform.position, Quaternion.identity)
                            .GetComponent<BattlePickup>();
        // skull.Initialize(instance);

        // Destroy(gameObject, 7f);
    }
}
