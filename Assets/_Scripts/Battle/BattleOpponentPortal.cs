using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BattleOpponentPortal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    AudioManager _audioManager;
    BattleManager _battleManager;
    BattleWaveManager _battleWaveManager;
    BattleTooltipManager _tooltipManager;

    [SerializeField] Sound _portalElementalAlarmSound;

    [SerializeField] BattleEntitySpawner _entitySpawnerPrefab;
    BattleEntitySpawner _entitySpawnerInstance;

    [SerializeField] GameObject RewardChestPrefab;

    public Element Element;

    Vector3 _portalPosition = new(1.1f, 1.8f, 0f);
    Vector3 _portalEffectScale = new(1.6f, 1.9f, 2f);

    BattleWave _currentWave;
    [SerializeField] List<string> _portalLog = new();

    public event Action<BattleOpponentPortal> OnPortalOpened;
    public event Action<BattleOpponentPortal> OnPortalClosed;

    public event Action OnGroupSpawned;
    void Start()
    {
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleWaveManager = BattleWaveManager.Instance;
        _battleWaveManager.AddPortal(this);
        _tooltipManager = BattleTooltipManager.Instance;
    }

    public void GetWave(BattleWave wave)
    {
        _currentWave = wave;

        if (_battleManager.GetTime() >= wave.StartTime)
        {
            InitializeWave(wave);
            return;
        }

        StartCoroutine(WaitToSpawnWave(wave));
    }

    IEnumerator WaitToSpawnWave(BattleWave wave)
    {
        float timeToWait = wave.StartTime - _battleManager.GetTime();
        _portalLog.Add($"{_battleManager.GetTime()}: Waiting to spawn wave. Time to wait {timeToWait}");
        yield return new WaitForSeconds(timeToWait);
        InitializeWave(wave);
    }

    public void InitializeWave(BattleWave wave)
    {
        if (_battleManager == null) _battleManager = BattleManager.Instance;

        _portalLog.Add($"{_battleManager.GetTime()}: Initializing wave. Wave start time {wave.StartTime}");
        _portalLog.Add($"Delay between groups {wave.DelayBetweenGroups}");
        _portalLog.Add($"Number of groups {wave.OpponentGroups.Count - 1}");

        _entitySpawnerInstance = Instantiate(_entitySpawnerPrefab, transform);
        _entitySpawnerInstance.transform.localPosition = _portalPosition;
        _entitySpawnerInstance.ShowPortal(Element, _portalEffectScale);

        _tooltipManager.ShowInfo($"{Element.ElementName} portal is active.", 2f);
        _audioManager.PlayUI(_portalElementalAlarmSound);

        _currentWave.IsStarted = true;

        if (_tooltipManager.CurrentTooltipDisplayer == gameObject)
            ShowTooltip(); // refreshing tooltip

        OnPortalOpened?.Invoke(this);
        StartCoroutine(HandleSpawningGroups());
    }

    IEnumerator HandleSpawningGroups()
    {
        while (_currentWave.CurrentGroupIndex < _currentWave.OpponentGroups.Count)
        {
            yield return SpawnCurrentOpponentGroup();
            _currentWave.SpawningGroupFinished();
            _portalLog.Add($"{_battleManager.GetTime()} Finished spawning group index: {_currentWave.CurrentGroupIndex}");
            if (_currentWave.CurrentGroupIndex != _currentWave.OpponentGroups.Count) // don't wait after the last one is spawned
                yield return new WaitForSeconds(_currentWave.DelayBetweenGroups - 1); // -1  to account for the 1s delay in spawning the group  
        }

        yield return ClosePortal();
    }

    IEnumerator SpawnCurrentOpponentGroup()
    {
        _portalLog.Add($"{_battleManager.GetTime()}: Started spawning group index: {_currentWave.CurrentGroupIndex}");

        OpponentGroup group = _currentWave.GetCurrentOpponentGroup();

        List<Entity> entities = new(group.Minions);
        entities.AddRange(group.Creatures);

        _entitySpawnerInstance.SpawnEntities(entities, Element, 2, true);
        _entitySpawnerInstance.OnSpawnComplete += (list) =>
        {
            _battleManager.AddOpponentArmyEntities(new(list));
            OnGroupSpawned?.Invoke();
            _entitySpawnerInstance.ClearSpawnedEntities();
        };

        yield return null;
    }

    IEnumerator ClosePortal()
    {
        OnPortalClosed?.Invoke(this);

        if (_tooltipManager.CurrentTooltipDisplayer == gameObject)
            ShowTooltip(); // refreshing tooltip

        yield return new WaitForSeconds(1.5f);

        _portalLog.Add($"{_battleManager.GetTime()}: Closing portal");
        yield return SpawnChest();

        _entitySpawnerInstance.DestroySelf();
    }

    IEnumerator SpawnChest()
    {
        Vector3 pos = _entitySpawnerInstance.transform.position;
        GameObject chest = Instantiate(RewardChestPrefab, pos, Quaternion.identity);
        chest.transform.LookAt(Vector3.zero);
        Vector3 jumpPos = pos + _entitySpawnerInstance.transform.forward * Random.Range(2f, 4f);
        yield return chest.transform.DOJump(jumpPos, 1f, 1, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo(new BattleInfoElement("Details"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return;

        ShowTooltip();
    }

    void ShowTooltip()
    {
        OpponentPortalCard tt = new OpponentPortalCard(Element, _currentWave);
        _tooltipManager.ShowTooltip(tt, gameObject);
    }
}
