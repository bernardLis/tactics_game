using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class BattleWaveTestManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Label _waveLabel;

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

            wave.NumberOfEnemies = Random.Range(10, 25);
            wave.EnemyLevelRange = new Vector2Int(1, 5);
            wave.Initialize();
            _waves.Add(wave);
        }

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _battleManager.OnBattleInitialized += SpawnWave;

        _waveLabel = _battleManager.Root.Q<Label>("waveCount");
        _waveLabel.style.display = DisplayStyle.Flex;

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
        UpdateWaveLabel();
        // TODO: something more interesting, like split some armies
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
        foreach (Element element in elements)
        {
            List<Creature> creatures = _waves[_currentWaveIndex].GetAllCreaturesByElement(element);
            if (creatures.Count == 0) continue;

            // https://forum.unity.com/threads/random-point-within-circle-with-min-max-radius.597523/
            Vector2 point = Random.insideUnitCircle.normalized * Random.Range(50, 80);
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

    void UpdateWaveLabel()
    {
        _waveLabel.text = $"Wave: {_currentWaveIndex} / {_waves.Count}";
    }


}

