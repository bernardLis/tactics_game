using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntityCanvas : MonoBehaviour
{
    BattleEntity _entity;

    Canvas _canvas;

    // Start is called before the first frame update
    void Start()
    {
        _entity = transform.parent.GetComponent<BattleEntity>();
        _entity.OnDeath += OnEntityDeath;

        _canvas = GetComponent<Canvas>();
    }


    void OnEntityDeath(BattleEntity be) { _canvas.enabled = false; }

}
