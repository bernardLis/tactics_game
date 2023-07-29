using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalSphereManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    [SerializeField] GameObject _elementalSpherePrefab;

    void Start()
    {
        _battleManager = GetComponent<BattleManager>();
        SpawnSpheres();
    }

    void SpawnSpheres()
    {
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());

    }

}
