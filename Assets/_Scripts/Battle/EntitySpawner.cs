using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class EntitySpawner : MonoBehaviour
{
    AudioManager _audioManager;

    [SerializeField] Sound _portalOpenSound;
    [SerializeField] Sound _portalCloseSound;
    [SerializeField] Sound _portalHumSound;
    [SerializeField] Sound _portalPopEntitySound;

    Element _portalElement;
    List<Entity> _entities = new();
    [SerializeField] List<PortalElement> _portalElements = new();
    [SerializeField] GameObject _blackPortal;

    float _delay;
    bool _portalShown;
    AudioSource _portalHumSource;

    public List<BattleEntity> SpawnedEntities = new();

    public event Action<List<BattleEntity>> OnSpawnComplete;
    void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    public void ShowPortal(Element element, Vector3 scale = default)
    {
        if (_portalShown) return;
        _portalShown = true;

        if (scale == default) scale = Vector3.one * 3f;

        _audioManager.PlaySFX(_portalOpenSound, transform.position);
        _portalHumSource = _audioManager.PlaySFX(_portalHumSound, transform.position, true);
        GameObject portal = _blackPortal;
        if (element != null)
            portal = _portalElements.Find(x => x.ElementName == element.ElementName).Portal;

        portal.transform.localScale = Vector3.zero;
        portal.SetActive(true);
        portal.transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack);
    }

    public void SpawnEntities(List<Entity> entities, Element portalElement = null, float duration = 2f)
    {
        _entities = new(entities);
        _portalElement = portalElement;
        _delay = duration / _entities.Count;

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
        Invoke(nameof(DestroySelf), 1f);
    }

    void SpawnEntity(Entity entity)
    {
        _audioManager.PlaySFX(_portalPopEntitySound, transform.position);

        entity.InitializeBattle(0);

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
        _audioManager.PlaySFX(_portalCloseSound, transform.position);
        if (_portalHumSource != null)
            _portalHumSource.Stop();

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

