using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Random = UnityEngine.Random;

public class BattleEntityTestManager : MonoBehaviour
{
    GameManager _gameManager;


    [SerializeField] List<ArmyGroup> TeamAArmies = new();
    [SerializeField] List<ArmyGroup> TeamBArmies = new();

    void Start()
    {
        BattleManager.Instance.Initialize(null, null, TeamAArmies, TeamBArmies);
    }


}
