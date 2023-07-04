using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class CreatureSpawner : MonoBehaviour
{
    Hero _hero;
    Element _portalElement;
    List<Creature> _creatures = new();
    [SerializeField] List<PortalElement> _portalElements = new();
    [SerializeField] GameObject _blackPortal;

    float _delay;

    public List<BattleEntity> SpawnedEntities = new();

    public event Action<List<BattleEntity>> OnSpawnComplete;
    public void SpawnHeroArmy(Hero hero, float duration = 2f)
    {
        SpawnCreatures(hero.Army, hero, duration);
    }

    public void SpawnCreatures(List<Creature> creatures, Hero hero = null,
                        float duration = 2f, Element portalElement = null)
    {
        _creatures = creatures;

        _hero = hero;
        if (_hero != null) _portalElement = _hero.Element;
        else _portalElement = portalElement;

        _delay = duration / _creatures.Count;

        StartCoroutine(SpawnShow());
    }

    IEnumerator SpawnShow()
    {
        if (_portalElement != null)
            _portalElements.Find(x => x.ElementName == _portalElement.ElementName).Portal.SetActive(true);
        else
            _blackPortal.SetActive(true);

        for (int i = 0; i < _creatures.Count; i++)
        {
            SpawnCreature(_creatures[i]);
            yield return new WaitForSeconds(_delay);
        }

        OnSpawnComplete?.Invoke(SpawnedEntities);
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

    public void DestroySelf()
    {
        transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
        Destroy(gameObject, 1f);
    }
}

[System.Serializable]
public struct PortalElement
{
    public ElementName ElementName;
    public GameObject Portal;
}

