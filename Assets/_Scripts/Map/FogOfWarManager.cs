using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FogOfWarManager : MonoBehaviour
{
    Camera _cam;
    [SerializeField] GameObject _squarePrefab;
    Vector2 _topLeftCorner = new(-40.5f, -28.5f);
    int _width = 80;
    int _height = 80;

    List<FogOfWarSquareInfo> _squares = new();
    MapHero[] _heroes;
    MapCastle[] _castles;

    void Start()
    {
        _cam = Camera.main;


        // Create a grid of squares
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject square = Instantiate(_squarePrefab, transform);
                FogOfWarSquareInfo info = square.GetComponent<FogOfWarSquareInfo>();
                square.transform.position = _topLeftCorner + new Vector2(x, y);
                square.transform.parent = transform;
                _squares.Add(info);
            }
        }
    }

    public void Initialize()
    {
        _heroes = FindObjectsOfType<MapHero>();
        _castles = FindObjectsOfType<MapCastle>();
    }

    void Update()
    {
        if (_squares.Count > 0)
            UpdateFogOfWar();
    }

    public void UpdateFogOfWar()
    {
        List<Vector2> visibilityProducers = new();
        foreach (MapHero h in _heroes)
            visibilityProducers.Add(new Vector2(h.transform.position.x, h.transform.position.y));
        foreach (MapCastle c in _castles)
            visibilityProducers.Add(new Vector2(c.transform.position.x, c.transform.position.y));

        foreach (FogOfWarSquareInfo square in _squares)
        {
            SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
            if (square.WasExplored)
                sr.color = new Color(0, 0, 0, 0.5f);
            else
                sr.color = Color.black;

            foreach (Vector2 v in visibilityProducers)
            {
                if (Vector2.Distance(v, square.transform.position) < 5)
                {
                    square.WasExplored = true;
                    sr.color = Color.clear;
                }
            }

        }

    }
}