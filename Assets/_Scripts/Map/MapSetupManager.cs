using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetupManager : MonoBehaviour
{
    GameManager _gameManager;
    [SerializeField] GameObject _heroPrefab;

    public List<GameObject> Heroes = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        PlaceCharacters();
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


}
