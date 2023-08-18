using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class BattleOpponentPortal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    [SerializeField] GameObject RewardChestPrefab;

    public Element Element;
    [SerializeField] GameObject _portalEffect;
    Vector3 _portalEffectScale;

    BattleWave _currentWave;
    [SerializeField] List<string> _portalLog = new();


    List<BattleEntity> _spawnedEntities = new();

    float _lastWaveSpawnTime;

    public event Action OnWaveSpawned;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.AddPortal(this);
        _tooltipManager = BattleTooltipManager.Instance;

        _portalEffectScale = _portalEffect.transform.localScale;
    }

    public void InitializeWave(BattleWave wave)
    {
        _portalLog.Add($"{Time.time} Initializing wave. Wave start time {wave.StartTime}");
        _portalLog.Add($"Delay between groups {wave.DelayBetweenGroups}");
        _portalLog.Add($"Number of groups {wave.OpponentGroups.Count - 1}");

        _tooltipManager.ShowInfo($"{Element.ElementName} portal is active.", 2f);

        _portalEffect.transform.localScale = Vector3.zero;
        _portalEffect.transform.DOScale(_portalEffectScale, 0.5f)
            .OnComplete(() =>
            {
                _portalEffect.SetActive(true);
            });

        _currentWave = wave;
        StartCoroutine(HandleSpawningGroups());
    }

    IEnumerator HandleSpawningGroups()
    {
        while (_currentWave.CurrentGroupIndex < _currentWave.OpponentGroups.Count)
        {
            yield return SpawnCurrentOpponentGroup();
            _lastWaveSpawnTime = Time.time;
            _currentWave.SpawningGroupFinished();
            _portalLog.Add($"{Time.time} Finished spawning group index: {_currentWave.CurrentGroupIndex}");
            if (_currentWave.CurrentGroupIndex != _currentWave.OpponentGroups.Count) // don't wait after the last one is spawned
                yield return new WaitForSeconds(_currentWave.DelayBetweenGroups - 1); // -1  to account for the 1s delay in spawning the group  
        }

        yield return ClosePortal();
    }

    IEnumerator SpawnCurrentOpponentGroup()
    {
        _portalLog.Add($"{Time.time} Started spawning group index: {_currentWave.CurrentGroupIndex}");

        OpponentGroup group = _currentWave.GetCurrentOpponentGroup();

        List<Entity> entities = new(group.Minions);
        entities.AddRange(group.Creatures);
        float delay = 1f / entities.Count;

        foreach (Entity e in entities)
        {
            SpawnEntity(e);
            yield return new WaitForSeconds(delay);
        }

        _battleManager.AddOpponentArmyEntities(_spawnedEntities);
        _spawnedEntities.Clear();
        OnWaveSpawned?.Invoke();
    }

    void SpawnEntity(Entity entity)
    {
        entity.InitializeBattle(null);

        Vector3 pos = _portalEffect.transform.position;
        pos.y = 1;
        GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity);

        Vector3 jumpPos = pos + _portalEffect.transform.forward * Random.Range(2f, 4f)
            + Vector3.left * Random.Range(-2f, 2f);
        jumpPos.y = 1;
        instance.transform.DOJump(jumpPos, 1f, 1, 0.5f);
        _spawnedEntities.Add(be);
    }

    IEnumerator ClosePortal()
    {
        yield return new WaitForSeconds(3f);

        _portalLog.Add($"{Time.time} Closing portal");
        Vector3 pos = _portalEffect.transform.position;
        GameObject chest = Instantiate(RewardChestPrefab, pos, Quaternion.identity);
        chest.transform.LookAt(Vector3.zero);
        Vector3 jumpPos = pos + _portalEffect.transform.forward * Random.Range(2f, 4f);
        chest.transform.DOJump(jumpPos, 1f, 1, 0.5f);

        yield return new WaitForSeconds(0.8f);

        _portalEffect.transform.DOScale(0, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _portalEffect.SetActive(false);
            });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for details.");
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
        BattleWaveCard c = new(_currentWave, _lastWaveSpawnTime);
        _tooltipManager.ShowTooltip(c, gameObject);
    }

}
