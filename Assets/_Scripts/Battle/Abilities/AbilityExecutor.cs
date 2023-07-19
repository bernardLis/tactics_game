using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;

public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] protected GameObject _areaHighlightPrefab;
    [SerializeField] protected GameObject _effectPrefab;
    [SerializeField] protected GameObject _entityEffectPrefab;

    protected BattleAbilityArea _areaHighlightInstance;
    protected GameObject _effectInstance;
    protected List<BattleEntity> _entitiesInArea = new();

    protected Ability _selectedAbility;

    protected int _damageDealt;

    public void HighlightAbilityArea(Ability ability)
    {
        _selectedAbility = ability;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _areaHighlightInstance = Instantiate(_areaHighlightPrefab, worldPos, Quaternion.identity)
                .GetComponent<BattleAbilityArea>();

        _areaHighlightInstance.Initialize(ability);
    }

    public virtual void ExecuteAbility(Ability ability)
    {
        if (_areaHighlightInstance == null) return;

        _selectedAbility = ability;

        _effectInstance = Instantiate(_effectPrefab, _areaHighlightInstance.transform.position, Quaternion.identity);
        _entitiesInArea = new(_areaHighlightInstance.GetEntitiesInArea());

        _areaHighlightInstance.IsHighlightingEnabled = false;
        _areaHighlightInstance.ClearHighlightedEntities();
        ClearAbilityHighlight();

        if (_effectInstance.TryGetComponent<AbilityEffectScaler>(out AbilityEffectScaler aes))
            aes.Initialize(ability.GetScale());

        StartCoroutine(ExecuteAbilityCoroutine());
    }

    protected virtual IEnumerator ExecuteAbilityCoroutine()
    {
        yield return null;
    }

    public void CancelAbility() { Invoke("DestroySelf", 0.1f); }

    public virtual void ClearAbilityHighlight()
    {
        if (_areaHighlightInstance == null) return;
        _areaHighlightInstance.ClearHighlightedEntities();
        DOTween.Kill(_areaHighlightInstance.transform);
        Destroy(_areaHighlightInstance.gameObject);
    }

    public virtual void CancelAbilityHighlight()
    {
        if (_areaHighlightInstance == null) return;
        ClearAbilityHighlight();
        DestroySelf();
    }

    public void ClearEffect()
    {
        if (_effectInstance == null) return;
        // HERE: ability scale testing Destroy(_effectInstance.gameObject);
    }

    void DestroySelf()
    {
        ClearEffect();
        Destroy(gameObject);
    }
}
