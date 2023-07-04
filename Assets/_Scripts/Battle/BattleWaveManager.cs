using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        for (int i = 0; i < 5; i++)
        {
            BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();

            wave.NumberOfEnemies = Random.Range(10, 50);
            wave.EnemyLevelRange = new Vector2Int(1, 5);
            wave.Initialize();
            _waves.Add(wave);
        }

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _battleManager.OnBattleInitialized += () =>
        {
            SpawnWave();
            StartCoroutine(WaveSpawnerCoroutine());
        };

    }

    IEnumerator WaveSpawnerCoroutine()
    {
        while (_currentWaveIndex < _waves.Count)
        {
            yield return new WaitForSeconds(30f); // HERE: waves
            _currentWaveIndex++;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        // TODO: something more interesting, like split some armies
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
        foreach (Element element in elements)
        {
            List<Creature> creatures = _waves[_currentWaveIndex].GetAllCreaturesByElement(element);
            if (creatures.Count == 0) continue;

            GameObject portal = Instantiate(_portalPrefab, transform);
            portal.transform.position = RandomPointInAnnulus(Vector3.zero, 10f, 15f);
            CreatureSpawner creatureSpawner = portal.GetComponent<CreatureSpawner>();
            creatureSpawner.SpawnCreatures(creatures);
            creatureSpawner.OnSpawnComplete += (list) =>
            {
                _battleManager.AddOpponentArmyEntities(list);
                creatureSpawner.DestroySelf();
            };
        }
    }

    //https://forum.unity.com/threads/random-point-within-circle-with-min-max-radius.597523/
    public Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
    {
        var randomDirection = (Random.insideUnitCircle * origin).normalized;
        var randomDistance = Random.Range(minRadius, maxRadius);
        var point = origin + randomDirection * randomDistance;
        return point;
    }

}

