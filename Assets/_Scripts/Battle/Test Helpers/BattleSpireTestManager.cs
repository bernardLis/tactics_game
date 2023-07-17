using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpireTestManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    void Awake()
    {
    }

    void Start()
    {
        _gameManager = GameManager.Instance;

        Hero h = ScriptableObject.CreateInstance<Hero>();
        h.CreateRandom(1);
        _gameManager.PlayerHero = h;

        Battle b = ScriptableObject.CreateInstance<Battle>();
        b.CreateRandom(1);
        _gameManager.SelectedBattle = b;

        Invoke("Initialize", 1f);
    }

    void Initialize()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.Initialize(_gameManager.PlayerHero);
        _battleManager.GetComponent<BattleWaveManager>().Initialize();

    }
}
