using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MapSetupManager : MonoBehaviour
{
    GameManager _gameManager;
    Map _currentMap;

    [SerializeField] GameObject _heroPrefab;
    [SerializeField] GameObject _collectablePrefab;
    [SerializeField] GameObject _battlePrefab;
    [SerializeField] GameObject _castlePrefab;

    public List<GameObject> Heroes = new();

    [SerializeField] Battle _battle;

    [SerializeField] CollectableGold _collectableGold;
    [SerializeField] CollectableSpice _collectableSpice;
    [SerializeField] CollectableItem _collectableItem;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _currentMap = _gameManager.Map;

        PlaceCharacters();
        PlaceCollectables();
        PlaceBattles();
        PlaceCastles();

        AstarPath.active.Scan();
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
            instance.GetComponent<MapHero>().Initialize(c);
            Heroes.Add(instance);
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
        Debug.Log($"_currentMap.Castles.Count  {_currentMap.Castles.Count}");
        foreach (Castle c in _currentMap.Castles)
        {
            GameObject instance = Instantiate(_castlePrefab, c.MapPosition, Quaternion.identity);
            instance.GetComponent<MapCastle>().Initialize(c);
        }

    }
}
