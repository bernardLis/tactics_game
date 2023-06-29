using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntitySpawner : MonoBehaviour
{
    BattleManager _battleManager;

    [SerializeField] bool _respawnToKeepNumberOfEntities;
    [SerializeField] int _numberOfEntities;
    [SerializeField] List<Creature> Creatures = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        StartCoroutine(SpawnShow());
        /*
            for (int i = 0; i < _numberOfEntities; i++)
            {
                Creature c = Creatures[Random.Range(0, Creatures.Count)];
                SpawnCreature(c);
            }
      */
    }

    IEnumerator SpawnShow()
    {
        for (int i = 0; i < _numberOfEntities; i++)
        {
            Creature c = Creatures[Random.Range(0, Creatures.Count)];
            yield return SpawnCreature(c);
            yield return new WaitForSeconds(1f);
        }
    }


    IEnumerator SpawnCreature(Creature creature)
    {
        Vector3 pos = transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        GameObject instance = Instantiate(creature.Prefab, pos, Quaternion.identity);
        instance.transform.parent = transform;
        BattleEntity be = instance.GetComponent<BattleEntity>();

        Vector3 jumpPos = pos + Vector3.forward * 2f + Vector3.up;
        yield return instance.transform.DOJump(jumpPos, 1f, 1, 0.5f).WaitForCompletion();

        //yield return new WaitForSeconds(1f);
        be.Initialize(0, creature, ref _battleManager.PlayerEntities);
        be.OnDeath += OnDeath;
    }


    void OnDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        StartCoroutine(CleanBody(be));
        if (!_respawnToKeepNumberOfEntities) return;

        Creature c = Creatures[Random.Range(0, Creatures.Count)];
        StartCoroutine(SpawnCreature(c));
    }

    IEnumerator CleanBody(BattleEntity be)
    {
        yield return new WaitForSeconds(4f);
        Destroy(be.gameObject);
    }
}
