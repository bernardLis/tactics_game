using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class BattleBuildingProduction : BattleBuilding
    {
        [FormerlySerializedAs("_spawnerPrefab")] [SerializeField]
        protected BattleEntitySpawner SpawnerPrefab;

        [FormerlySerializedAs("_spawnPoint")] [SerializeField]
        protected Transform SpawnPoint;

        [FormerlySerializedAs("_starRenderers")] [SerializeField]
        protected SpriteRenderer[] StarRenderers;

        [FormerlySerializedAs("_star")] [SerializeField]
        protected Sprite Star;

        IEnumerator _productionCoroutine;
        float _currentProductionDelaySecond;

        BuildingProduction _buildingProduction;

        readonly List<BattleCreature> _producedCreatures = new();
        
        public override void Initialize(Vector3 pos, Building building)
        {
            base.Initialize(pos, building);
            _buildingProduction = building as BuildingProduction;
        }

        protected override IEnumerator SecuredCoroutine()
        {
            yield return base.SecuredCoroutine();

            for (int i = 0; i < _buildingProduction.BuildingUpgrade.CurrentLevel + 1; i++)
                StarRenderers[i].sprite = Star;

            StartProductionCoroutine();
        }

        void StartProductionCoroutine()
        {
            if (!Building.IsSecured) return;
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
            Color c = _buildingProduction.ProducedCreature.Element.Color.Primary;
            ProgressBarHandler.SetBorderColor(c);
            ProgressBarHandler.SetFillColor(Color.white);
            ProgressBarHandler.SetProgress(0);
            ProgressBarHandler.ShowProgressBar();

            while (_producedCreatures.Count < _buildingProduction.GetCurrentUpgrade().ProductionLimit)
            {
                SpawnHostileCreature();
                yield return ProductionDelay();
            }

            ProgressBarHandler.HideProgressBar();
            _productionCoroutine = null;
        }

        void SpawnFriendlyCreature()
        {
            BattleEntitySpawner spawner = Instantiate(SpawnerPrefab,
                SpawnPoint.position, transform.rotation);

            Creature creature = Instantiate(_buildingProduction.ProducedCreature);
            spawner.SpawnEntities(new List<Entity>() { creature }, portalElement: creature.Element);
            spawner.OnSpawnComplete += (l) =>
            {
                // now I need to track the spawned wolf
                BattleCreature bc = l[0] as BattleCreature;
                _producedCreatures.Add(bc);

                BattleManager.AddPlayerArmyEntities(l);
                spawner.DestroySelf();

                if (bc == null) return;
                bc.OnDeath += (_, __) =>
                {
                    _producedCreatures.Remove(bc);
                    StartProductionCoroutine();
                };
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
                ProgressBarHandler.SetProgress(_currentProductionDelaySecond / totalDelay);

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

        protected override void Corrupted()
        {
            base.Corrupted();

            _productionCoroutine = CorruptedProductionCoroutine();
            StartCoroutine(_productionCoroutine);
        }

        IEnumerator CorruptedProductionCoroutine()
        {
            yield return new WaitForSeconds(2f);

            Color c = GameManager.GameDatabase.GetColorByName("Corruption").Primary;
            ProgressBarHandler.SetBorderColor(c);
            ProgressBarHandler.SetFillColor(Color.black);
            ProgressBarHandler.SetProgress(0);
            ProgressBarHandler.ShowProgressBar();

            while (!Building.IsSecured)
            {
                SpawnHostileCreature();
                yield return ProductionDelay();
            }

            ProgressBarHandler.HideProgressBar();
        }

        void SpawnHostileCreature()
        {
            BattleEntitySpawner spawner = Instantiate(SpawnerPrefab,
                SpawnPoint.position, transform.rotation);

            Creature creature = Instantiate(_buildingProduction.ProducedCreature);
            spawner.SpawnEntities(new List<Entity>() { creature }, portalElement: null, team: 1);
            spawner.OnSpawnComplete += (l) =>
            {
                // now I need to track the spawned wolf
                BattleCreature bc = l[0] as BattleCreature;

                BattleManager.AddOpponentArmyEntity(bc);
                spawner.DestroySelf();
            };
        }

        /* INTERACTION */
        public override void DisplayTooltip()
        {
            if (TooltipManager == null) return;

            TooltipManager.ShowTooltip(new BuildingProductionCard(_buildingProduction), gameObject);
        }
    }
}