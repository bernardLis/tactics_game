using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPower : Creatable
{
    BattleUI _battleUI;
    AudioManager _audioManager;

    [SerializeField] Ability _abilityToAdd;
    [SerializeField] GameObject _stepOnEffect;
    [SerializeField] Sound _enterSound;
    [SerializeField] Sound _exitSound;

    ParticleSystem _ps;

    void Start()
    {
        _battleUI = BattleUI.Instance;
        _audioManager = AudioManager.Instance;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _abilityToAdd.Icon;

        _ps = GetComponent<ParticleSystem>();
        _ps.textureSheetAnimation.SetSprite(0, _abilityToAdd.Icon);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // shield someone as they enter
        if (other.TryGetComponent(out CharacterStats stats))
            EnterPlaceOfPower(stats);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        // remove shield
        if (other.TryGetComponent(out CharacterStats stats))
            ExitPlaceOfPower(stats);
    }

    void EnterPlaceOfPower(CharacterStats stats)
    {
        Destroy(Instantiate(_stepOnEffect, transform.position, Quaternion.identity), 1f);
        _battleUI.DisplayBattleLog($"{stats.gameObject.name} enters Place Of Power");
        stats.ReplaceAbility(_abilityToAdd);
        _audioManager.PlaySFX(_enterSound, transform.position);
    }

    void ExitPlaceOfPower(CharacterStats stats)
    {
        _battleUI.DisplayBattleLog($"{stats.gameObject.name} exits Place Of Power");
        stats.RevertAbilityReplace();
        _audioManager.PlaySFX(_exitSound, transform.position);
    }

    public override string DisplayText()
    {
        return "Place of power. Replaces your first ability as long as you stand on it.";
    }


}
