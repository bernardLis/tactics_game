using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCreatureBonding : MonoBehaviour
{
    GameManager _gameManager;
    AudioManager _audioManager;
    BattleCameraManager _battleCameraManager;
    BattleTooltipManager _battleTooltipManager;

    [SerializeField] Sound _bondingLevelSound;

    BattleCreature _battleCreature;
    Creature _creature;

    Vector3 _originalCamPos;
    Vector3 _originalCamRot;
    float _originalCamZoomHeight;

    CreatureCardFull _card;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _battleCameraManager = BattleCameraManager.Instance;
        _battleTooltipManager = BattleTooltipManager.Instance;
        Initialize();
    }

    void Initialize()
    {
        if (TryGetComponent(out _battleCreature))
        {
            if (_battleCreature.Creature == null) return;
            _creature = _battleCreature.Creature;
            _creature.OnLevelUp += OnLevelUp;
        }
    }

    void OnDisable()
    {
        if (_creature != null)
            _creature.OnLevelUp -= OnLevelUp;
    }

    void OnDestroy()
    {
        if (_creature != null)
            _creature.OnLevelUp -= OnLevelUp;
    }

    void OnLevelUp()
    {
        // TODO: skipping bonding for now...
        // if (_creature.Level.Value == 3) // name change
        //     HandleNameChange();
        // if (_creature.Level.Value == _creature.CreatureAbility.UnlockLevel)
        //     HandleAbilityUnlock();

        // ResolveEvolution();
    }

    void HandleNameChange()
    {
        BaseBondingShow();
        //    _card = new(_creature, isChangingName: true);
        //    _card.OnHide += NameChangeHide;

    }

    void HandleAbilityUnlock()
    {
        BaseBondingShow();
        //  _card = new(_creature, isUnlockingAbility: true);
        //   _card.OnHide += CreatureCardFullHidden;
    }

    void BaseBondingShow()
    {
        if (_battleTooltipManager.CurrentTooltipDisplayer == gameObject)
            _battleTooltipManager.HideTooltip(); // TODO: this is a workaround for multiple sounds playing when unlocking ability
        _audioManager.PlaySFX(_bondingLevelSound, transform.position);
        RotateCameraAroundBattleEntity();
    }

    void RotateCameraAroundBattleEntity()
    {
        _originalCamPos = _battleCameraManager.transform.position;
        _originalCamRot = _battleCameraManager.transform.rotation.eulerAngles;
        _originalCamZoomHeight = _battleCameraManager.transform.localPosition.y;

        _battleCameraManager.RotateCameraAround(_battleCreature.transform);
    }

    void NameChangeHide()
    {
        AudioManager.Instance.PlayUI("Creature Bonding Name");
        CreatureCardFullHidden();
        _card.OnHide -= NameChangeHide;
    }

    void CreatureCardFullHidden()
    {
        _battleCameraManager.MoveCameraTo(_originalCamPos, _originalCamRot, _originalCamZoomHeight);
        _card.OnHide -= CreatureCardFullHidden;
    }

    /* EVOLUTION */
    void ResolveEvolution()
    {
        int maxTier = 0;//BattleSpire.Instance.Spire.StoreyTroops.CreatureTierTree.CurrentValue.Value;
        if (_creature.UpgradeTier >= maxTier) return;
        if (_creature.ShouldEvolve())
        {
            // HERE: evolution camera management

            //    _card = new(_creature, isEvolving: true);
            //     _card.OnHide += Evolve;
        }
    }

    void Evolve()
    {
        _card.OnHide -= Evolve;
        _battleCreature.Evolve();
    }
}
