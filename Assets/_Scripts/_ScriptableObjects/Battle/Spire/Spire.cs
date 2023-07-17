using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spire : BaseScriptableObject
{
    public List<Storey> Storeys = new();

    public StoreyLives LivesUpgrade;
    public StoreyTroops TroopsUpgrade;
    public StoreyMana ManaUpgrade;

    public GameObject SpireGameObject;

    GameManager _gameManager;

    public void Initialize()
    {
        _gameManager = GameManager.Instance;

        foreach (Storey u in _gameManager.GameDatabase.AllBaseUpgrades)
        {
            Storey instance = Instantiate(u);
            instance.Initialize();
            Storeys.Add(instance);

            if (instance is StoreyLives)
                LivesUpgrade = instance as StoreyLives;
            if (instance is StoreyTroops)
                TroopsUpgrade = instance as StoreyTroops;
            if (instance is StoreyMana)
                ManaUpgrade = instance as StoreyMana;
        }
    }

    public void InitializeBattle()
    {
        if (_gameManager == null) _gameManager = GameManager.Instance;

        SpireGameObject = Instantiate(_gameManager.GameDatabase.BaseGameObject, Vector3.zero, Quaternion.identity);

        foreach (Storey b in Storeys)
            b.InitializeBattle();
    }
}
