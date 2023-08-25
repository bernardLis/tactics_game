using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleSpire : Singleton<BattleSpire>
{
    public Spire Spire { get; private set; }

    void Start()
    {
        Spire = ScriptableObject.CreateInstance<Spire>();
        Spire.Initialize();
    }
}
