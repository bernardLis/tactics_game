using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleWaveManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    [SerializeField] GameObject _portalPrefab;

    [SerializeField] List<BattleWave> _waves = new();
    int _currentWaveIndex;

    void Start()
    {
        _gameManager = GameManager.Instance;

        // HERE: waves you could pass a list of waves
        for (int i = 0; i < 3; i++)
        {
            BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();

            wave.NumberOfEnemies = Random.Range(1, 5);
            wave.EnemyLevelRange = new Vector2Int(1, 5);
            wave.Initialize();
            _waves.Add(wave);
        }

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _battleManager.OnBattleInitialized += SpawnWave;

        _battleManager.OnPlayerEntityDeath += (count) =>
        {
            if (count == 0)
                _battleManager.LoseBattle();
        };
        _battleManager.OnOpponentEntityDeath += ResolveNextWave;

    }
    void ResolveNextWave(int count)
    {
        if (count != 0) return;
        _currentWaveIndex++;
        if (_currentWaveIndex >= _waves.Count)
        {
            _battleManager.WinBattle();
            return;
        }
        SpawnWave();
    }
    void SpawnWave()
    {
        Debug.Log($"spawn wave {_currentWaveIndex}");
        // TODO: something more interesting, like split some armies
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
        foreach (Element element in elements)
        {
            List<Creature> creatures = _waves[_currentWaveIndex].GetAllCreaturesByElement(element);
            if (creatures.Count == 0) continue;
            Debug.Log($"element.name {element.ElementName}, creatures.Count: {creatures.Count}");

            // https://forum.unity.com/threads/random-point-within-circle-with-min-max-radius.597523/
            Vector2 point = Random.insideUnitCircle.normalized * Random.Range(30, 50);
            Vector3 pos = new Vector3(point.x, 0, point.y);
            Vector3 lookRotation = (pos - Vector3.zero).normalized; // TODO: math, this seems dumb

            GameObject portal = Instantiate(_portalPrefab, pos, Quaternion.LookRotation(lookRotation));

            CreatureSpawner creatureSpawner = portal.GetComponent<CreatureSpawner>();
            creatureSpawner.SpawnCreatures(creatures, portalElement: element);
            creatureSpawner.OnSpawnComplete += (list) =>
            {
                _battleManager.AddOpponentArmyEntities(list);
                creatureSpawner.DestroySelf();

                foreach (BattleEntity be in list)
                    be.OnDeath += ClearBody;
            };
        }

        void ClearBody(BattleEntity be, BattleEntity killer, Ability ability)
        {
            be.transform.DOMoveY(-1, 10f)
                    .SetDelay(3f)
                    .OnComplete(() => Destroy(be.gameObject));
        }
    }
}

