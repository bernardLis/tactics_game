using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleBuildingProduction : BattleBuilding, IInteractable
{
    [SerializeField] protected BattleEntitySpawner _spawnerPrefab;
    [SerializeField] protected Transform _spawnPoint;

    [SerializeField] protected SpriteRenderer[] _starRenderers;
    [SerializeField] protected Sprite _star;

    protected IEnumerator _productionCoroutine;
    float _currentProductionDelaySecond;

    BuildingProduction _buildingProduction;

    protected List<BattleCreature> _producedCreatures = new();

    IEnumerator _corruptedProductionCoroutine;

    public override void Initialize(Vector3 pos, Building building)
    {
        base.Initialize(pos, building);
        _buildingProduction = building as BuildingProduction;

        _buildingProduction.OnUpgradePurchased += OnUpgradePurchased;
        _battleFightManager.OnWaveSpawned += SpawnWave;
    }

    protected virtual void SpawnWave()
    {
        int difficulty = _battleFightManager.CurrentDifficulty;
        BuildingUpgrade bu = _buildingProduction.GetCurrentUpgrade();

        // TODO: difficulty
        List<Entity> entitiesToSpawn = new();
        for (int i = 0; i < difficulty * 3; i++)
            entitiesToSpawn.Add(Instantiate(bu.ProducedCreature));

        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab, _spawnPoint.position, transform.rotation);
        spawner.SpawnEntities(entitiesToSpawn, team: 1);
        spawner.OnSpawnComplete += (l) =>
        {
            _battleManager.AddOpponentArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    protected override void Secured()
    {
        base.Secured();

        _starRenderers[0].sprite = _star;

        _battleFightManager.OnWaveSpawned -= SpawnWave;
        StartProductionCoroutine();
    }

    protected override void OnUpgradePurchased()
    {
        _starRenderers[_buildingProduction.CurrentLevel.Value - 1].sprite = _star;

        Vector3 scale = transform.localScale + Vector3.one;
        transform.DOScale(scale, 1f)
            .SetEase(Ease.OutBack);
        transform.DOLocalMoveY(scale.x * 0.5f, 1f)
            .SetEase(Ease.OutBack);

        StartProductionCoroutine();
    }

    void StartProductionCoroutine()
    {
        if (!_building.IsSecured) return;
        if (_productionCoroutine != null) return;

        _productionCoroutine = ProductionCoroutine();
        StartCoroutine(_productionCoroutine);
    }

    IEnumerator ProductionCoroutine()
    {
        yield return new WaitForSeconds(2f);

        while (_producedCreatures.Count < _buildingProduction.GetCurrentUpgrade().ProductionLimit)
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

        Creature creature = Instantiate(_buildingProduction.GetCurrentUpgrade().ProducedCreature);
        spawner.SpawnEntities(new List<Entity>() { creature }, portalElement: creature.Element);
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

    IEnumerator ProductionDelay()
    {
        float totalDelay = _buildingProduction.GetCurrentUpgrade().ProductionDelay;
        _currentProductionDelaySecond = 0f;
        while (_currentProductionDelaySecond < totalDelay)
        {
            _currentProductionDelaySecond += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    /* CORRUPTION */
    public override void Corrupted()
    {
        base.Corrupted();

        if (_productionCoroutine != null) StopCoroutine(_productionCoroutine);

        _corruptedProductionCoroutine = CorruptedProductionCoroutine();
        StartCoroutine(_corruptedProductionCoroutine);
    }

    IEnumerator CorruptedProductionCoroutine()
    {
        yield return new WaitForSeconds(2f);

        while (!_building.IsSecured)
        {
            SpawnHostileCreature();
            yield return ProductionDelay();
            yield return ProductionDelay(); // double delay
        }
    }

    void SpawnHostileCreature()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature creature = Instantiate(_buildingProduction.GetCurrentUpgrade().ProducedCreature);
        spawner.SpawnEntities(new List<Entity>() { creature }, portalElement: null, team: 1);
        spawner.OnSpawnComplete += (l) =>
        {
            // now I need to track the spawned wolf
            BattleCreature bc = l[0] as BattleCreature;

            _battleManager.AddOpponentArmyEntity(bc);
            spawner.DestroySelf();
        };
    }


    /* INTERACTION */
    public override void DisplayTooltip()
    {
        if (_tooltipManager == null) return;

        _tooltipManager.ShowTooltip(new BuildingProductionCard(_buildingProduction), gameObject);

        if (_buildingProduction.CurrentLevel.Value >= _buildingProduction.BuildingUpgrades.Length) return;
        if (!CanInteract(default)) return;

        _tooltipManager.ShowKeyTooltipInfo(
            new BattleInfoElement($"<b>Upgrade {Helpers.ParseScriptableObjectName(_building.name)}</b>"));
    }

    public override bool Interact(BattleInteractor interactor)
    {
        _buildingProduction.Upgrade();

        if (_buildingProduction.CurrentLevel.Value == _buildingProduction.BuildingUpgrades.Length)
            _tooltipManager.HideKeyTooltipInfo();

        return true;
    }

}
