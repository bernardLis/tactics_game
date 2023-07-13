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

            if (instance is BaseUpgradeLives)
                LivesUpgrade = instance as BaseUpgradeLives;
            if (instance is BaseUpgradeTroops)
                TroopsUpgrade = instance as BaseUpgradeTroops;
            if (instance is BaseUpgradeMana)
                ManaUpgrade = instance as BaseUpgradeMana;

            Debug.Log($"initalize {LivesUpgrade}");

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
