using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FogOfWarManager : MonoBehaviour
{
    Map _currentMap;

    Camera _cam;
    [SerializeField] GameObject _squarePrefab;
    Vector2 _bottomLeftCorner = new(-40.5f, -28.5f);//new(0.5f, 0.5f); //new(-40.5f, -28.5f);
    int _width = 80;
    int _height = 80;

    List<FogOfWarSquare> _squares = new();
    List<FogOfWarObject> _fogOfWarObjects = new();
    List<FogOfWarEffector> _fogOfWarEffectors = new();

    List<int> _exploredListPositions = new();

    void Start()
    {
        _cam = Camera.main;
        MapSetupManager setupManager = GetComponent<MapSetupManager>();
        _currentMap = setupManager.CurrentMap;
        setupManager.OnMapSetupFinished += Initialize;

        MapInputManager inputManager = GetComponent<MapInputManager>();
        inputManager.OnHeroMoving += OnHeroChangedPosition;
        inputManager.OnHeroTargetReached += DelayedFogOfWarUpdate;

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

        foreach (int listPos in _currentMap.ExploredListPositions)
            _squares[listPos].SetExplored();

        _fogOfWarEffectors = new(FindObjectsOfType<FogOfWarEffector>());

        UpdateFogOfWar();
    }

    // I could do something smarter but this will work.
    void DelayedFogOfWarUpdate(MapHero hero)
    {
        Invoke(nameof(UpdateFogOfWar), 0.3f);
    }

    void OnHeroChangedPosition(MapHero hero)
    {
        UpdateFogOfWar(hero.GetComponent<FogOfWarEffector>());
    }

    void UpdateFogOfWar(FogOfWarEffector effector)
    {
        int delta = Mathf.CeilToInt(effector.ExploreRadius) + 2;
        for (int i = -delta; i < delta; i++)
        {
            for (int j = -delta; j < delta; j++)
            {
                float effectorPosX = (int)effector.transform.position.x + 0.5f;
                float effectorPosY = (int)effector.transform.position.y + 0.5f;

                float squarePosX = effectorPosX + i;
                float squarePosY = effectorPosY + j;
                int x = Mathf.RoundToInt(squarePosX - _bottomLeftCorner.x);
                int y = Mathf.RoundToInt(squarePosY - _bottomLeftCorner.y);

                int listPos = y + _width * x;
                FogOfWarSquare square = _squares[listPos];
                square.ResetVisibility();
                UpdateSquare(square, listPos);
            }
        }
    }

    public void UpdateFogOfWar()
    {
        // I'd have to know where in the list is square at a given world position
        // https://blog.gemserk.com/2018/11/20/implementing-fog-of-war-for-rts-games-in-unity-2-2/
        foreach (FogOfWarSquare square in _squares)
            UpdateSquare(square, _squares.IndexOf(square));
    }

    void UpdateSquare(FogOfWarSquare square, int listPos)
    {
        square.ResetVisibility();
        foreach (FogOfWarEffector e in _fogOfWarEffectors)
        {
            if (!e.IsOwnedByPlayer) continue;
            if (Vector2.Distance(e.transform.position, square.transform.position) <= e.ExploreRadius)
            {
                if (!_currentMap.ExploredListPositions.Contains(listPos))
                    _currentMap.ExploredListPositions.Add(listPos);
                square.SetExplored();
            }

            if (Vector2.Distance(e.transform.position, square.transform.position) <= e.VisionRadius)
            {
                square.SetVisible();
                break; // important
            }
        }
    }
}
