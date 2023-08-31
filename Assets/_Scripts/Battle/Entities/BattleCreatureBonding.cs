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
        if (_creature.Level == 3) // name change
        {
            NameChange();
        }
        if (_creature.Level == _creature.CreatureAbility.UnlockLevel)
        {
            if (_battleTooltipManager.CurrentTooltipDisplayer == gameObject)
                _battleTooltipManager.HideTooltip(); // TODO: this is a workaround for multiple sounds playing when unlocking ability
            _audioManager.PlaySFX(_bondingLevelSound, transform.position);
            RotateCameraAroundBattleEntity();
            CreatureCardFull card = new(_creature, isUnlockingAbility: true);
            card.OnHide += CreatureCardFullHidden;
        }

        ResolveEvolution();
    }

    void NameChange()
    {
        _audioManager.PlaySFX(_bondingLevelSound, transform.position);

        CreatureCardFull card = new(_creature, isChangingName: true);
        card.OnHide += CreatureCardFullHidden;

        RotateCameraAroundBattleEntity();
    }

    void RotateCameraAroundBattleEntity()
    {
        _originalCamPos = _battleCameraManager.transform.position;
        _originalCamRot = _battleCameraManager.transform.rotation.eulerAngles;
        _originalCamZoomHeight = _battleCameraManager.transform.localPosition.y;

        _battleCameraManager.RotateCameraAround(_battleCreature.transform);
    }

    void CreatureCardFullHidden()
    {
        _battleCameraManager.MoveCameraTo(_originalCamPos, _originalCamRot, _originalCamZoomHeight);
    }

    /* EVOLUTION */
    void ResolveEvolution()
    {
        int maxTier = BattleSpire.Instance.Spire.StoreyTroops.CreatureTierTree.CurrentValue.Value;
        if (_creature.UpgradeTier >= maxTier) return;
        if (_creature.ShouldEvolve())
        {
            // HERE: evolution camera management

            CreatureCardFull card = new(_creature, isEvolving: true);
            card.OnHide += Evolve;
        }
    }

    void Evolve()
    {
        _battleCreature.Evolve();
    }
}
