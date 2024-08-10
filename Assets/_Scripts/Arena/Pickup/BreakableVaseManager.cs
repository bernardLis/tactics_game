using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Arena.Pickup
{
    public class BreakableVaseManager : PoolManager<BreakableVaseController>
    {
        const int _vasesPerSpawn = 5;

        [FormerlySerializedAs("_vasePrefab")] [SerializeField]
        BreakableVaseController _vaseControllerPrefab;

        [SerializeField] bool _debugSpawnVase;
        ArenaManager _arenaManager;

        Camera _cam;
        FightManager _fightManager;
        InputManager _inputManager;
        Mouse _mouse;

        public event Action<BreakableVaseController> OnVaseBroken;

        public void Initialize()
        {
            _arenaManager = ArenaManager.Instance;
            _inputManager = GetComponent<InputManager>();
            _fightManager = GetComponent<FightManager>();

#if UNITY_EDITOR
            _cam = Camera.main;
            _inputManager.OnLeftMouseClick += DebugSpawnVase;
#endif
            CreatePool(_vaseControllerPrefab.gameObject);
            _fightManager.OnFightStarted += SpawnVases;
        }

        public void SpawnVases()
        {
            StartCoroutine(SpawnVasesCoroutine());
        }

        public void DisableVaseSpawning()
        {
            _fightManager.OnFightStarted -= SpawnVases;
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

        public BreakableVaseController GetRandomActiveVase()
        {
            List<BreakableVaseController> activeVases = GetActiveObjects();
            if (activeVases.Count == 0) return null;
            return activeVases[Random.Range(0, activeVases.Count)];
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