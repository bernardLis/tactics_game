using System;
using System.Collections;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleVaseManager : PoolManager<BattleBreakableVase>
    {
        BattleManager _battleManager;
        BattleInputManager _battleInputManager;
        BattleAreaManager _battleAreaManager;

        [SerializeField] BattleBreakableVase _vasePrefab;

        const int _vasesPerSpawn = 5; // HERE: disabled vases

        [SerializeField] bool _debugSpawnVase;

        public event Action<BattleBreakableVase> OnVaseBroken;

        Camera _cam;
        Mouse _mouse;

        public void Initialize()
        {
            _battleManager = GetComponent<BattleManager>();
            _battleAreaManager = GetComponent<BattleAreaManager>();
            _battleInputManager = GetComponent<BattleInputManager>();

#if UNITY_EDITOR
            _cam = Camera.main;
            _battleInputManager.OnLeftMouseClick += DebugSpawnVase;
#endif
            CreatePool(_vasePrefab.gameObject);
            StartCoroutine(SpawnVasesCoroutine());
        }

        IEnumerator SpawnVasesCoroutine()
        {
            while (true)
            {
                if (_battleManager.GetTimeLeft() < 0) yield break;
                yield return new WaitForSeconds(Random.Range(2f, 5f)); // HERE: balance -> maybe longer

                for (int i = 0; i < _vasesPerSpawn; i++)
                {
                    BattleTile tile = _battleAreaManager.GetRandomUnlockedTile();

                    Vector3 pos = tile.GetRandomPositionOnTile();
                    SpawnVase(pos);
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }

        void SpawnVase(Vector3 position)
        {
            BattleBreakableVase vase = GetObjectFromPool();
            vase.Initialize(position);
            vase.OnBroken += VaseBroken;
        }

        void VaseBroken(BattleBreakableVase vase)
        {
            OnVaseBroken?.Invoke(vase);
            vase.OnBroken -= VaseBroken;
        }

        public void BreakAllVases()
        {
            foreach (BattleBreakableVase vase in GetActiveObjects())
                if (vase.gameObject.activeSelf)
                    vase.TriggerBreak();
        }

        void DebugSpawnVase()
        {
            if (!_debugSpawnVase) return;

            Vector3 mousePosition = _mouse.position.ReadValue();
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                return;
            SpawnVase(hit.point);
        }
    }
}