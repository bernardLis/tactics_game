using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : BaseScriptableObject
{
    public List<BaseUpgrade> AllBaseUpgrades = new();

    public BaseUpgradeLives LivesUpgrade;
    public BaseUpgradeTroops TroopsUpgrade;
    public BaseUpgradeMana ManaUpgrade;

    //public IntVariable Lives;
    //public IntVariable TroopsLimit;

    public GameObject BaseGameObject;

    GameManager _gameManager;

    public void Initialize()
    {
        _gameManager = GameManager.Instance;

        //   Lives = ScriptableObject.CreateInstance<IntVariable>();
        //  TroopsLimit = ScriptableObject.CreateInstance<IntVariable>();

        foreach (BaseUpgrade u in _gameManager.GameDatabase.AllBaseUpgrades)
        {
            BaseUpgrade instance = Instantiate(u);
            instance.Initialize();
            AllBaseUpgrades.Add(instance);

            if (u is BaseUpgradeLives)
                LivesUpgrade = u as BaseUpgradeLives;
            if (u is BaseUpgradeTroops)
                TroopsUpgrade = u as BaseUpgradeTroops;
            if (u is BaseUpgradeMana)
                ManaUpgrade = u as BaseUpgradeMana;
        }
    }

    public void InitializeBattle()
    {
        if (_gameManager == null) _gameManager = GameManager.Instance;

        BaseGameObject = Instantiate(_gameManager.GameDatabase.BaseGameObject, Vector3.zero, Quaternion.identity);

        foreach (BaseUpgrade b in AllBaseUpgrades)
            b.InitializeBattle();
    }
}
