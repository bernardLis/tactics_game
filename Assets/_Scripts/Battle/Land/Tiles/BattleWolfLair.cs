using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleWolfLair : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Building _wolfLair;

    [SerializeField] BattleEntitySpawner _spawnerPrefab;
    [SerializeField] Transform _spawnPoint;

    IEnumerator _friendlyWolfSpawnCoroutine;
    float _currentProductionDelaySecond;
    public List<BattleCreature> _friendlyWolves = new();



    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        _wolfLair = Instantiate(_gameManager.GameDatabase.GetBuildingByName("Wolf Lair"));
        _wolfLair.Initialize();
        _wolfLair.OnUpgradePurchased += () => StartFriendlyWolfSpawnCoroutine();

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(2.5f);

        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);
    }

    public void SpawnWave(int difficulty)
    {
        BuildingUpgrade bu = _wolfLair.GetCurrentUpgrade();

        // TODO: difficulty
        List<Entity> entitiesToSpawn = new();
        for (int i = 0; i < difficulty * 3; i++)
            entitiesToSpawn.Add(Instantiate(bu.ProducedCreature));

        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab, _spawnPoint.position, transform.rotation);
        spawner.SpawnEntities(entitiesToSpawn);
        spawner.OnSpawnComplete += (l) =>
        {
            _battleManager.AddOpponentArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    public void Secured()
    {
        StartFriendlyWolfSpawnCoroutine();
    }

    void StartFriendlyWolfSpawnCoroutine()
    {
        if (_friendlyWolfSpawnCoroutine != null) return;

        _friendlyWolfSpawnCoroutine = SpawnFriendlyWolves();
        StartCoroutine(_friendlyWolfSpawnCoroutine);
    }

    IEnumerator SpawnFriendlyWolves()
    {
        while (_friendlyWolves.Count < _wolfLair.GetCurrentUpgrade().ProductionLimit)
        {
            SpawnFriendlyWolf();
            yield return ProductionDelay();
        }
        _friendlyWolfSpawnCoroutine = null;
    }

    IEnumerator ProductionDelay()
    {
        float totalDelay = _wolfLair.GetCurrentUpgrade().ProductionDelay;
        _currentProductionDelaySecond = 0f;
        while (_currentProductionDelaySecond < totalDelay)
        {
            _currentProductionDelaySecond += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnFriendlyWolf()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature wolf = Instantiate(_wolfLair.GetCurrentUpgrade().ProducedCreature);
        spawner.SpawnEntities(new List<Entity>() { wolf });
        spawner.OnSpawnComplete += (l) =>
        {
            // now I need to track the spawned wolf
            BattleCreature bc = l[0] as BattleCreature;
            _friendlyWolves.Add(bc);
            // if it dies, and coroutine is inactive - restart coroutine
            bc.OnDeath += (_, __) =>
            {
                _friendlyWolves.Remove(bc);
                StartFriendlyWolfSpawnCoroutine();
            };

            _battleManager.AddPlayerArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;

        _tooltipManager.ShowHoverInfo(new BattleInfoElement("Woof Lair"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideHoverInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
    }
}
