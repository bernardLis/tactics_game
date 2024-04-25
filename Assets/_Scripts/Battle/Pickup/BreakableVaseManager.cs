using System;
using System.Collections;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Battle.Pickup
{
    public class BreakableVaseManager : PoolManager<BreakableVaseController>
    {
        InputManager _inputManager;
        ArenaManager _arenaManager;
        FightManager _fightManager;

        [FormerlySerializedAs("_vasePrefab")] [SerializeField]
        BreakableVaseController _vaseControllerPrefab;

        const int _vasesPerSpawn = 5;

        [SerializeField] bool _debugSpawnVase;

        public event Action<BreakableVaseController> OnVaseBroken;

        Camera _cam;
        Mouse _mouse;

        public void Initialize()
        {
            _arenaManager = GetComponent<ArenaManager>();
            _inputManager = GetComponent<InputManager>();
            _fightManager = GetComponent<FightManager>();

#if UNITY_EDITOR
            _cam = Camera.main;
            _inputManager.OnLeftMouseClick += DebugSpawnVase;
#endif
            CreatePool(_vaseControllerPrefab.gameObject);
            _fightManager.OnFightStarted += () => StartCoroutine(SpawnVasesCoroutine());
        }

        IEnumerator SpawnVasesCoroutine()
        {
            if (this == null) yield break;

            for (int i = 0; i < _vasesPerSpawn; i++)
            {
                yield return new WaitForSeconds(Random.Range(1f, 4f));
                Vector3 pos = _arenaManager.GetRandomPositionInArena();
                pos.y = 1;
                SpawnVase(pos);
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