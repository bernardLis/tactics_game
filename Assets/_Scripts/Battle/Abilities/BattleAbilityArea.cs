using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleAbilityArea : MonoBehaviour
{

    List<BattleEntity> _entitiesInRange = new();
    public bool IsHighlightingEnabled = true;

    int _floorLayerMask;

    void Start()
    {
        _floorLayerMask = LayerMask.GetMask("Floor");

    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, _floorLayerMask))
        {
            Vector3 pos = new Vector3(hit.point.x - 2f, 0f, hit.point.z);
            transform.position = pos;
        }
    }

    public List<BattleEntity> GetEntitiesInArea() { return _entitiesInRange; }
    public void ClearHighlightedEntities()
    {
        foreach (BattleEntity be in _entitiesInRange)
            be.ToggleHighlight(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsHighlightingEnabled) return;
        if (other.TryGetComponent<BattleEntity>(out BattleEntity be))
        {
            be.ToggleHighlight(true);
            _entitiesInRange.Add(be);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BattleEntity>(out BattleEntity be))
        {
            be.ToggleHighlight(false);
            _entitiesInRange.Remove(be);
        }
    }

}
