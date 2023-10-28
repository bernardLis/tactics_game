using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    protected GameManager _gameManager;
    protected BattleManager _battleManager;
    protected BattleTooltipManager _tooltipManager;
    protected BattleFightManager _battleFightManager;

    [SerializeField] GameObject _banner;
    [SerializeField] protected BattleEntitySpawner _spawnerPrefab;
    [SerializeField] protected Transform _spawnPoint;


    protected Building _building;

    protected IEnumerator _productionCoroutine;
    float _currentProductionDelaySecond;

    protected List<BattleCreature> _producedCreatures = new();


    public virtual void Initialize(Building building)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _battleFightManager = BattleFightManager.Instance;
        _battleFightManager.OnWaveSpawned += SpawnWave;
        _battleFightManager.OnFightEnded += Secured;

        _building = building;
        _building.OnUpgradePurchased += () => StartProductionCoroutine();

        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 1f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(2.5f);

        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);

    }

    public virtual void SpawnWave()
    {
        int difficulty = _battleFightManager.CurrentDifficulty;
        BuildingUpgrade bu = _building.GetCurrentUpgrade();

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
        _battleFightManager.OnWaveSpawned -= SpawnWave;

        _banner.SetActive(true);
        _building.Secure();
        StartProductionCoroutine();
    }

    protected void StartProductionCoroutine()
    {
        if (!_building.IsSecured) return;
        if (_productionCoroutine != null) return;

        _productionCoroutine = ProductionCoroutine();
        StartCoroutine(_productionCoroutine);
    }

    protected virtual IEnumerator ProductionCoroutine()
    {
        while (_producedCreatures.Count < _building.GetCurrentUpgrade().ProductionLimit)
        {
            SpawnFriendlyCreature();
            yield return ProductionDelay();
        }
        _productionCoroutine = null;
    }

    void SpawnFriendlyCreature()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature wolf = Instantiate(_building.GetCurrentUpgrade().ProducedCreature);
        spawner.SpawnEntities(new List<Entity>() { wolf });
        spawner.OnSpawnComplete += (l) =>
        {
            // now I need to track the spawned wolf
            BattleCreature bc = l[0] as BattleCreature;
            _producedCreatures.Add(bc);
            // if it dies, and coroutine is inactive - restart coroutine
            bc.OnDeath += (_, __) =>
            {
                _producedCreatures.Remove(bc);
                StartProductionCoroutine();
            };

            _battleManager.AddPlayerArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    protected IEnumerator ProductionDelay()
    {
        float totalDelay = _building.GetCurrentUpgrade().ProductionDelay;
        _currentProductionDelaySecond = 0f;
        while (_currentProductionDelaySecond < totalDelay)
        {
            _currentProductionDelaySecond += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    /* Mouse */
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowHoverInfo(
                    new BattleInfoElement($"<b>{Helpers.ParseScriptableObjectName(_building.name)}</b>"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideHoverInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        _tooltipManager.ShowTooltip(new BuildingCard(_building), gameObject);
    }

}
