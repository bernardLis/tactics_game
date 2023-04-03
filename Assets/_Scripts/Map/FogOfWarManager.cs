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
    List<FogOfWarObject> _fogOfWarObjects = new();
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
        _fogOfWarObjects = new List<FogOfWarObject>(FindObjectsOfType<FogOfWarObject>());
        foreach (FogOfWarObject o in _fogOfWarObjects)
        {
            foreach (FogOfWarSquareInfo square in _squares)
            {
                if (square.transform.position == o.transform.position)
                {
                    o.SetYourSquareInfo(square);
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (_squares.Count > 0)
            UpdateFogOfWar();
    }

    public void UpdateFogOfWar()
    {
        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        foreach (FogOfWarSquareInfo square in _squares)
        {
            square.IsVisible = false;

            SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
            if (square.IsExplored)
                sr.color = new Color(0, 0, 0, 0.5f);
            else
                sr.color = Color.black;

            if (Vector2.Distance(worldPos, square.transform.position) < 5)
            {
                square.IsExplored = true;
                square.IsVisible = true;
                sr.color = Color.clear;
            }

        }

        /*
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
        */

    }



    // TODO: There must be a better way to do this like have squares in dictionary or something
    // and check dict for position

}