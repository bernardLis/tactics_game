using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntitySpawner : MonoBehaviour
{
    BattleManager _battleManager;

    [SerializeField] bool _respawnToKeepNumberOfEntities;
    [SerializeField] int _numberOfEntities;
    [SerializeField] List<ArmyGroup> ArmiesToSpawn = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;

        for (int i = 0; i < _numberOfEntities; i++)
        {
            Creature entity = ArmiesToSpawn[Random.Range(0, ArmiesToSpawn.Count)].Creature;
            SpawnEntity(entity);
        }
    }
    void SpawnEntity(Creature entity)
    {
        Vector3 pos = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
        instance.transform.parent = transform;
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.Initialize(1, entity, ref _battleManager.PlayerEntities);
        be.OnDeath += OnDeath;

    }

    void OnDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        StartCoroutine(CleanBody(be));
        if (!_respawnToKeepNumberOfEntities) return;

        Creature entity = ArmiesToSpawn[Random.Range(0, ArmiesToSpawn.Count)].Creature;
        SpawnEntity(entity);
    }

    IEnumerator CleanBody(BattleEntity be)
    {
        yield return new WaitForSeconds(4f);
        Destroy(be.gameObject);
    }
}
