using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MapSetupManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] GameObject _battlePrefab;
    [SerializeField] GameObject _heroPrefab;
    [SerializeField] GameObject _collectablePrefab;

    public List<GameObject> Heroes = new();

    [SerializeField] Battle _battle;

    [SerializeField] CollectableGold _collectableGold;
    [SerializeField] CollectableSpice _collectableSpice;
    [SerializeField] CollectableItem _collectableItem;

    void Start()
    {
        _gameManager = GameManager.Instance;
        PlaceCharacters();
        PlaceCollectables();
        PlaceBattle();

        AstarPath.active.Scan();
    }

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
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-15, 11);
            float y = Random.Range(-4, -13);
            Vector2 pos = new Vector2(x + 0.5f, y + 0.5f);
            GameObject instance = Instantiate(_collectablePrefab, pos, Quaternion.identity);

            float v = Random.value;
            if (v < 0.3f)
                PlaceGold(instance);
            else if (v >= 0.3f && v < 0.6f)
                PlaceSpice(instance);
            else
                PlaceItem(instance);
        }
    }

    void PlaceGold(GameObject instance)
    {
        CollectableGold g = Instantiate(_collectableGold);
        g.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(g);
    }

    void PlaceSpice(GameObject instance)
    {
        CollectableSpice s = Instantiate(_collectableSpice);
        s.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(s);
    }

    void PlaceItem(GameObject instance)
    {
        CollectableItem i = Instantiate(_collectableItem);
        i.Initialize();
        instance.GetComponent<MapCollectable>().Initialize(i);
    }

    void PlaceBattle()
    {
        Battle b = Instantiate(_battle);
        b.RandomizeBattle();

        GameObject instance = Instantiate(_battlePrefab, new Vector3(6.5f, -5.5f), Quaternion.identity);
        instance.GetComponent<MapBattle>().Initialize(b);
    }
}
