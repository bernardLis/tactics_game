using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOf : Creatable
{
    protected BattleUI _battleUI;
    AudioManager _audioManager;

    protected SpriteRenderer _sr;
    protected ParticleSystem _ps;

    [SerializeField] GameObject _stepOnEffect;
    [SerializeField] Sound _enterSound;
    [SerializeField] Sound _exitSound;

    protected CharacterStats _characterOnPlace;

    void Start()
    {
        _battleUI = BattleUI.Instance;
        _audioManager = AudioManager.Instance;

        _sr = GetComponentInChildren<SpriteRenderer>();
        _ps = GetComponent<ParticleSystem>();

        Initialize();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    async void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (_characterOnPlace == null)
            return;

        if (Status == null)
            return;

        if (_characterOnPlace.CompareTag(Tags.Player) && state == BattleState.PlayerTurn)
            await _characterOnPlace.AddStatus(Status, null);

        if (_characterOnPlace.CompareTag(Tags.Enemy) && state == BattleState.EnemyTurn)
            await _characterOnPlace.AddStatus(Status, null);
    }


    protected virtual void Initialize()
    {
        //meant to be overwritten
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CharacterStats stats))
            EnterPlaceOf(stats);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out CharacterStats stats))
            ExitPlaceOf(stats);
    }

    protected virtual void EnterPlaceOf(CharacterStats stats)
    {
        Destroy(Instantiate(_stepOnEffect, transform.position, Quaternion.identity), 1f);
        _audioManager.PlaySFX(_enterSound, transform.position);
    }

    protected virtual void ExitPlaceOf(CharacterStats stats)
    {
        _audioManager.PlaySFX(_exitSound, transform.position);
    }



}
