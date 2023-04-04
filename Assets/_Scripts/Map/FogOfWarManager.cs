using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FogOfWarManager : MonoBehaviour
{
    Camera _cam;
    [SerializeField] GameObject _squarePrefab;
    Vector2 _bottomLeftCorner = new(-40.5f, -28.5f);
    int _width = 80;
    int _height = 80;

    List<FogOfWarSquare> _squares = new();
    List<FogOfWarObject> _fogOfWarObjects = new();
    List<FogOfWarEffector> _fogOfWarEffectors = new();

    void Start()
    {
        _cam = Camera.main;
        MapSetupManager setupManager = GetComponent<MapSetupManager>();
        setupManager.OnMapSetupFinished += Initialize;

        MapInputManager inputManager = GetComponent<MapInputManager>();
        inputManager.OnHeroMoving += OnHeroChangedPosition;
        inputManager.OnHeroTargetReached += OnHeroChangedPosition;
        Debug.Log($"start");
        // Create a grid of squares
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject square = Instantiate(_squarePrefab, transform);
                FogOfWarSquare info = square.GetComponent<FogOfWarSquare>();
                square.transform.position = _bottomLeftCorner + new Vector2(x, y);
                square.transform.parent = transform;
                _squares.Add(info);
            }
        }
    }

    public void Initialize()
    {
        _fogOfWarObjects = new(FindObjectsOfType<FogOfWarObject>());
        foreach (FogOfWarObject o in _fogOfWarObjects)
        {
            foreach (FogOfWarSquare square in _squares)
            {
                if (square.transform.position == o.transform.position)
                {
                    o.SetYourSquareInfo(square);
                    break;
                }
            }
        }

        _fogOfWarEffectors = new(FindObjectsOfType<FogOfWarEffector>());

        UpdateFogOfWar();
    }

    void OnHeroChangedPosition(MapHero hero)
    {
        UpdateFogOfWar(hero.GetComponent<FogOfWarEffector>());
    }

    public void UpdateFogOfWar(FogOfWarEffector effector)
    {
        foreach (FogOfWarSquare square in _squares)
        {
            if (Vector2.Distance(effector.transform.position, square.transform.position)
                      > effector.VisionRadius + 2)
                continue;

            square.ResetVisibility();

            if (Vector2.Distance(effector.transform.position, square.transform.position)
                    <= effector.ExploredRadius)
                square.SetExplored();

            if (Vector2.Distance(effector.transform.position, square.transform.position)
                    <= effector.VisionRadius)
                square.SetVisible();
        }
    }

    public void UpdateFogOfWar()
    {
        foreach (FogOfWarSquare square in _squares)
        {
            square.ResetVisibility();

            foreach (FogOfWarEffector e in _fogOfWarEffectors)
            {
                if (Vector2.Distance(e.transform.position, square.transform.position) <= e.ExploredRadius)
                    square.SetExplored();

                if (Vector2.Distance(e.transform.position, square.transform.position) <= e.VisionRadius)
                {
                    square.SetVisible();
                    break; // important
                }
            }
        }
    }
}