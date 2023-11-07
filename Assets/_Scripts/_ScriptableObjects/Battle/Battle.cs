using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
public class Battle : BaseScriptableObject
{
    GameManager _gameManager;

    public int Duration = 900; // seconds
    public int TilesUntilBoss = 3;

    public bool Won;

    public void CreateRandom(int level)
    {
        _gameManager = GameManager.Instance;
    }
}