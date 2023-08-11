using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class EntitySpawner : MonoBehaviour
{
    Hero _hero;
    Element _portalElement;
    List<Entity> _entities = new();
    [SerializeField] List<PortalElement> _portalElements = new();
    [SerializeField] GameObject _blackPortal;

    float _delay;
    bool _portalShown;

    public List<BattleEntity> SpawnedEntities = new();

    public event Action<List<BattleEntity>> OnSpawnComplete;

    public void ShowPortal(Element element)
    {
        if (_portalShown) return;
        _portalShown = true;

        if (element == null)
        {
            _blackPortal.SetActive(true);
            return;
        }

        _portalElements.Find(x => x.ElementName == element.ElementName).Portal.SetActive(true);
    }

    public void SpawnCreatures(List<Creature> creatures, float duration = 2f)
    {
        _delay = duration / creatures.Count;
        _portalElement = creatures[0].Element;

        SpawnEntities(creatures: creatures);
    }

    public void SpawnMinions(List<Minion> minions, Element portalElement = null, float duration = 2f)
    {
        _portalElement = portalElement;
        _delay = duration / minions.Count;

        SpawnEntities(minions: minions);
    }

    public void SpawnEntities(List<Creature> creatures = null, List<Minion> minions = null)
    {
        if (creatures != null)
            _entities = new(creatures);
        if (minions != null)
            _entities = new(minions);

        StartCoroutine(SpawnShow());
    }

    IEnumerator SpawnShow()
    {
        ShowPortal(_portalElement);

        for (int i = 0; i < _entities.Count; i++)
        {
            SpawnEntity(_entities[i]);
            yield return new WaitForSeconds(_delay);
        }

        OnSpawnComplete?.Invoke(SpawnedEntities);
        Invoke("DestroySelf", 1f);
    }

    void SpawnEntity(Entity entity)
    {
        entity.InitializeBattle(_hero);

        Vector3 pos = transform.position;
        GameObject instance = Instantiate(entity.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity);
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

[Serializable]
public struct PortalElement
{
    public ElementName ElementName;
    public GameObject Portal;
}

