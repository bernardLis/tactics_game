using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class CreatureSpawner : MonoBehaviour
{
    BattleManager _battleManager;

    Hero _hero;
    List<Creature> _creatures = new();

    float _delay;

    public List<BattleEntity> SpawnedEntities = new();

    public event Action OnSpawnComplete;
    public void Initialize(Hero hero, float duration = 2f)
    {
        Debug.Log($"hero {hero}");
        Debug.Log($"_hero.army count {hero.Army.Count}");
        SpawnCreatures(hero.Army, hero, duration);
    }

    public void SpawnCreatures(List<Creature> creatures, Hero hero = null, float duration = 2f)
    {
        _creatures = creatures;
        _hero = hero;
        _delay = duration / _creatures.Count;

        StartCoroutine(SpawnShow());
    }

    IEnumerator SpawnShow()
    {
        // HERE: choose portal color
        for (int i = 0; i < _creatures.Count; i++)
        {
            SpawnCreature(_creatures[i]);
            yield return new WaitForSeconds(_delay);
        }

        OnSpawnComplete?.Invoke();
    }

    void SpawnCreature(Creature creature)
    {
        creature.InitializeBattle(_hero);

        Vector3 pos = transform.position;
        GameObject instance = Instantiate(creature.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.SpawnCreature(creature);
        SpawnedEntities.Add(be);

        Vector3 jumpPos = pos + transform.forward * 2f + Vector3.up + Vector3.left * Random.Range(-2, 2);
        instance.transform.DOJump(jumpPos, 1f, 1, 0.5f);
    }
}
