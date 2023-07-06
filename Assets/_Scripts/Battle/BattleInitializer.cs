using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class BattleInitializer : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    BattleCameraManager _battleCameraManager;
    BattleInputManager _battleInputManager;
    PlayerArmyDeployer _playerArmyDeployer;

    Transform _entityHolder;

    [SerializeField] Transform _playerSpawnPoint;
    [SerializeField] Transform _enemySpawnPoint;

    [SerializeField] GameObject _obstaclePrefab;
    GameObject _obstacleInstance;

    [SerializeField] GameObject _creatureSpawnerPrefab;

    Hero _playerHero;
    Battle _selectedBattle;
    Hero _opponentHero;

    Label _waveLabel;
    int _currentWaveIndex;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleCameraManager = _battleManager.GetComponent<BattleCameraManager>();
        _battleInputManager = _battleManager.GetComponent<BattleInputManager>();
        _playerArmyDeployer = _battleManager.GetComponent<PlayerArmyDeployer>();

        _waveLabel = _battleManager.Root.Q<Label>("waveCount");
        _waveLabel.style.display = DisplayStyle.Flex;

        _entityHolder = _battleManager.EntityHolder;

        _battleCameraManager = Camera.main.GetComponentInParent<BattleCameraManager>();

        _playerHero = _gameManager.PlayerHero;

        // HERE: waves
        if (_playerHero == null)
        {
            _playerHero = ScriptableObject.CreateInstance<Hero>();
            _playerHero.CreateRandom(1);
            _playerHero.Army = new(_gameManager.HeroDatabase.GetStartingArmy(_playerHero.Element).Creatures);
        }

        _selectedBattle = _gameManager.SelectedBattle;
        _opponentHero = _gameManager.SelectedBattle.Opponent;

        StartCoroutine(BattleStartShow());
    }

    IEnumerator BattleStartShow()
    {
        _battleInputManager.enabled = false;

        yield return new WaitForSeconds(0.5f);

        _battleCameraManager.MoveCameraToDefaultPosition(3f);

        if (_selectedBattle.IsObstacleActive)
            PlaceObstacle();

        yield return new WaitForSeconds(1f);

        _battleManager.Initialize(_playerHero);

        ResolveBattleType();

        yield return new WaitForSeconds(2f);

        _playerArmyDeployer.Initialize();
        _battleInputManager.enabled = true;
    }

    void PlaceObstacle()
    {
        if (_obstacleInstance != null)
            Destroy(_obstacleInstance);

        // between player and enemy
        float posX = _playerSpawnPoint.transform.position.x + (_enemySpawnPoint.transform.position.x - _playerSpawnPoint.transform.position.x) / 2;
        float posZ = _playerSpawnPoint.transform.position.z + (_enemySpawnPoint.transform.position.z - _playerSpawnPoint.transform.position.z) / 2;
        Vector3 pos = new Vector3(posX, 10, posZ);

        float sizeY = Random.Range(3, 10);
        float sizeX = Random.Range(10, 30);
        float sizeZ = Random.Range(1, 5);
        Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 rot = new Vector3(0, Random.Range(-45, 45), 0);

        _obstacleInstance = Instantiate(_obstaclePrefab, pos, Quaternion.identity);
        _obstacleInstance.transform.localScale = size;
        _obstacleInstance.transform.Rotate(rot);
    }

    void ResolveBattleType()
    {
        // HERE: deployment
        // InitializePlayerArmy();
        if (_selectedBattle.BattleType == BattleType.Duel)
            InitializeOpponentArmy();

        if (_selectedBattle.BattleType == BattleType.Waves)
        {
            _battleManager.BlockBattleEnd = true;
            _battleManager.OnPlayerEntityDeath += (count) =>
            {
                if (count == 0)
                    _battleManager.LoseBattle();
            };
            _battleManager.OnOpponentEntityDeath += ResolveNextWave;
            SpawnWave();
        }
    }

    void InitializeOpponentArmy()
    {
        Vector3 oppPortalRotation = new(0, 180, 0);
        GameObject opponentSpawnerInstance = Instantiate(_creatureSpawnerPrefab, _enemySpawnPoint.position,
                 Quaternion.Euler(oppPortalRotation));
        CreatureSpawner opponentSpawner = opponentSpawnerInstance.GetComponent<CreatureSpawner>();
        opponentSpawner.SpawnHeroArmy(_opponentHero, 1.5f);
        opponentSpawner.OnSpawnComplete += (list) =>
        {
            opponentSpawner.DestroySelf();
            _battleManager.AddOpponentArmyEntities(list);
        };
    }
    void ResolveNextWave(int count)
    {
        if (count != 0) return;
        _currentWaveIndex++;
        if (_currentWaveIndex >= _selectedBattle.Waves.Count)
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
            List<Creature> creatures = _selectedBattle.Waves[_currentWaveIndex].GetAllCreaturesByElement(element);
            if (creatures.Count == 0) continue;

            // https://forum.unity.com/threads/random-point-within-circle-with-min-max-radius.597523/
            Vector2 point = Random.insideUnitCircle.normalized * Random.Range(50, 80);
            Vector3 pos = new Vector3(point.x, 0, point.y);
            Vector3 lookRotation = (pos - Vector3.zero).normalized; // TODO: math, this seems dumb

            GameObject portal = Instantiate(_creatureSpawnerPrefab, pos, Quaternion.LookRotation(lookRotation));

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
        _waveLabel.text = $"Wave: {_currentWaveIndex + 1} / {_selectedBattle.Waves.Count}";
    }

}
