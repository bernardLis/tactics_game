using System;
using System.Collections;
using Lis.Battle.Arena;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Battle.Pickup
{
    public class BreakableVaseManager : PoolManager<BreakableVaseController>
    {
        BattleManager _battleManager;
        InputManager _inputManager;
        ArenaManager _arenaManager;

        [FormerlySerializedAs("_vasePrefab")] [SerializeField]
        BreakableVaseController _vaseControllerPrefab;

        const int _vasesPerSpawn = 5;

        [SerializeField] bool _debugSpawnVase;

        public event Action<BreakableVaseController> OnVaseBroken;

        Camera _cam;
        Mouse _mouse;

        public void Initialize()
        {
            _battleManager = GetComponent<BattleManager>();
            _arenaManager = GetComponent<ArenaManager>();
            _inputManager = GetComponent<InputManager>();

#if UNITY_EDITOR
            _cam = Camera.main;
            _inputManager.OnLeftMouseClick += DebugSpawnVase;
#endif
            CreatePool(_vaseControllerPrefab.gameObject);
            StartCoroutine(SpawnVasesCoroutine());
        }

        IEnumerator SpawnVasesCoroutine()
        {
            while (true)
            {
                if (this == null) yield break;
                yield return new WaitForSeconds(Random.Range(10f, 20f)); // HERE: balance -> maybe longer

                for (int i = 0; i < _vasesPerSpawn; i++)
                {
                    Vector3 pos = _arenaManager.GetRandomPosition();
                    SpawnVase(pos);
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }

        void SpawnVase(Vector3 position)
        {
            BreakableVaseController vaseController = GetObjectFromPool();
            vaseController.Initialize(position);
            vaseController.OnBroken += VaseBroken;
        }

        void VaseBroken(BreakableVaseController vaseController)
        {
            OnVaseBroken?.Invoke(vaseController);
            vaseController.OnBroken -= VaseBroken;
        }

        public void BreakAllVases()
        {
            foreach (BreakableVaseController vase in GetActiveObjects())
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