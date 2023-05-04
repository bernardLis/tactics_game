using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MapSetupManager : MonoBehaviour
{
    GameManager _gameManager;
    public Map CurrentMap { get; private set; }

    [SerializeField] GameObject _heroPrefab;
    [SerializeField] GameObject _collectablePrefab;
    [SerializeField] GameObject _battlePrefab;
    [SerializeField] GameObject _castlePrefab;

    public List<MapHero> MapHeroes = new();
    public List<MapCastle> MapCastles = new();

    public event Action OnMapSetupFinished;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        CurrentMap = _gameManager.Map;
    }

    void Start()
    {
        PlaceHeroes();
        PlaceCollectables();
        PlaceCastles();

        AstarPath.active.Scan();
        OnMapSetupFinished?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Map")]
    void ResetMap()
    {
        CurrentMap.Reset();
        PlaceCollectables();

        AstarPath.active.Scan();
    }
#endif

    void PlaceHeroes()
    {
        GameObject instance = Instantiate(_heroPrefab, Vector2.zero, Quaternion.identity);
        MapHero mapHero = instance.GetComponent<MapHero>();
        mapHero.Initialize(_gameManager.PlayerHero);
        MapHeroes.Add(mapHero);
    }

    void PlaceCollectables()
    {
        foreach (Collectable c in CurrentMap.Collectables)
        {
            if (c.IsCollected)
                continue;
            GameObject instance = Instantiate(_collectablePrefab, c.MapPosition, Quaternion.identity);
            if (c.GetType().ToString() == "CollectableGold")
                PlaceGold((CollectableGold)c, instance);
            if (c.GetType().ToString() == "CollectableSpice")
                PlaceSpice((CollectableSpice)c, instance);
            if (c.GetType().ToString() == "CollectableItem")
                PlaceItem((CollectableItem)c, instance);
        }
    }

    void PlaceGold(CollectableGold g, GameObject instance)
    {
        g.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(g);
    }

    void PlaceSpice(CollectableSpice s, GameObject instance)
    {
        s.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(s);
    }

    void PlaceItem(CollectableItem i, GameObject instance)
    {
        i.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(i);
    }

    void PlaceCastles()
    {
        foreach (Castle c in CurrentMap.Castles)
        {
            GameObject instance = Instantiate(_castlePrefab, c.MapPosition, Quaternion.identity);
            MapCastle mc = instance.GetComponent<MapCastle>();
            mc.Initialize(c);
            MapCastles.Add(mc);
        }
    }
}

