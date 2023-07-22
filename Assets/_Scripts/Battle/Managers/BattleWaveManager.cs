using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleWaveManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Battle _selectedBattle;

    Transform _entityHolder;

    Label _waveLabel;
    int _currentWaveIndex;

    [SerializeField] GameObject _entitySpawnerPrefab;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _selectedBattle = _gameManager.SelectedBattle;

        _entityHolder = _battleManager.EntityHolder;

        _waveLabel = _battleManager.Root.Q<Label>("waveCount");
        _waveLabel.style.display = DisplayStyle.Flex;
    }

    public void Initialize()
    {
        _battleManager.OnOpponentEntityDeath += ResolveNextWave;
        SpawnWave();
    }

    void ResolveNextWave(BattleEntity ignored)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        _currentWaveIndex++;
        SpawnWave();
    }

    void SpawnWave()
    {
        UpdateWaveLabel();
        BattleWave wave = _selectedBattle.GetWave(_currentWaveIndex);

        InstantiateMinions(wave);
        InstantiateCreatures(wave);
    }

    void InstantiateMinions(BattleWave wave)
    {
        // TODO: something more interesting, like split some armies
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
        foreach (Element element in elements)
        {
            List<Minion> minions = wave.GetAllMinionsByElement(element);
            if (minions.Count == 0) continue;

            EntitySpawner creatureSpawner = InstantiateSpawner();
            creatureSpawner.SpawnMinions(minions, portalElement: element);
            creatureSpawner.OnSpawnComplete += OnEntitySpawnComplete;
        }
    }

    void InstantiateCreatures(BattleWave wave)
    {
        foreach (Creature c in wave.Creatures)
        {
            EntitySpawner creatureSpawner = InstantiateSpawner();
            List<Creature> creatures = new List<Creature>() { c };
            creatureSpawner.SpawnCreatures(creatures);
            creatureSpawner.OnSpawnComplete += OnEntitySpawnComplete;
        }
    }

    EntitySpawner InstantiateSpawner()
    {
        // https://forum.unity.com/threads/random-point-within-circle-with-min-max-radius.597523/
        Vector2 point = Random.insideUnitCircle.normalized * Random.Range(50, 80);
        Vector3 pos = new Vector3(point.x, 0, point.y);
        Vector3 lookRotation = (pos - Vector3.zero).normalized; // TODO: math, this seems dumb

        GameObject portal = Instantiate(_entitySpawnerPrefab, pos, Quaternion.LookRotation(lookRotation));

        return portal.GetComponent<EntitySpawner>();
    }

    void OnEntitySpawnComplete(List<BattleEntity> list)
    {
        _battleManager.AddOpponentArmyEntities(list);

        foreach (BattleEntity be in list)
            be.OnDeath += ClearBody;
    }

    void ClearBody(BattleEntity be, BattleEntity killer, Ability ability)
    {
        be.transform.DOMoveY(-1, 10f)
                .SetDelay(3f)
                .OnComplete(() =>
                {
                    be.transform.DOKill();
                    Destroy(be.gameObject);
                });
    }


    void UpdateWaveLabel()
    {
        _waveLabel.text = $"Wave: {_currentWaveIndex + 1}";
    }

}
