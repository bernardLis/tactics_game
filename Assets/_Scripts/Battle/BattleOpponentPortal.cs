using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class BattleOpponentPortal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Element Element;
    [SerializeField] GameObject _portalEffect;

    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    BattleWave _currentWave;

    List<BattleEntity> _spawnedEntities = new();

    float _lastWaveSpawnTime;

    public event Action OnWaveSpawned;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.AddPortal(this);
        _tooltipManager = BattleTooltipManager.Instance;
    }

    public void InitializeWave(BattleWave wave)
    {
        _tooltipManager.ShowInfo($"{Element.ElementName} portal is active.", 2f);

        _portalEffect.SetActive(true);
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
            if (_currentWave.CurrentGroupIndex != _currentWave.OpponentGroups.Count - 1) // don't wait after the last one is spawned
                yield return new WaitForSeconds(_currentWave.DelayBetweenGroups);
        }
        // HERE: spawn a reward chest?
        _portalEffect.SetActive(false);
    }

    IEnumerator SpawnCurrentOpponentGroup()
    {
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
        _tooltipManager.DisplayTooltip(c, gameObject);
    }

}
