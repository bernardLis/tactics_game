using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spire : BaseScriptableObject
{
    public List<Storey> Storeys = new();

    public StoreyLives StoreyLives { get; private set; }
    public StoreyTroops StoreyTroops { get; private set; }
    public StoreyMana StoreyMana { get; private set; }

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
                StoreyLives = instance as StoreyLives;
            if (instance is StoreyTroops)
                StoreyTroops = instance as StoreyTroops;
            if (instance is StoreyMana)
                StoreyMana = instance as StoreyMana;
        }
    }

    public void InitializeBattle()
    {
        if (_gameManager == null) _gameManager = GameManager.Instance;

        SpireGameObject = Instantiate(_gameManager.GameDatabase.BaseGameObject, Vector3.zero, Quaternion.identity);
    }
}
