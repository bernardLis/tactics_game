using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalSphereManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    [SerializeField] GameObject _elementalSpherePrefab;
    List<ElementalSphere> _elementalSpheres = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = GetComponent<BattleManager>();
        _battleManager.OnBattleInitialized += SpawnSpheres;
    }

    void SpawnSpheres()
    {
        List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
        foreach (Element e in elements)
            InstantiateSphere(e);
    }

    void InstantiateSphere(Element e)
    {
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), 10f, Random.Range(-5f, 5f));
        GameObject sphere = Instantiate(_elementalSpherePrefab, pos, Quaternion.identity);
        sphere.transform.parent = _battleManager.EntityHolder;
        ElementalSphere es = sphere.GetComponent<ElementalSphere>();
        es.Initialize(e);
        _elementalSpheres.Add(es);
    }
}
