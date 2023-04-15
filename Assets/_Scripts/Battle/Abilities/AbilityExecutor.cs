using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] protected GameObject _areaHighlightPrefab;
    [SerializeField] protected GameObject _effectPrefab;
    [SerializeField] protected GameObject _entityEffectPrefab;

    protected BattleAbilityArea _areaHighlightInstance;
    protected GameObject _effectInstance;
    protected List<BattleEntity> _entitiesInArea = new();

    protected Ability _selectedAbility;

    public void HighlightAbilityArea()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _areaHighlightInstance = Instantiate(_areaHighlightPrefab, worldPos, Quaternion.identity)
                .GetComponent<BattleAbilityArea>();
    }

    public void ClearAbilityHighlight()
    {
        if (_areaHighlightInstance == null) return;
        Destroy(_areaHighlightInstance.gameObject);
    }

    public void ExecuteAbility(Ability ability)
    {
        _effectInstance = Instantiate(_effectPrefab, _areaHighlightInstance.transform.position, Quaternion.identity);
        _entitiesInArea = new(_areaHighlightInstance.GetEntitiesInArea());

        _areaHighlightInstance.IsHighlightingEnabled = false;
        _areaHighlightInstance.ClearHighlightedEntities();
        ClearAbilityHighlight();
        StartCoroutine(ExecuteAbilityCoroutine());
    }

    protected virtual IEnumerator ExecuteAbilityCoroutine()
    {
        yield return null;
    }

    public void CancelAbility()
    {
        ClearAbilityHighlight();
        ClearEffect();
        Invoke("DestroySelf", 0.1f);
    }

    void ClearEffect()
    {
        if (_effectInstance == null) return;
        Destroy(_effectInstance.gameObject);
    }
    void DestroySelf() { Destroy(gameObject); }
}
