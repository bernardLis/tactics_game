using System;
using System.Collections;
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

        readonly int _vasesPerSpawn = 5;

        [SerializeField] bool _debugSpawnVase;

        public event Action<BattleBreakableVase> OnVaseBroken;

        Camera _cam;

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
                if (_battleManager.IsBossFight()) yield break;
                yield return new WaitForSeconds(Random.Range(10f, 20f));

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

            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            int layerMask = Tags.BattleFloorLayer;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            {
                Vector3 pos = hit.point;
                pos.y = 0.5f;
                Debug.Log($"hit.point {hit.point}");
                SpawnVase(pos);
            }
        }
    }
}