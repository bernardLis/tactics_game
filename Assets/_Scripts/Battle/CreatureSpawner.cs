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
    [SerializeField] List<PortalElement> _portalElements = new();
    [SerializeField] GameObject _blackPortal;

    float _delay;

    public List<BattleEntity> SpawnedEntities = new();

    public event Action OnSpawnComplete;
    public void SpawnHeroArmy(Hero hero, float duration = 2f)
    {
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
        if (_hero != null)
            _portalElements.Find(x => x.ElementName == _hero.Element.ElementName).Portal.SetActive(true);
        else
            _blackPortal.SetActive(true);

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
        be.InitializeCreature(creature);
        SpawnedEntities.Add(be);

        Vector3 jumpPos = pos + transform.forward * 2f + Vector3.up + Vector3.left * Random.Range(-2, 2);
        instance.transform.DOJump(jumpPos, 1f, 1, 0.5f);
    }
}

[System.Serializable]
public struct PortalElement
{
    public ElementName ElementName;
    public GameObject Portal;
}

