using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleOpponentPortal : MonoBehaviour
{
    public Element Element;
    [SerializeField] GameObject _portalEffect;

    BattleManager _battleManager;

    BattleWave _currentWave;

    void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public void InitializeWave(BattleWave wave)
    {
        Debug.Log($"portal initialize wave");
        _portalEffect.SetActive(true);
        _currentWave = wave;
        StartCoroutine(HandleSpawningGroups());
    }

    IEnumerator HandleSpawningGroups()
    {
        while (_currentWave.CurrentGroupIndex < _currentWave.OpponentGroups.Count)
        {
            SpawnCurrentOpponentGroup();
            _currentWave.CurrentGroupIndex++;
            Debug.Log($"delay between groups {_currentWave.DelayBetweenGroups}");
            yield return new WaitForSeconds(_currentWave.DelayBetweenGroups);
        }
        Debug.Log($"end of wave {_currentWave.Element}");
        // HERE: spawn a reward chest?
        _portalEffect.SetActive(false);
    }

    void SpawnCurrentOpponentGroup()
    {
        OpponentGroup group = _currentWave.GetCurrentOpponentGroup();
        Debug.Log($"spawning opp group with minions: {group.Minions.Count}");

        List<Entity> entities = new(group.Minions);
        entities.AddRange(group.Creatures);

        foreach (Entity e in entities)
            SpawnEntity(e);

    }

    void SpawnEntity(Entity entity)
    {
        Debug.Log($"spawn entity {entity.name}");
        entity.InitializeBattle(null);

        Vector3 pos = transform.position;
        GameObject instance = Instantiate(entity.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity);

        Vector3 jumpPos = pos + transform.forward * 2f + Vector3.up + Vector3.left * Random.Range(-2, 2);
        instance.transform.DOJump(jumpPos, 1f, 1, 0.5f);
        _battleManager.AddOpponentArmyEntity(be);
    }
}
