using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCreatureBonding : MonoBehaviour
{
    GameManager _gameManager;
    BattleCameraManager _battleCameraManager;

    BattleCreature _battleCreature;
    Creature _creature;

    Vector3 _originalCamPos;
    Vector3 _originalCamRot;
    float _originalCamZoomHeight;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleCameraManager = BattleCameraManager.Instance;
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

    void OnLevelUp()
    {
        if (_creature.Level == 3) // name change
        {
            RotateCameraAroundBattleEntity();
            CreatureCardFull card = new(_creature, isChangingName: true);
            card.OnHide += ReturnCameraToOriginal;
        }
        if (_creature.Level == _creature.CreatureAbility.UnlockLevel)
        {
            RotateCameraAroundBattleEntity();
            CreatureCardFull card = new(_creature, isUnlockingAbility: true);
            card.OnHide += ReturnCameraToOriginal;
        }

        ResolveEvolution();
    }

    void RotateCameraAroundBattleEntity()
    {
        _originalCamPos = _battleCameraManager.transform.position;
        _originalCamRot = _battleCameraManager.transform.rotation.eulerAngles;
        _originalCamZoomHeight = _battleCameraManager.GetZoomHeight();

        _battleCameraManager.RotateCameraAroundTransform(_battleCreature.transform);

    }

    void ReturnCameraToOriginal()
    {
        _battleCameraManager.MoveCameraTo(_originalCamPos, _originalCamRot, _originalCamZoomHeight);
    }

    /* EVOLUTION */
    void ResolveEvolution()
    {
        //  int maxTier = _gameManager.SelectedBattle.Spire.StoreyTroops.CreatureTierTree.CurrentValue.Value;
        //    if (_creature.UpgradeTier >= maxTier) return;
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
