using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MapSetupManager : MonoBehaviour
{
    GameManager _gameManager;
    Map _currentMap;

    FogOfWarManager _fogOfWarManager;

    [SerializeField] GameObject _heroPrefab;
    [SerializeField] GameObject _collectablePrefab;
    [SerializeField] GameObject _battlePrefab;
    [SerializeField] GameObject _castlePrefab;

    public List<MapHero> MapHeroes = new();
    public List<MapCastle> MapCastles = new();

    public event Action OnMapSetupFinished;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _currentMap = _gameManager.Map;
        _fogOfWarManager = GetComponent<FogOfWarManager>();

        PlaceCharacters();
        PlaceCollectables();
        PlaceBattles();
        PlaceCastles();

        AstarPath.active.Scan();
        OnMapSetupFinished?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Map")]
    void ResetMap()
    {
        _currentMap.Reset();
        PlaceCollectables();
        PlaceBattles();

        AstarPath.active.Scan();
    }
#endif

    void PlaceCharacters()
    {
        foreach (Character c in _gameManager.GetAllCharacters())
        {
            GameObject instance = Instantiate(_heroPrefab, c.MapPosition, Quaternion.identity);
            MapHero mapHero = instance.GetComponent<MapHero>();
            mapHero.Initialize(c);
            MapHeroes.Add(mapHero);
        }
    }

    void PlaceCollectables()
    {
        foreach (Collectable c in _currentMap.Collectables)
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

    void PlaceBattles()
    {
        foreach (Battle b in _currentMap.Battles)
        {
            GameObject instance = Instantiate(_battlePrefab, b.MapPosition, Quaternion.identity);
            instance.GetComponent<MapBattle>().Initialize(b);
        }
    }

    void PlaceCastles()
    {
        foreach (Castle c in _currentMap.Castles)
        {
            GameObject instance = Instantiate(_castlePrefab, c.MapPosition, Quaternion.identity);
            MapCastle mc = instance.GetComponent<MapCastle>();
            mc.Initialize(c);
            MapCastles.Add(mc);
        }
    }
}

