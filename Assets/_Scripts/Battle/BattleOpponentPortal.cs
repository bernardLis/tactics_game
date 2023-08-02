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

    List<BattleEntity> _spawnedEntities = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public void InitializeWave(BattleWave wave)
    {
        _portalEffect.SetActive(true);
        _currentWave = wave;
        StartCoroutine(HandleSpawningGroups());
    }

    IEnumerator HandleSpawningGroups()
    {
        while (_currentWave.CurrentGroupIndex < _currentWave.OpponentGroups.Count)
        {
            yield return SpawnCurrentOpponentGroup();
            _currentWave.CurrentGroupIndex++;
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
        float delay = 0.5f;

        foreach (Entity e in entities)
        {
            SpawnEntity(e);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.5f);

        _battleManager.AddOpponentArmyEntities(_spawnedEntities);
        _spawnedEntities.Clear();
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
        // instance.GetComponent<Rigidbody>().isKinematic = false;// HERE: minion kinematic
        _spawnedEntities.Add(be);
    }
}
