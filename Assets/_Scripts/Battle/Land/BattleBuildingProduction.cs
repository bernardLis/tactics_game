using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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

    public override void Initialize(Vector3 pos, Building building)
    {
        base.Initialize(pos, building);
        _buildingProduction = building as BuildingProduction;
    }

    public override IEnumerator SecuredCoroutine()
    {
        yield return base.SecuredCoroutine();

        for (int i = 0; i < _buildingProduction.BuildingUpgrade.CurrentLevel + 1; i++)
            _starRenderers[i].sprite = _star;

        StartProductionCoroutine();
    }

    void StartProductionCoroutine()
    {
        if (!_building.IsSecured) return;
        if (_productionCoroutine != null) return;

        _productionCoroutine = ProductionCoroutine();
        StartCoroutine(_productionCoroutine);
    }

    void StopProductionCoroutine()
    {
        if (_productionCoroutine != null) StopCoroutine(_productionCoroutine);
        _productionCoroutine = null;
    }

    IEnumerator ProductionCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Color c = _buildingProduction.ProducedCreature.Element.Color.Color;
        _progressBarHandler.SetBorderColor(c);
        _progressBarHandler.SetFillColor(Color.white);
        _progressBarHandler.SetProgress(0);
        _progressBarHandler.ShowProgressBar();

        while (_producedCreatures.Count < _buildingProduction.GetCurrentUpgrade().ProductionLimit)
        {
            SpawnFriendlyCreature();
            yield return ProductionDelay();
        }

        _progressBarHandler.HideProgressBar();
        _productionCoroutine = null;
    }

    void SpawnFriendlyCreature()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature creature = Instantiate(_buildingProduction.ProducedCreature);
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
        if (!_buildingProduction.IsSecured) totalDelay *= 2; // double delay for corrupted production

        _currentProductionDelaySecond = 0f;
        while (_currentProductionDelaySecond < totalDelay)
        {
            _currentProductionDelaySecond += 0.5f;
            _progressBarHandler.SetProgress(_currentProductionDelaySecond / totalDelay);

            yield return new WaitForSeconds(0.5f);
        }
    }

    /* CORRUPTION */
    public override void StartCorruption(BattleBoss boss)
    {
        base.StartCorruption(boss);
        StopProductionCoroutine();

        boss.OnCorruptionBroken += StartProductionCoroutine;
    }

    public override void Corrupted()
    {
        base.Corrupted();

        _productionCoroutine = CorruptedProductionCoroutine();
        StartCoroutine(_productionCoroutine);
    }

    IEnumerator CorruptedProductionCoroutine()
    {
        yield return new WaitForSeconds(2f);

        Color c = _gameManager.GameDatabase.GetColorByName("Corruption").Color;
        _progressBarHandler.SetBorderColor(c);
        _progressBarHandler.SetFillColor(Color.black);
        _progressBarHandler.SetProgress(0);
        _progressBarHandler.ShowProgressBar();

        while (!_building.IsSecured)
        {
            SpawnHostileCreature();
            yield return ProductionDelay();
        }

        _progressBarHandler.HideProgressBar();
    }

    void SpawnHostileCreature()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature creature = Instantiate(_buildingProduction.ProducedCreature);
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
    }

}
